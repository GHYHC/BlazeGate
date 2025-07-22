using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement.Remote
{
    /// <summary>
    /// 授权WebApi
    /// </summary>
    public class AuthWebApi : BaseWebApi
    {
        private readonly IAuthTokenStorageServices authTokenStorage;

        private Task<ApiResult<AuthTokenDto>> refreshTask = null;
        private object lockObj = new object();
        private static SemaphoreSlim refreshTokenSemaphore = new SemaphoreSlim(1, 1);

        public AuthWebApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.authTokenStorage = serviceProvider.GetRequiredService<IAuthTokenStorageServices>();
        }

        public override async Task<TResult> HttpPostJsonAsync<TValue, TResult>(string requestUri, TValue value, int timeout = 100)
        {
            string url = CombineUrl(WebApiAddress, requestUri);

            var httpClient = await CreateAuthHttpClient(timeout);
            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(url, value);

            //判断是否授权过期
            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && httpResponse.Headers.TryGetValues("x-access-token", out IEnumerable<string>? headerValue) && headerValue != null && headerValue.Contains("expired"))
            {
                var result = await RefreshTokenAsync();
                if (result.Success)
                {
                    // 刷新Token成功后重新请求
                    httpClient = await CreateAuthHttpClient(timeout);
                    httpResponse = await httpClient.PostAsJsonAsync(url, value);
                }
                else
                {
                    throw new Exception("刷新Token失败: " + result.Msg);
                }
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
            }

            return await httpResponse.Content.ReadFromJsonAsync<TResult>();
        }

        /// <summary>
        /// 刷新Token(同一个类下同时刷新，只会刷新一次)
        /// </summary>
        /// <returns></returns>
        private Task<ApiResult<AuthTokenDto>> RefreshTokenAsync()
        {
            // 双重检查锁定
            if (refreshTask != null)
            {
                return refreshTask;
            }

            lock (lockObj)
            {
                // 再次检查是否已经被其他线程初始化
                if (refreshTask != null)
                {
                    return refreshTask;
                }

                // 初始化刷新Token的任务
                refreshTask = InternalRefreshTokenAsync().ContinueWith(b =>
                {
                    // 重置刷新Token的任务，允许下一次刷新
                    refreshTask = null;
                    return b.Result;
                });

                return refreshTask;
            }
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        private async Task<ApiResult<AuthTokenDto>> InternalRefreshTokenAsync()
        {
            await refreshTokenSemaphore.WaitAsync();
            try
            {
                string url = new Uri(new Uri(BlazeGateAddress), $"/api/Account/RefreshToken?serviceName={ServiceName}").ToString();
                AuthTokenDto authToken = await authTokenStorage.GetAuthToken();

                var httpClient = await CreateHttpClient();

                HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(url, authToken);
                var result = await httpResponse.Content.ReadFromJsonAsync<ApiResult<AuthTokenDto>>();
                if (result.Success)
                {
                    await authTokenStorage.SetAuthToken(result.Data);
                }
                return result;
            }
            catch (Exception ex)
            {
                return ApiResult<AuthTokenDto>.FailResult(ex.Message);
            }
            finally
            {
                refreshTokenSemaphore.Release();
            }
        }

        /// <summary>
        /// 创建授权HttpClient
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<HttpClient> CreateAuthHttpClient(int? timeout)
        {
            var httpClient = await CreateHttpClient(timeout);

            //设置授权信息
            AuthTokenDto authToken = await authTokenStorage.GetAuthToken();
            if (authToken != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken.AccessToken);
            }

            return httpClient;
        }
    }
}