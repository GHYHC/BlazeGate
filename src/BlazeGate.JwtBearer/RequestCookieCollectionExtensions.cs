using BlazeGate.Model.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.JwtBearer
{
    public static class RequestCookieCollectionExtensions
    {
        /// <summary>
        /// 从cookies中获取access token
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static AuthTokenDto GetAuthToken(this IRequestCookieCollection cookies)
        {
            if (cookies.TryGetValue(HeaderNames.Authorization, out var json) && !string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<AuthTokenDto>(json);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
