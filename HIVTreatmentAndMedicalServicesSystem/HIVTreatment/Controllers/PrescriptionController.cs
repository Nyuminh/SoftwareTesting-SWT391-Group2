using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService prescriptionService;
        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            this.prescriptionService = prescriptionService;
        }
        [HttpPost("add-prescription")]
        public IActionResult AddPrescription([FromBody] PrescriptionDTO prescriptionDto)
        {

            if (prescriptionDto == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền kê đơn thuốc!");
            }
            var result = prescriptionService.AddPrescription(prescriptionDto);
            if (result)
            {
                return Ok("Kê đơn thuốc thành công");
            }
            else
            {
                return BadRequest("Kê đơn thuốc không thành công");
            }
        }
        [HttpPut("update-prescription")]
        public IActionResult UpdatePrescription([FromBody] UpdatePrescriptionDTO prescriptionDto)
        {
            if (prescriptionDto == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền cập nhật đơn thuốc!");
            }
            var result = prescriptionService.UpdatePrescription(prescriptionDto);
            if (result)
            {
                return Ok("Cập nhật đơn thuốc thành công");
            }
            else
            {
                return BadRequest("Cập nhật đơn thuốc không thành công");
            }
        }

        [HttpGet("get-all-prescriptions")]
        public IActionResult GetAllPrescriptions()
        {
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return BadRequest("Bạn không có quyền xem đơn thuốc!");
            }
            var prescriptions = prescriptionService.GetAllPrescription();
            return Ok(prescriptions);

        }

        [HttpGet("get-prescription-by-id/{prescriptionId}")]
        public IActionResult GetPrescriptionById(string prescriptionId)
        {
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return BadRequest("Bạn không có quyền xem đơn thuốc!");
            }
            var prescription = prescriptionService.GetPrescriptionById(prescriptionId);
            if (prescription == null)
            {
                return NotFound("Không tìm thấy đơn thuốc với ID đã cho.");
            }
            return Ok(prescription);
        }
        [Authorize(Roles = "R001,R003")]
        [HttpGet("by-patient/{patientId}")]
        public IActionResult GetByPatientId(string patientId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (role == "R001") // Admin
            {
                var prescriptions = prescriptionService.GetPrescriptionsByPatientForAdmin(patientId);

                return Ok(prescriptions);
            }
            else if (role == "R003") // Doctor
            {
                var doctorId = User.FindFirst("DoctorId")?.Value;
                var prescriptions = prescriptionService.GetPrescriptionsByPatientForDoctor(patientId, doctorId);
                return Ok(prescriptions);
            }


            return Forbid();
        }


    }
}


