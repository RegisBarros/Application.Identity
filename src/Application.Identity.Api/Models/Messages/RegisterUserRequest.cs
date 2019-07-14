using System.ComponentModel.DataAnnotations;
using Application.Identity.Api.ValidationAttributes;

namespace Application.Identity.Api.Models.Messages
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Required Field")]
        [EmailAddress(ErrorMessage = "Invalid E-Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [StringLength(100, ErrorMessage = "Password must be between {2} and {1} characters", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; }

        [RoleType(ErrorMessage = "Invalid access type")]
        public string AccessType { get; set; }
    }
}
