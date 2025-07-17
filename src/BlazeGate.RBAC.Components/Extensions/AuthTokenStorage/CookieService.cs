using Microsoft.JSInterop;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    /// <summary>
    /// 通过 JavaScript 互操作实现的 Cookie 管理服务
    /// 支持设置、获取和删除 Cookie，提供灵活的过期时间控制和安全选项
    /// </summary>
    public class CookieService : ICookieService
    {
        private readonly IJSRuntime _jsRuntime;

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// 设置 Cookie 值，支持灵活的过期时间和安全选项
        /// </summary>
        /// <param name="key">Cookie 名称</param>
        /// <param name="value">Cookie 值（将自动进行 URL 编码）</param>
        /// <param name="expiresIn">相对过期时间（从当前时刻算起）</param>
        /// <param name="expiresAt">绝对过期时间（精确到具体日期和时间）</param>
        /// <param name="path">Cookie 路径，指定哪些路径可以访问该 Cookie</param>
        /// <param name="domain">Cookie 域名，指定哪些域名可以访问该 Cookie</param>
        /// <param name="secure">是否仅通过 HTTPS 传输（默认 false）</param>
        /// <param name="httpOnly">是否禁止 JavaScript 访问（默认 false）</param>
        /// <param name="sameSite">控制跨站请求时 Cookie 的携带规则，可选值：strict/lax/none</param>
        /// <remarks>
        /// 注意事项：
        /// 1. 同时提供 expiresIn 和 expiresAt 时，优先使用 expiresIn
        /// 2. 设置 sameSite=none 时，必须同时设置 secure=true
        /// 3. HttpOnly 为 true 时，JavaScript 无法通过 document.cookie 访问该 Cookie
        /// </remarks>
        public async Task SetCookieAsync<T>(
            string key,
            T value,
            TimeSpan? expiresIn = null,    // 相对时间（如：30分钟后过期）
            DateTimeOffset? expiresAt = null,  // 绝对时间（如：2025年12月31日过期）
            string path = "/",             // 默认全站可见
            string domain = null,          // 默认仅当前域名可见
            bool secure = false,            // 默认仅限 HTTPS
            bool httpOnly = false,         // 默认允许 JavaScript 访问
            string sameSite = "lax"        // 默认防止大部分 CSRF 攻击
        )
        {
            var encodedKey = System.Net.WebUtility.UrlEncode(key);

            //判断T是否为字符串类型，如果不是则进行序列化
            var valueString = value is string strValue ? strValue : System.Text.Json.JsonSerializer.Serialize(value);
            valueString = System.Net.WebUtility.UrlEncode(valueString);

            if (sameSite?.ToLower() == "none" && !secure)
                secure = true; // 强制secure

            // 构建 Cookie 字符串
            var cookieBuilder = new StringBuilder();
            cookieBuilder.Append($"{encodedKey}={valueString}");

            // 处理过期时间（优先使用相对时间，同时设置 max-age 和 expires 以兼容新旧浏览器）
            if (expiresIn.HasValue)
            {
                // 设置 max-age（现代浏览器优先使用）
                var maxAgeSeconds = (int)expiresIn.Value.TotalSeconds;
                cookieBuilder.Append($"; max-age={maxAgeSeconds}");

                // 为兼容旧浏览器（如 IE8），同时设置 expires
                var expiresDate = DateTimeOffset.UtcNow.Add(expiresIn.Value);
                cookieBuilder.Append($"; expires={expiresDate:R}");
            }
            else if (expiresAt.HasValue)
            {
                // 仅设置 expires（适用于需要绝对时间的场景，如特定日期促销活动）
                cookieBuilder.Append($"; expires={expiresAt.Value:R}");
            }
            // 未提供过期时间时，默认为会话 Cookie（浏览器关闭时过期）

            // 设置路径（控制哪些路径可以访问该 Cookie）
            if (!string.IsNullOrEmpty(path))
                cookieBuilder.Append($"; path={path}");

            // 设置域名（控制哪些域名可以访问该 Cookie）
            if (!string.IsNullOrEmpty(domain))
                cookieBuilder.Append($"; domain={domain}");

            // 安全标志（仅限 HTTPS 传输）
            if (secure)
                cookieBuilder.Append("; secure");

            // HttpOnly 标志（禁止 JavaScript 访问）
            if (httpOnly)
                cookieBuilder.Append("; HttpOnly");

            // SameSite 标志（控制跨站请求时 Cookie 的携带规则）
            if (!string.IsNullOrEmpty(sameSite))
                cookieBuilder.Append($"; samesite={sameSite}");

            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "eval",
                    $"document.cookie = '{cookieBuilder.ToString().Replace("'", "\\'")}'"
                );
            }
            catch
            {
                // eval 失败时回退到 blazorCookie.set
                await _jsRuntime.InvokeVoidAsync("blazorCookie.set", cookieBuilder.ToString());
            }
        }

        /// <summary>
        /// 获取指定名称的 Cookie 值
        /// </summary>
        /// <param name="key">Cookie 名称</param>
        /// <returns>Cookie 值，若不存在则返回 null</returns>
        public async Task<T> GetCookieAsync<T>(string key)
        {
            var encodedKey = System.Net.WebUtility.UrlEncode(key);

            string json;

            try
            {
                json = await _jsRuntime.InvokeAsync<string>(
                    "eval",
                    $@"(function() {{
                            var row = document.cookie.split('; ').find(row => row.startsWith('{encodedKey}='));
                            return row ? row.substring({encodedKey.Length + 1}) : null;
                        }})()"
                );
            }
            catch
            {
                // eval 失败时回退到 blazorCookie.get
                json = await _jsRuntime.InvokeAsync<string>("blazorCookie.get", encodedKey);
            }

            if (json == null)
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)System.Net.WebUtility.UrlDecode(json);
            }
            else
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(System.Net.WebUtility.UrlDecode(json));
            }
        }

        /// <summary>
        /// 删除指定名称的 Cookie
        /// </summary>
        /// <param name="key">Cookie 名称</param>
        /// <param name="path">Cookie 路径（必须与设置时一致）</param>
        /// <param name="domain">Cookie 域名（必须与设置时一致）</param>
        public async Task DeleteCookieAsync(string key, string path = "/", string domain = null)
        {
            // 通过设置过期时间为过去来删除 Cookie
            // 同时设置 max-age=0 和 expires=过去时间以确保兼容性
            await SetCookieAsync(
                key: key,
                value: "",                  // 值设为空
                expiresIn: TimeSpan.Zero,   // 立即过期
                path: path,                 // 路径必须与设置时一致
                domain: domain              // 域名必须与设置时一致
            );
        }
    }

    public interface ICookieService
    {
        Task SetCookieAsync<T>(
            string key,
            T value,
            TimeSpan? expiresIn = null,
            DateTimeOffset? expiresAt = null,
            string path = "/",
            string domain = null,
            bool secure = false,
            bool httpOnly = false,
            string sameSite = "lax"
        );

        Task<T> GetCookieAsync<T>(string key);

        Task DeleteCookieAsync(string key, string path = "/", string domain = null);
    }
}