using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Identity.Api.Models.Messages;
using Application.Identity.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Identity.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentitySettings _identitySettings;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<IdentitySettings> identitySettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identitySettings = identitySettings.Value;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody]RegisterUserRequest registerUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true                
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _signInManager.SignInAsync(user, false);

            return Ok(await CreateJwt(registerUser.Email));
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (!result.Succeeded)
                return BadRequest("Invalid User");

            return Ok(await CreateJwt(loginUser.Email));
        }

        private async Task<string> CreateJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_identitySettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _identitySettings.Issuer,
                Audience = _identitySettings.Audience,
                Expires = DateTime.UtcNow.AddHours(_identitySettings.ExpiresHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}