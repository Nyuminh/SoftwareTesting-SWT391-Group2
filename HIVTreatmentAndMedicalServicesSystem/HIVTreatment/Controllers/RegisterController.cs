using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public RegisterController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return BadRequest("Invalid registration data");
            }

            // Validate required fields
            if (string.IsNullOrEmpty(registerDTO.Fullname) ||
                string.IsNullOrEmpty(registerDTO.Password) ||
                string.IsNullOrEmpty(registerDTO.Email))
            {
                return BadRequest("All fields are required");
            }

            // Get default role from configuration or use fallback
            string defaultRole = _configuration["DefaultUserRole"] ?? "R005";

            try
            {
                // Convert DTO to User model
                var user = new User
                {
                    RoleId = defaultRole,
                    Fullname = registerDTO.Fullname,
                    Password = registerDTO.Password,
                    Email = registerDTO.Email,
                    Address = registerDTO.Address,
                    Image = "patient.png"

                    // UserId will be assigned in the service
                };

                var result = _userService.Register(user);
                if (result == null)
                {
                    return BadRequest("Email đã tồn tại hoặc thông tin không hợp lệ");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred during registration");
            }
        }
        [HttpPost("CreateUser")]
        public IActionResult RegisterByAdmin([FromBody] UserDTO UserDTO)
        {

            if (UserDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R002" }; // Admin và Manager

            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền tạo hồ sơ người khác");
            }
            var result = _userService.AddUser(UserDTO);
            if (result == null)
            {
                return BadRequest("Email đã tồn tại hoặc thông tin không hợp lệ");
            }
            return Ok(result);

        }

        [HttpPut("UpdateUser")]
        public IActionResult UpdateUser([FromBody] UpdateUserDTO user)
        {
            if (user == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R002" }; // Admin và Doctor
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền chỉnh sửa hồ sơ người khác");
            }
            var result = _userService.UpdateUser(user);
            if (result == null)
            {
                return BadRequest("Email đã tồn tại hoặc thông tin không hợp lệ");
            }
            return Ok(result);
        }

    }
}
