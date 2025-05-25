using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazeGate.Model.JwtBearer
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static UserDto GetUser(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return null;
            }

            var user = new UserDto
            {
                Id = long.Parse(claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"),
                UserName = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? "",
                Account = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? "",
                Phone = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone)?.Value ?? "",
                Remark = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "remark")?.Value ?? "",
            };

            //获取用户角色
            List<string> roleList = claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            if (roleList.Count > 0)
            {
                user.Roles = roleList;
            }

            //获取用户数据
            user.UserData = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value ?? "";
            if (!string.IsNullOrEmpty(user.UserData))
            {
                try
                {
                    user.UserDataDict = JsonSerializer.Deserialize<Dictionary<string, string>>(user.UserData);
                }
                catch (Exception ex)
                {
                    user.UserDataDict = new Dictionary<string, string>();
                }
            }

            return user;
        }

        /// <summary>
        /// 创建用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> CreateClaimByUser(this UserDto user)
        {
            if (user == null)
            {
                return new List<Claim>();
            }

            //用ClaimTypes
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Account ?? ""),
                new Claim(ClaimTypes.GivenName, user.UserName ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.Phone ?? ""),
                new Claim("remark", user.Remark ?? ""),
                new Claim(ClaimTypes.UserData, user.UserData ?? ""),
            };

            // 添加用户角色
            if (user.Roles != null && user.Roles.Count > 0)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return claims;
        }

        /// <summary>
        /// 创建页面ID
        /// </summary>
        /// <param name="pageIds"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> CreateClaimByPageId(List<long> pageIds)
        {
            if (pageIds == null)
            {
                return new List<Claim>();
            }

            var claims = new List<Claim>()
        {
            new Claim("page_id",string.Join('/',pageIds)),
        };

            return claims;
        }

        /// <summary>
        /// 获取页面ID
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static List<long> GetPageId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return null;
            }

            List<long> pageIds = new List<long>();
            string str = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "page_id")?.Value;
            if (str != null)
            {
                pageIds = str.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToList();
            }

            return pageIds;
        }
    }
}