using Azure.Core;
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
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement.Remote
{
    /// <summary>
    /// WebApi基类
    /// </summary>
    public class BaseWebApi
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHttpClientFactory httpClientFactory;
        public readonly IConfiguration configuration;
        private readonly IAppCultureStorageService appCultureStorageService;

        /// <summary>
        /// WebApi地址(后台接口地址)
        /// </summary>
        public string WebApiAddress { get; set; }

        /// <summary>
        /// BlazeGate地址
        /// </summary>
        public string BlazeGateAddress { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        public BaseWebApi(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            this.configuration = serviceProvider.GetRequiredService<IConfiguration>();
            this.appCultureStorageService = serviceProvider.GetService<IAppCultureStorageService>();

            BlazeGateAddress = configuration["BlazeGate:BlazeGateAddress"];
            ServiceName = configuration["BlazeGate:ServiceName"];
            WebApiAddress = configuration["BlazeGate:WebApiAddress"];

            //如果WebApiAddress为空，则使用BlazeGateAddress和ServiceName拼接
            if (string.IsNullOrWhiteSpace(WebApiAddress))
            {
                WebApiAddress = CombineUrl(BlazeGateAddress, ServiceName);
            }
        }

        /// <summary>
        /// 发送HttpGet请求
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="timeout">默认超时时间100s</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual async Task<TResult> HttpPostJsonAsync<TValue, TResult>(string requestUri, TValue value, int timeout = 100)
        {
            string url = CombineUrl(WebApiAddress, requestUri);

            var httpClient = await CreateHttpClient(timeout);

            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(url, value);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
            }

            //如果TResult是String则用httpResponse.Content.ReadAsStringAsync()读取内容
            if (typeof(TResult) == typeof(string))
            {
                var str = await httpResponse.Content.ReadAsStringAsync();
                // 需要强制转换为 TResult 返回
                return (TResult)(object)str;
            }
            else
            {
                return await httpResponse.Content.ReadFromJsonAsync<TResult>();
            }
        }

        /// <summary>
        /// 合并Url
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static string CombineUrl(string url1, string url2)
        {
            if (string.IsNullOrWhiteSpace(url1))
            {
                return url2;
            }
            if (string.IsNullOrWhiteSpace(url2))
            {
                return url1;
            }

            string url = url1.TrimEnd('/') + "/" + url2.TrimStart('/');
            return url;
        }

        /// <summary>
        /// 创建HttpClient
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public virtual async Task<HttpClient> CreateHttpClient(int? timeout = null)
        {
            var httpClient = httpClientFactory.CreateClient();

            //设置超时时间
            if (timeout != null)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeout.Value);
            }

            //设置默认当前系统的语言
            httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(System.Globalization.CultureInfo.CurrentCulture.Name));

            //设置请求的语言头
            if (appCultureStorageService != null)
            {
                var appCultureInfo = await appCultureStorageService.GetAppCulture();
                if (appCultureInfo != null)
                {
                    httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
                    httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(string.IsNullOrWhiteSpace(appCultureInfo.Culture) ? appCultureInfo.UICulture : appCultureInfo.Culture));
                }
            }

            return httpClient;
        }
    }
}