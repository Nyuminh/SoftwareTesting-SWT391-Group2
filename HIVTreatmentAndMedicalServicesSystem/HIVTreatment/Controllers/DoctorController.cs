using System.Security.Claims;
using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        [HttpPost("AddARVProtocol")]
        [Authorize(Roles = "R001,R002")]
        public IActionResult AddARVProtocol([FromBody] CreateARVProtocolDTO dto)
        {
            var result = _doctorService.AddARVProtocol(dto);
            if (!result)
                return BadRequest("Phác đồ ARV đã tồn tại hoặc dữ liệu không hợp lệ.");
            return Ok("Thêm phác đồ ARV thành công.");
        }
        [HttpGet("AllARVProtocol")]
        public IActionResult GetAllARVRegiemns()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem ARV Protocol!");
            }
            // Fix: Invoke the delegate to get the actual list before calling Any()
            var arvProtocols = _doctorService.GetAllARVProtocol();
            if (arvProtocols == null || !arvProtocols.Any())
            {
                return NotFound("Không có phác đồ ARV nào.");
            }
            return Ok(arvProtocols);
        }

        [HttpGet("AllDoctors")]
        public IActionResult GetAllDoctors()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem danh sách bác sĩ!");
            }
            var doctors = _doctorService.GetAllDoctors();
            if (doctors == null || !doctors.Any())
            {
                return NotFound("Không có bác sĩ nào.");
            }
            return Ok(doctors);
        }

        [HttpGet("InfoDoctor/{doctorId}")]
        public IActionResult GetInfoDoctorById(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin bác sĩ!");
            }
            var doctorInfo = _doctorService.GetInfoDoctorById(doctorId);
            if (doctorInfo == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");
            }
            return Ok(doctorInfo);
        }
        [HttpGet("ARVProtocol/{ARVProtocolID}")]
        public IActionResult GetARVById(string ARVProtocolID)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005"};
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem phác đồ ARV!");
            }
            var arvProtocol = _doctorService.GetARVById(ARVProtocolID);
            if (arvProtocol == null)
            {
                return NotFound("Không tìm thấy phác đồ ARV.");
            }
            return Ok(arvProtocol);
        }

        [HttpPut("UpdateARVProtocol")]
        public IActionResult updateARVRegimen(ARVProtocolDTO ARVProtocolDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền sửa phác đồ ARV!");
            }
            var arvUpdated = _doctorService.updateARVProtocol(ARVProtocolDTO);
            if (!arvUpdated)
            {
                return NotFound("Phác đồ ARV không tồn tại hoặc cập nhật không thành công.");
            }
            return Ok("Cập nhật phác đồ ARV thành công.");
        }

        [HttpGet("DoctorWorkSchedule/{doctorId}")]
        public IActionResult GetScheduleByDoctorId(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem lịch làm việc của bác sĩ!");
            }
            var schedule = _doctorService.GetScheduleByDoctorId(doctorId);
            if (schedule == null || !schedule.Any())
            {
                return NotFound("Không tìm thấy lịch làm việc của bác sĩ.");
            }
            return Ok(schedule);
        }

        [HttpGet("GetDoctorByUserID")]
        public IActionResult GetInfoDoctorByUserId(string userID)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin bác sĩ!");
            }
            var doctorInfo = _doctorService.GetInfoDoctorByUserId(userID);
            if (doctorInfo == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");
            }
            return Ok(doctorInfo);
        }
    }
}
