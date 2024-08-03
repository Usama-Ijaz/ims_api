using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;
using IMS.Services.User;
using IMS.Models;

namespace IMS.Controllers
{
    [Route("api/user")]
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
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserLogin user)
        {
            int userId = await _userService.ValidateUser(user);
            if (userId == -1 || userId == 0)
            {
                return BadRequest(new GenericResponse() { ResponseMessage = "Invalid email" });
            }
            else if (userId == -2) 
            {
                return BadRequest(new GenericResponse() { ResponseMessage = "Invalid password" });
            }
            var token = await _loginService.GenerateJwtToken(userId);
            return Ok(new GenericResponse() { ResponseMessage = "Token generated successfully", ResponseContent = token });
        }
    }
}