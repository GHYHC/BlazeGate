using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BlazeGate.Services.Implement.Remote
{
    /// <summary>
    /// 授权WebApi
    /// </summary>
    public class AuthWebApi : BaseWebApi
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAuthTokenStorageServices authTokenStorage;

        private Task<bool> refreshTask = null;
        private object lockObj = new object();
        private static SemaphoreSlim refreshTokenSemaphore = new SemaphoreSlim(1, 1);

        public AuthWebApi(IHttpClientFactory httpClientFactory, IAuthTokenStorageServices authTokenStorage, IConfiguration configuration) : base(httpClientFactory, configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.authTokenStorage = authTokenStorage;
        }

        public override async Task<TResult> HttpPostJsonAsync<TValue, TResult>(string requestUri, TValue value, int timeout = 100)
        {
            string url = CombineUrl(WebApiAddress, requestUri);

            HttpResponseMessage httpResponse = await AuthPostAsJsonAsync(url, value, timeout);

            //判断是否授权过期
            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && httpResponse.Headers.TryGetValues("x-access-token", out IEnumerable<string>? headerValue) && headerValue != null && headerValue.Contains("expired"))
            {
                // 刷新Token成功后重新请求
                if (await RefreshTokenAsync())
                {
                    httpResponse = await AuthPostAsJsonAsync(url, value, timeout);
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
        private Task<bool> RefreshTokenAsync()
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
        private async Task<bool> InternalRefreshTokenAsync()
        {
            await refreshTokenSemaphore.WaitAsync();
            try
            {
                string url = new Uri(new Uri(BlazeGateAddress), $"/api/Account/RefreshToken?serviceName={ServiceName}").ToString();
                AuthTokenDto authToken = await authTokenStorage.GetAuthToken();

                var httpClient = httpClientFactory.CreateClient();

                HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(url, authToken);
                ApiResult<AuthTokenDto> result = await httpResponse.Content.ReadFromJsonAsync<ApiResult<AuthTokenDto>>();
                if (result.Success)
                {
                    await authTokenStorage.SetAuthToken(result.Data);
                    return true;
                }
                return false;
            }
            finally
            {
                refreshTokenSemaphore.Release();
            }
        }

        /// <summary>
        /// 授权Post
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> AuthPostAsJsonAsync<TValue>([StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri, TValue value, int timeout = 100)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeout);

            //设置授权信息
            AuthTokenDto authToken = await authTokenStorage.GetAuthToken();
            if (authToken != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken.AccessToken);
            }

            return await httpClient.PostAsJsonAsync(requestUri, value);
        }
    }
}