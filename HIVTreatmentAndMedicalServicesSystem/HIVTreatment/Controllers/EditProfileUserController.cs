using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authentication
    public class EditProfileUserController : ControllerBase
    {
        private readonly IProfileService iProfileService;

        public EditProfileUserController(IProfileService profileService)
        {
            iProfileService = profileService;
        }

        [HttpPut("edit-profile")]
        public IActionResult EditProfile([FromBody] EditProfileUserDTO editProfileUserDTO)
        {
            if (editProfileUserDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" }; // Admin và Doctor

            if (currentUserId != editProfileUserDTO.UserId && !allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền chỉnh sửa hồ sơ người khác");
            }

            var result = iProfileService.UpdateProfile(editProfileUserDTO);
            if (result)
            {
                return Ok("Cập nhật hồ sơ thành công");
            }
            else
            {
                return NotFound("Không tìm thấy người dùng để cập nhật");
            }
        }

        [HttpPut("edit-doctor-profile")]
        public IActionResult EditDoctorProfile([FromBody] EditprofileDoctorDTO editProfileDoctorDTO)
        {
            if (editProfileDoctorDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (currentUserId != editProfileDoctorDTO.UserId && !allowedRoles.Contains(userRole)) // Chỉ cho phép Doctor chỉnh sửa hồ sơ của mình
            {
                return Forbid("Bạn không có quyền chỉnh sửa hồ sơ bác sĩ khác");
            }
            var result = iProfileService.UpdateDoctorProfile(editProfileDoctorDTO);
            if (result)
            {
                return Ok("Cập nhật hồ sơ bác sĩ thành công");
            }
            else
            {
                return NotFound("Không tìm thấy bác sĩ để cập nhật");
            }
        }

        [HttpGet]
        public IActionResult GetAllPatient()
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole)) // Chỉ cho phép Doctor chỉnh sửa hồ sơ của mình
            {
                return Forbid("Bạn không có quyền chỉnh sửa hồ sơ bác sĩ khác");
            }
            var patients = iProfileService.GetAllPatient();
            if (patients == null || !patients.Any())
            {
                return NotFound("Không có bệnh nhân nào.");
            }
            return Ok(patients);

        }

        [HttpGet("{userId}")]
        public IActionResult GetInfoPatientById(string userId)
        {
            var result = iProfileService.GetInfoPatientById(userId);
            if (result == null)
            {
                return NotFound("Không tìm thấy thông tin bệnh nhân");
            }
            return Ok(result);

        }


        // Example of a role-specific endpoint
        [HttpGet("admin-only")]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }

        // Another example for doctor role
        [HttpGet("doctor-only")]
        [Authorize(Policy = "RequireDoctorRole")]
        public IActionResult DoctorOnlyEndpoint()
        {
            return Ok("You are a doctor!");
        }

        [HttpGet("patient/{patientId}")]
        public IActionResult GetPatientById(string patientId)
        {
            try
            {
                // Basic input validation
                if (string.IsNullOrEmpty(patientId))
                {
                    return BadRequest("Mã bệnh nhân không được để trống");
                }

                // Authorization check
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var allowedRoles = new[] { "R001", "R003" }; // Admin and Doctor roles

                if (!allowedRoles.Contains(userRole))
                {
                    return Forbid("Bạn không có quyền truy cập thông tin bệnh nhân");
                }

                var result = iProfileService.GetInfoPatientByIdDTO(patientId);
                if (result == null)
                {
                    return NotFound("Không tìm thấy thông tin bệnh nhân");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logging service
                return StatusCode(500, "Đã xảy ra lỗi khi truy vấn thông tin bệnh nhân");
            }
        }

    }
}
