namespace Resteurant_API.Authentication
{
    public class AuthenticationSettings
    {
        public string AuthenticationScheme { get; set; }
        public string JwtKey { get; set; }
        public int JwtExpireDays { get; set; }
        public string JwtIssuer { get; set; }
    }
}
