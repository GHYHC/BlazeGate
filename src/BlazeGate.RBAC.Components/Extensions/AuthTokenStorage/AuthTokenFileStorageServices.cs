using AltairCA.Blazor.WebAssembly.Cookie;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    public class AuthTokenFileStorageServices : IAuthTokenStorageServices
    {
        private AuthTokenDto authToken;

        public AuthTokenFileStorageServices()
        {
        }

        public async Task<AuthTokenDto> GetAuthToken()
        {
            //如果authToken为空就从文件中读取
            if (authToken == null)
            {
                //读取文件
                var filePath = Path.Combine(AppContext.BaseDirectory, "AuthToken.json");
                if (File.Exists(filePath))
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    authToken = System.Text.Json.JsonSerializer.Deserialize<AuthTokenDto>(json);
                }
            }

            return authToken;
        }

        public async Task SetAuthToken(AuthTokenDto authToken)
        {
            this.authToken = authToken;
            //写入文件
            var filePath = Path.Combine(AppContext.BaseDirectory, "AuthToken.json");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            var json = System.Text.Json.JsonSerializer.Serialize(authToken);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}