using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Api.Models.Messages
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Required Field")]
        [EmailAddress(ErrorMessage = "Invalid E-Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [StringLength(100, ErrorMessage = "Password must be between {2} and {1} characters", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
