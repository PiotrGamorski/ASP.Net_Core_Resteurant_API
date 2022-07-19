using Microsoft.AspNetCore.Mvc;
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

        public AccountController(IAccountService service)
        {
            _service = service;
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
            var token = await _service.Login(dto);
            return Ok(token);
        }
    }
}
