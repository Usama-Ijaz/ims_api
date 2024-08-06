using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;
using IMS.Services.User;
using IMS.Services.Email;
using Microsoft.AspNetCore.Authorization;
using IMS.Models;

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
                return BadRequest(new GenericResponse() { ResponseMessage = "User with email already exists" });
            }
            int otpId = await _emailService.SendEmail(userRegister.Email, userId);
            if (otpId <= 0)
            {
                return StatusCode(500, new GenericResponse() { ResponseMessage = "Error sending email" });
            }
            var token = await _loginService.GenerateJwtToken(userId);
            return Ok(new GenericResponse() { ResponseMessage = "Token generated successfully", ResponseContent = token });
        }
        [Authorize]
        [Route("email/send")]
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] string email)
        {
            int otpId = await _emailService.SendEmail(email, 0);
            if (otpId <= 0)
            {
                return StatusCode(500, new GenericResponse() { ResponseMessage = "Error sending email" });
            }
            return Ok(otpId);
        }
        [Authorize]
        [Route("otp/verify")]
        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromBody] string otp)
        {
            int valid = await _userService.VerifyOtp(otp);
            if (valid == -1) return BadRequest(new GenericResponse() { ResponseMessage = "OTP Invalid" });
            else if (valid == -2) return BadRequest(new GenericResponse() { ResponseMessage = "OTP expired" });
            return Ok(new GenericResponse() { ResponseMessage = "OTP Verified" });
        }
        [Authorize]
        [Route("address/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateAddress([FromBody] UserAddress userAddress)
        {
            bool updated = await _userService.UpdateAddress(userAddress);
            if (!updated) return StatusCode(500, new GenericResponse() { ResponseMessage = "Error while updating user address" });
            return Ok(new GenericResponse() { ResponseMessage = "User address updated" });
        }
        [Authorize]
        [Route("image/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateImage([FromBody] UserImage userImage)
        {
            bool updated = await _userService.UpdateImage(userImage);
            if (!updated) return StatusCode(500, new GenericResponse() { ResponseMessage = "Error while updating user image" });
            return Ok(new GenericResponse() { ResponseMessage = "User image updated" });
        }
        [Authorize]
        [Route("preferences/all")]
        [HttpGet]
        public async Task<IActionResult> GetAllPreferences()
        {
            var preferences = await _userService.GetAllPreferences();
            if (preferences == null) return StatusCode(500, new GenericResponse() { ResponseMessage = "Error while fetching all preferences" });
            return Ok(new GenericResponse() { ResponseMessage = "Success", ResponseContent = preferences });
        }
        [Authorize]
        [Route("preferences/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserPreferences([FromBody] List<UpdateUserPreference> userPreferences)
        {
            bool updated = await _userService.UpdateUserPreferences(userPreferences);
            if (!updated) return StatusCode(500, new GenericResponse() { ResponseMessage = "Error while updating user preferences" });
            return Ok(new GenericResponse() { ResponseMessage = "User preferences updated" });
        }
        [Authorize]
        [Route("card-status/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateCardAddedStatus()
        {
            bool updated = await _userService.UpdateCardAddedStatus();
            if (!updated) return StatusCode(500, new GenericResponse() { ResponseMessage = "Error while updating user card status" });
            return Ok(new GenericResponse() { ResponseMessage = "User card status updated" });
        }
    }
}