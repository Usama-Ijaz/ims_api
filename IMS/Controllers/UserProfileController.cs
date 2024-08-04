using IMS.Models.User;
using IMS.Models;
using IMS.Services.Login;
using IMS.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IMS.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [Route("profile")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userModel = await _userService.GetUserProfile();
            if (userModel is null || userModel.UserId <= 0)
            {
                return StatusCode(500, new GenericResponse() { ResponseMessage = "Error fetching user profile" });
            }
            return Ok(new GenericResponse() { ResponseContent = userModel });
        }
        [Authorize]
        [Route("profile-status")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfileStatus()
        {
            string profileStatus = await _userService.GetUserProfileStatus();
            if (string.IsNullOrWhiteSpace(profileStatus))
            {
                return StatusCode(500, new GenericResponse() { ResponseMessage = "Error fetching user profile status" });
            }
            return Ok(new GenericResponse() { ResponseContent = profileStatus });
        }
    }
}
