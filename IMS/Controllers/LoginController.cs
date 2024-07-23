using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;
using IMS.Services.User;

namespace IMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        
        public LoginController(ILoginService loginService, IUserService userService)
        {
            _loginService = loginService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            int userId = await _userService.ValidateUser(user);
            if (userId <= 0)
            {
                return BadRequest("Invalid Credentials");
            }
            user.UserId = userId;
            var token = await _loginService.GenerateJwtToken(user);
            return Ok(token);
        }
    }
}