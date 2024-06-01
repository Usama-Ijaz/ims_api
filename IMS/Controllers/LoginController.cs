using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;

namespace IMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            // get user from DB depending on login request
            int userId = 1;
            //add conditions for return
            var token = await _loginService.GenerateJwtToken(userId);
            return Ok(token);
        }
    }
}