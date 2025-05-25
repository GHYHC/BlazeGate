using System.Net.Http.Json;

namespace BlazeGate.Common
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// 发送一个使用 JSON 序列化的 Post 请求
        /// </summary>
        /// <typeparam name="TValue">请求参数</typeparam>
        /// <typeparam name="TResult">响应结果</typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri">请求地址</param>
        /// <param name="value">请求参数</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<TResult> HttpPostAsJsonAsync<TValue, TResult>(this System.Net.Http.HttpClient client, string requestUri, TValue value)
        {
            var httpResponse = await client.PostAsJsonAsync(requestUri, value);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
            }
            return await httpResponse.Content.ReadFromJsonAsync<TResult>();
        }
    }
}