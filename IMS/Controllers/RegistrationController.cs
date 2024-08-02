using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;
using IMS.Services.User;
using IMS.Services.Email;
using Microsoft.AspNetCore.Authorization;

namespace IMS.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public RegistrationController(ILoginService loginService, IUserService userService, IEmailService emailService)
        {
            _loginService = loginService;
            _userService = userService;
            _emailService = emailService;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister userRegister)
        {
            int userId = await _userService.RegisterUser(userRegister);
            if (userId <= 0)
            {
                return BadRequest("User with email already exists");
            }
            var token = await _loginService.GenerateJwtToken(userId);
            return Ok(token);
        }
        [Authorize]
        [Route("email/send")]
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] string email)
        {
            int otpId = await _emailService.SendEmail(email);
            if (otpId <= 0)
            {
                return Ok("Error sending email");
            }
            return Ok(otpId);
        }
        [Authorize]
        [Route("otp/verify")]
        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromBody] string otp)
        {
            int valid = await _userService.VerifyOtp(otp);
            if (valid == -1) return BadRequest("OTP Invalid");
            else if (valid == -2) return BadRequest("OTP expired");
            return Ok("OTP Verified");
        }
    }
}