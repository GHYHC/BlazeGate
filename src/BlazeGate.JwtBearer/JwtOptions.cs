using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.JwtBearer
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public static readonly double DefaultAccessTokenExpiresMinutes = 30d;
        public static readonly double DefaultRefreshTokenExpiresDays = 7d;

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string PublicKey { get; set; }

        public double AccessTokenExpiresMinutes { get; set; } = DefaultAccessTokenExpiresMinutes;

        public double RefreshTokenExpiresDays { get; set; } = DefaultRefreshTokenExpiresDays;
    }
}