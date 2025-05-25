using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Helper
{
    public static class StringHelper
    {
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