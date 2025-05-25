using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace BlazeGate.JwtBearer
{
    public static class JwtBearerExtensions
    {
        /// <summary>
        /// 添加JwtBearer认证
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddBlazeGateJwtBearer(this IHostApplicationBuilder builder, bool balidateAudience = true)
        {
            //添加Swagger的JwtBearer认证
            builder.Services.AddJwtSwaggerGen();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Name));
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.Name).Get<JwtOptions>();

            RsaSecurityKey rsaSecurityPublicKey = new(RsaParametersHelper.PemToRsaParameters(jwtOptions.PublicKey));

            builder.Services.AddScoped<AppJwtBearerEvents>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,

                        //是否验证Audience
                        ValidateAudience = balidateAudience,

                        IssuerSigningKey = rsaSecurityPublicKey,
                        ValidateIssuerSigningKey = true,

                        ClockSkew = TimeSpan.Zero,
                    };

                    options.EventsType = typeof(AppJwtBearerEvents);
                });

            return builder;
        }

        /// <summary>
        /// 添加Token服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddAuthenticationTokenService(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

            return builder;
        }
    }
}