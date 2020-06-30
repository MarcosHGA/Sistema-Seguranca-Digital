using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SSDDemo.Models;

namespace SSDDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("nova-conta")]
        public async Task<IActionResult> Register(RegisterUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(s => s.Errors));

            IdentityUser identityUser = new IdentityUser
            {
                UserName = viewModel.Email,
                Email = viewModel.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, viewModel.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _signInManager.SignInAsync(identityUser, false);

            return Ok(await GetFullJwt(viewModel.Email));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(s => s.Errors));

            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, true);

            if (result.Succeeded)
                return Ok(await GetFullJwt(viewModel.Email));

            return BadRequest("Usuário ou senha inválidos");
        }

        private async Task<string> GetFullJwt(string email)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(email);
            
            ClaimsIdentity identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(await _userManager.GetClaimsAsync(user));

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] encriptedKey = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(encriptedKey), SecurityAlgorithms.HmacSha256Signature),
                Subject = identityClaims
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
