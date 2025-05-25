using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement.Remote
{
    /// <summary>
    /// WebApi基类
    /// </summary>
    public class BaseWebApi
    {
        private readonly IHttpClientFactory httpClientFactory;
        public readonly IConfiguration configuration;

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

        public BaseWebApi(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;

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

            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeout);

            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(url, value);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
            }

            return await httpResponse.Content.ReadFromJsonAsync<TResult>();
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
    }
}