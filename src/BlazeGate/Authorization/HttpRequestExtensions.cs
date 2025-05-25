namespace BlazeGate.Authorization
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 获取服务信息
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="serviceName"></param>
        /// <param name="path"></param>
        public static void GetServiceInfo(this HttpRequest httpRequest, out string serviceName, out string path)
        {
            serviceName = "";
            path = "";

            var paths = httpRequest.Path.Value?.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();
            if (paths != null && paths.Count > 0)
            {
                serviceName = paths[0].ToLower();
                paths.RemoveAt(0);
            }

            if (paths != null && paths.Count > 0)
            {
                path = "/" + string.Join("/", paths).ToLower();
            }
        }
    }
}
