using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IAccountService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<ApiResult<AuthTokenDto>> Login(LoginParam param);

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public Task<ApiResult<AuthTokenDto>> RefreshToken(string serviceName, AuthTokenDto? authToken);

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public Task<ApiResult<string>> Logout(string serviceName, AuthTokenDto? authToken);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public Task<ApiResult<UserDto>> GetUser(string serviceName);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<ApiResult<string>> ChangePassword(ChangePasswordParam param);
    }
}