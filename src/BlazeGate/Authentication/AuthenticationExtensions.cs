using BlazeGate.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace BlazeGate.Authentication
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// 添加认证(JwtBearer)
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddAuthentication(this IHostApplicationBuilder builder, bool balidateAudience = true)
        {
            //添加Swagger的JwtBearer认证
            builder.Services.AddJwtSwaggerGen();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Name));
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.Name).Get<JwtOptions>();

            builder.Services.AddScoped<CustomJwtBearerEvents>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,

                        //是否验证Audience
                        ValidateAudience = balidateAudience,

                        //IssuerSigningKey = rsaSecurityPublicKey,
                        ValidateIssuerSigningKey = true,

                        ClockSkew = TimeSpan.Zero,
                    };

                    options.EventsType = typeof(CustomJwtBearerEvents);
                });

            return builder;
        }
    }
}