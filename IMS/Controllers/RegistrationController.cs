using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;
using IMS.Services.User;

namespace IMS.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;

        public RegistrationController(ILoginService loginService, IUserService userService)
        {
            _loginService = loginService;
            _userService = userService;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegister userRegister)
        {
            int userId = await _userService.RegisterUser(userRegister);
            if (userId <= 0)
            {
                return BadRequest("User with email already exists");
            }
            var token = await _loginService.GenerateJwtToken(userId);
            return Ok(token);
        }
    }
}