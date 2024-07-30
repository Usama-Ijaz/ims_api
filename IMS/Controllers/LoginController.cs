﻿using Microsoft.AspNetCore.Mvc;
using IMS.Models.User;
using IMS.Services.Login;
using IMS.Services.User;

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
            if (userId <= 0)
            {
                return BadRequest("Invalid Credentials");
            }
            var token = await _loginService.GenerateJwtToken(userId);
            return Ok(token);
        }
    }
}