namespace Application.Identity.Api.Settings
{
    public class IdentitySettings
    {
        public string Secret { get; set; }
        public int ExpiresHours { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SuperUser SuperUser { get; set; }
    }

    public class SuperUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }          
    }
}
