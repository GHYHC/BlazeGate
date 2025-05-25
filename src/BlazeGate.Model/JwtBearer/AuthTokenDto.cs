namespace BlazeGate.Model.JwtBearer
{
    public class AuthTokenDto
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
