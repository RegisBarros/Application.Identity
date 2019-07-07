namespace Application.Identity.Api.Models
{
    public class IdentitySettings
    {
        public string Secret { get; set; }
        public int ExpiresHours { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
