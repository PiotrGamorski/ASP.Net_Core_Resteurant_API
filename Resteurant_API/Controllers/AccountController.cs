using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resteurant_API.Authentication;
using Resteurant_API.Dtos;
using Resteurant_API.Interfaces;
using System.Threading.Tasks;

namespace Resteurant_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountController(IAccountService service, AuthenticationSettings authenticationSettings)
        {
            _service = service;
            _authenticationSettings = authenticationSettings;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            await _service.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDto dto)
        {
            if (_authenticationSettings.AuthenticationScheme == "Bearer")
            {
                var token = await _service.Login_UseJSONWebToken(dto);
                return Ok(token);
            }
            else if (_authenticationSettings.AuthenticationScheme == "Cookies")
            {
                await _service.Login_UseCookies(dto);
                return Ok("You've been successufully logged in.");
            }
            else return BadRequest("Something went wrong...");
        }

        #region Logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<string>> Logout()
        {
            await _service.Logout();
            return Ok("You've been logged out.");
        }
        #endregion
    }
}
