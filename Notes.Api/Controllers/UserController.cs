using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notes.Api.Resources;
using Notes.Infrastructure.Interfaces;
using Notes.Models.ApplicationUser;

namespace Notes.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IApplicationUser _userService;
        private readonly AppSettings _appSettings;

        public UserController(IApplicationUser userService, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] ApplicationUserUpsertModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.Add(model);
                    return Ok();
                }
                return BadRequest(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult GetToken([FromBody] ApplicationUserGetRequestModel model)
        {
            try {
                var user = _userService.Get(model);
                if (user == null)
                {
                    return BadRequest(new { message = "Incorrect username/email or password." });
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id.ToString())
                };
                var token = new JwtSecurityToken(
                    issuer: "Notes.App",
                    expires: DateTime.Now.AddMonths(3),
                    claims: claims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );
                return Ok(new TokenModel { Token = tokenHandler.WriteToken(token) });
            }
            catch {
                return BadRequest();
            }
        }
    }
}
