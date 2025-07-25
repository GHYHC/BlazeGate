﻿using AutoMapper;
using BlazeGate.JwtBearer;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BlazeGateContext BlazeGateContext;
        private readonly IAuthTokenService authTokenService;
        private readonly IMapper mapper;
        private readonly IDistributedCache distributedCache;
        private readonly IStringLocalizer<I18n> l;

        public AccountController(BlazeGateContext BlazeGateContext, IAuthTokenService authTokenService, IMapper mapper, IDistributedCache distributedCache, IStringLocalizer<I18n> l)
        {
            this.BlazeGateContext = BlazeGateContext;
            this.authTokenService = authTokenService;
            this.mapper = mapper;
            this.distributedCache = distributedCache;
            this.l = l;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResult<AuthTokenDto>> Login(LoginParam param)
        {
            // 使用单个缓存键存储登录错误次数
            string cacheKey = $"login:BlazeGateAttempts:{param.Account}";
            string attemptsStr = await distributedCache.GetStringAsync(cacheKey);
            int attempts = 0;

            // 如果有缓存记录，则解析尝试次数
            if (!string.IsNullOrEmpty(attemptsStr) && int.TryParse(attemptsStr, out attempts))
            {
                // 如果尝试次数达到或超过3次，返回账号锁定信息
                if (attempts >= 3)
                {
                    return ApiResult<AuthTokenDto>.FailResult(l["LoginLocked"]);
                }
            }

            var user = BlazeGateContext.Users.AsNoTracking().Where(b => b.Account == param.Account && b.Password == param.Password && b.Enabled == true).FirstOrDefault();

            if (user == null)
            {
                // 登录失败，增加错误次数，并设置1分钟过期时间
                attempts++;
                await distributedCache.SetStringAsync(cacheKey, attempts.ToString(), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                });

                return ApiResult<AuthTokenDto>.FailResult(l["LoginFailed"]);
            }
            else
            {
                // 登录成功，清除错误计数
                await distributedCache.RemoveAsync(cacheKey);
            }

            //查询服务的私钥
            var privateKey = await BlazeGateContext.AuthRsaKeys.AsNoTracking().Where(b => b.ServiceName.ToLower() == param.ServiceName.ToLower()).Select(b => b.PrivateKey).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                return ApiResult<AuthTokenDto>.FailResult(l["ServiceKeyNotConfigured"]);
            }

            SigningCredentials signingCredentials = null;
            try
            {
                RsaSecurityKey rsaSecurityKey = new(RsaParametersHelper.PemToRsaParameters(privateKey));
                signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256Signature);
            }
            catch (Exception ex)
            {
                return ApiResult<AuthTokenDto>.FailResult(l["ServiceKeyFormatError"]);
            }

            //查询用户角色的Id
            var roleIds = await BlazeGateContext.UserRoles.AsNoTracking().Where(b => b.UserId == user.Id && b.ServiceName.ToLower() == param.ServiceName.ToLower()).Select(b => b.RoleId).ToListAsync();

            //查询角色的名称
            var roleNames = await BlazeGateContext.Roles.AsNoTracking().Where(b => roleIds.Contains(b.Id) && b.ServiceName.ToLower() == param.ServiceName.ToLower()).Select(b => b.RoleName).ToListAsync();

            var userDto = mapper.Map<User, UserDto>(user);
            userDto.Roles = roleNames;

            var token = await authTokenService.CreateAuthTokenAsync(userDto, param.ServiceName, signingCredentials);

            return ApiResult<AuthTokenDto>.SuccessResult(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResult<AuthTokenDto>> RefreshToken(string serviceName, AuthTokenDto? authToken)
        {
            try
            {
                //如果没有传递authToken，则从Cookie中获取
                authToken = authToken ?? Request.Cookies.GetAuthToken();

                //查询服务的私钥
                var privateKey = await BlazeGateContext.AuthRsaKeys.AsNoTracking().Where(b => b.ServiceName.ToLower() == serviceName.ToLower()).Select(b => b.PrivateKey).FirstOrDefaultAsync();
                if (string.IsNullOrWhiteSpace(privateKey))
                {
                    return ApiResult<AuthTokenDto>.FailResult(l["ServiceKeyNotConfigured"]);
                }

                SigningCredentials signingCredentials = null;
                try
                {
                    RsaSecurityKey rsaSecurityKey = new(RsaParametersHelper.PemToRsaParameters(privateKey));
                    signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256Signature);
                }
                catch (Exception ex)
                {
                    return ApiResult<AuthTokenDto>.FailResult(l["ServiceKeyFormatError"]);
                }

                //验证authToken
                var principal = authTokenService.GetClaimsPrincipal(authToken, signingCredentials);
                var tokenUser = principal.GetUser();

                var user = await BlazeGateContext.Users.AsNoTracking().Where(b => b.Id == tokenUser.Id && b.Enabled == true).FirstOrDefaultAsync();

                //如果用户不存在
                if (user == null)
                {
                    return ApiResult<AuthTokenDto>.FailResult(l["InvalidUser"]);
                }

                //验证更新时间是否一致
                if (user.UpdateTime != tokenUser.UpdateTime)
                {
                    return ApiResult<AuthTokenDto>.FailResult(l["UserInfoUpdated"]);
                }

                //移除令牌
                await authTokenService.RemoveAuthTokenAsync(authToken, signingCredentials, principal);

                //查询用户角色的Id
                var roleIds = await BlazeGateContext.UserRoles.AsNoTracking().Where(b => b.UserId == user.Id && b.ServiceName.ToLower() == serviceName.ToLower()).Select(b => b.RoleId).ToListAsync();

                //查询角色的名称
                var roleNames = await BlazeGateContext.Roles.AsNoTracking().Where(b => roleIds.Contains(b.Id) && b.ServiceName.ToLower() == serviceName.ToLower()).Select(b => b.RoleName).ToListAsync();

                //创建新的令牌
                var userDto = mapper.Map<User, UserDto>(user);
                userDto.Roles = roleNames;

                var token = await authTokenService.CreateAuthTokenAsync(userDto, serviceName, signingCredentials);
                return ApiResult<AuthTokenDto>.SuccessResult(token);
            }
            catch (BadHttpRequestException ex)
            {
                return ApiResult<AuthTokenDto>.FailResult(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResult<string>> Logout(string serviceName, AuthTokenDto? authToken)
        {
            try
            {
                //如果没有传递authToken，则从Cookie中获取
                authToken = authToken ?? Request.Cookies.GetAuthToken();

                //查询服务的私钥
                var privateKey = await BlazeGateContext.AuthRsaKeys.AsNoTracking().Where(b => b.ServiceName.ToLower() == serviceName.ToLower()).Select(b => b.PrivateKey).FirstOrDefaultAsync();
                if (string.IsNullOrWhiteSpace(privateKey))
                {
                    return ApiResult<string>.FailResult(l["ServiceKeyNotConfigured"]);
                }

                SigningCredentials signingCredentials = null;
                try
                {
                    RsaSecurityKey rsaSecurityKey = new(RsaParametersHelper.PemToRsaParameters(privateKey));
                    signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256Signature);
                }
                catch (Exception ex)
                {
                    return ApiResult<string>.FailResult(l["ServiceKeyFormatError"]);
                }

                var user = await authTokenService.RemoveAuthTokenAsync(authToken, signingCredentials);

                return ApiResult<string>.SuccessResult(user.Id.ToString());
            }
            catch (BadHttpRequestException ex)
            {
                return ApiResult<string>.FailResult(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ApiResult<UserDto>> GetUser(string serviceName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.GetUser();
                return await Task.FromResult(ApiResult<UserDto>.SuccessResult(user));
            }
            else
            {
                return await Task.FromResult(ApiResult<UserDto>.FailResult(l["NotAuthenticated"]));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ApiResult<string>> ChangePassword(ChangePasswordParam param)
        {
            var user = User.GetUser();

            // 使用单个缓存键存储修改密码错误次数
            string cacheKey = $"ChangePassword:BlazeGateAttempts:{user.Account}";
            string attemptsStr = await distributedCache.GetStringAsync(cacheKey);
            int attempts = 0;

            // 如果有缓存记录，则解析尝试次数
            if (!string.IsNullOrEmpty(attemptsStr) && int.TryParse(attemptsStr, out attempts))
            {
                // 如果尝试次数达到或超过3次，返回账号锁定信息
                if (attempts >= 3)
                {
                    return ApiResult<string>.FailResult(l["ChangePasswordLocked"]);
                }
            }

            var userdb = await BlazeGateContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userdb == null)
            {
                return ApiResult<string>.FailResult(l["UserNotFound"]);
            }
            if (param.OldPassword == param.NewPassword)
            {
                return ApiResult<string>.FailResult(l["NewPasswordSameAsOld"]);
            }
            if (userdb.Password != param.OldPassword)
            {
                //增加错误次数，并设置1分钟过期时间
                attempts++;
                await distributedCache.SetStringAsync(cacheKey, attempts.ToString(), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                });

                return ApiResult<string>.FailResult(l["OldPasswordIncorrect"]);
            }
            else
            {
                // 修改成功，清除错误计数
                await distributedCache.RemoveAsync(cacheKey);
            }

            userdb.Password = param.NewPassword;
            userdb.UpdateTime = DateTime.Now;
            await BlazeGateContext.SaveChangesAsync();

            return ApiResult<string>.SuccessResult(l["PasswordChanged"]);
        }
    }
}