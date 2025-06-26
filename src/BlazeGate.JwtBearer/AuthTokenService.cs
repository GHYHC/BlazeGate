using BlazeGate.Model.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace BlazeGate.JwtBearer
{
    public class AuthTokenService : IAuthTokenService
    {
        private const string RefreshTokenIdClaimType = "refresh_token_id";

        private readonly JwtBearerOptions _jwtBearerOptions;
        private readonly JwtOptions _jwtOptions;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<AuthTokenService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenService(
           IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
           IOptionsSnapshot<JwtOptions> jwtOptions,
           IDistributedCache distributedCache,
           ILogger<AuthTokenService> logger,
           IHttpContextAccessor httpContextAccessor)
        {
            _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _jwtOptions = jwtOptions.Value;
            _distributedCache = distributedCache;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 创建令牌
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user, string audience, SigningCredentials signingCredentials)
        {
            var result = new AuthTokenDto();

            var (refreshTokenId, refreshToken) = await CreateRefreshTokenAsync(user.Id.ToString());
            result.RefreshToken = refreshToken;
            result.AccessToken = CreateAccessToken(user, refreshTokenId, audience, signingCredentials);

            // 将AuthTokent放入Cookie
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(HeaderNames.Authorization, JsonConvert.SerializeObject(result), new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                MaxAge = TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays),
                Path = "/",
                SameSite = SameSiteMode.Lax
            });

            return result;
        }

        /// <summary>
        /// 移除令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<UserDto> RemoveAuthTokenAsync(AuthTokenDto token, SigningCredentials signingCredentials, ClaimsPrincipal principal = null)
        {
            principal = principal ?? GetClaimsPrincipal(token, signingCredentials);

            var user = principal.GetUser();
            var identity = principal.Identities.First();
            var refreshTokenId = identity.Claims.FirstOrDefault(c => c.Type == RefreshTokenIdClaimType).Value;
            var refreshTokenKey = GetRefreshTokenKey(user.Id.ToString(), refreshTokenId);
            var refreshToken = await _distributedCache.GetStringAsync(refreshTokenKey);

            //验证RefreshToken是否有效
            if (refreshToken != token.RefreshToken)
            {
                throw new BadHttpRequestException("Invalid refresh token");
            }

            //清除refresh token
            await _distributedCache.RemoveAsync(refreshTokenKey);

            //删除Cookies
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(HeaderNames.Authorization);

            return user;
        }

        /// <summary>
        /// 获取ClaimsPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <param name="signingCredentials"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public ClaimsPrincipal GetClaimsPrincipal(AuthTokenDto token, SigningCredentials signingCredentials)
        {
            //验证AccessToken有效
            var handler = _jwtBearerOptions.TokenHandlers.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            try
            {
                var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
                // 不校验生命周期
                validationParameters.ValidateLifetime = false;
                validationParameters.IssuerSigningKey = signingCredentials.Key;
                validationParameters.ValidateIssuerSigningKey = true;

                principal = handler.ValidateToken(token.AccessToken, validationParameters, out _);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                throw new BadHttpRequestException("Invalid access token");
            }

            return principal;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="GetUser"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token, string audience, SigningCredentials signingCredentials, Func<UserDto, Task<UserDto>> GetUser)
        {
            var user = await RemoveAuthTokenAsync(token, signingCredentials);

            var newUser = await GetUser(user);
            if (newUser == null)
            {
                throw new BadHttpRequestException("Invalid user");
            }

            return await CreateAuthTokenAsync(newUser, audience, signingCredentials);
        }

        /// <summary>
        /// 创建RefreshToken
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<(string refreshTokenId, string refreshToken)> CreateRefreshTokenAsync(string userId)
        {
            var tokenId = Guid.NewGuid().ToString("N");

            var rnBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rnBytes);
            var token = Convert.ToBase64String(rnBytes);

            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays));

            await _distributedCache.SetStringAsync(GetRefreshTokenKey(userId, tokenId), token, options);

            return (tokenId, token);
        }

        /// <summary>
        /// 创建AccessToken
        /// </summary>
        /// <param name="user"></param>
        /// <param name="refreshTokenId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string CreateAccessToken(UserDto user, string refreshTokenId, string audience, SigningCredentials signingCredentials)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(RefreshTokenIdClaimType, refreshTokenId)
                }),
                Issuer = _jwtOptions.Issuer,
                Audience = string.IsNullOrWhiteSpace(audience) ? _jwtOptions.Audience : audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiresMinutes),
                SigningCredentials = signingCredentials,
            };

            tokenDescriptor.Subject.AddClaims(ClaimsPrincipalExtensions.CreateClaimByUser(user));

            var handler = _jwtBearerOptions.TokenHandlers.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return token;
        }

        /// <summary>
        /// 获取RefreshToken的Key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshTokenId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetRefreshTokenKey(string userId, string refreshTokenId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            return $"AuthToken:{userId}:{refreshTokenId}";
        }
    }

    public interface IAuthTokenService
    {
        /// <summary>
        /// 创建令牌
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user, string audience, SigningCredentials signingCredentials);

        /// <summary>
        /// 移除令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<UserDto> RemoveAuthTokenAsync(AuthTokenDto token, SigningCredentials signingCredentials, ClaimsPrincipal principal = null);

        /// <summary>
        /// 获取ClaimsPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <param name="signingCredentials"></param>
        /// <returns></returns>
        ClaimsPrincipal GetClaimsPrincipal(AuthTokenDto token, SigningCredentials signingCredentials);
    }
}