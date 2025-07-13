using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        
        
        // Add logout endpoint
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Đăng xuất thành công");
        }
        //login lqv
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password)) //check password and email
                return BadRequest("Email và mật khẩu không được để trống");
            var result = _userService.Login(loginDTO.Email, loginDTO.Password);

            if (result == null)
                return Unauthorized("Sai email hoặc mật khẩu");
            return Ok(result);
        }

        // GET: api/login/me
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng");

            var user = _userService.GetByUserId(userId);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            return Ok(user);
        }

        [HttpPost("Forget Password")]
        public IActionResult ResetPassword([FromBody] ForgetPasswordDTO forgetPasswordDTO)
        {
            if (string.IsNullOrEmpty(forgetPasswordDTO.Email) || string.IsNullOrEmpty(forgetPasswordDTO.NewPassword))
            {
                return BadRequest("Email và mật khẩu mới không được để trống.");
            }

            var result = _userService.ResetPassword(forgetPasswordDTO.Email, forgetPasswordDTO.NewPassword);
            if (!result)
            {
                return BadRequest("Chỉ bệnh nhân (Patient) mới có quyền đổi mật khẩu hoặc email không tồn tại.");
            }

            return Ok("Đặt lại mật khẩu thành công.");
        }


    }

}
