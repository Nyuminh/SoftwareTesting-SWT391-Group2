using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentPlanController : ControllerBase
    {
        private readonly ITreatmentPlanRepository _repository;
        private readonly ITreatmentPlan treatmentPlan;
        private readonly IUserService userService;

        public TreatmentPlanController(ITreatmentPlanRepository repository, ITreatmentPlanRepository TreatmentRepo, IUserService userService)
        {
            _repository = repository;
            treatmentPlan = new TreatmentPlan(TreatmentRepo);
            this.userService = userService;
        }

        // Admin xem toàn bộ
        [Authorize(Roles = "R001, R004")]
        [HttpGet("admin")]
        public IActionResult GetAll()
        {
            var plans = _repository.GetAll();
            return Ok(plans);
        }

        // Doctor chỉ xem treatment plan của họ
        [Authorize(Roles = "R003")]
        [HttpGet("doctor")]
        public IActionResult GetByDoctor()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Doctor not logged in");

            var plans = _repository.GetByDoctorUserId(userId);
            return Ok(plans);
        }
        //admin xem được treatment plan theo patientId,doctor xem đc của chính bệnh nhân họ điều trị
        [HttpGet("by-patient/{patientId}")]
        [Authorize(Roles = "R001,R003")]
        public IActionResult GetByPatient(string patientId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "R001")  // Admin
            {
                var plans = _repository.GetByPatient(patientId);
                return Ok(plans);
            }
            else if (role == "R003")  // Doctor
            {
                var doctorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var plans = _repository.GetByPatientAndDoctor(patientId, doctorUserId);
                return Ok(plans);
            }

            return Forbid();
        }

        [HttpPost("AddTreatmentPlan")]
        public IActionResult AddTreatmentPlan([FromBody] TreatmentPlanDTO treatmentPlanDTO)
        {
            if (treatmentPlanDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (currentUserId != treatmentPlanDTO.DoctorID && !allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền thêm kế hoạch điều trị cho người khác");
            }
            var result = treatmentPlan.AddTreatmentPlan(treatmentPlanDTO);
            if (result)
            {
                return Ok("Thêm kế hoạch điều trị thành công");
            }
            else
            {
                return BadRequest("Thêm kế hoạch điều trị thất bại");
            }

        }
        [HttpPut("UpdateTreatmentPlan")]
        public IActionResult UpdateTreatmentPlan([FromBody] UpdateTreatmentPlanDTO updateTreatmentPlanDTO)
        {
            if (updateTreatmentPlanDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (currentUserId != updateTreatmentPlanDTO.DoctorID && !allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền cập nhật kế hoạch điều trị cho người khác");
            }
            var result = treatmentPlan.UpdateTreatmentPlan(updateTreatmentPlanDTO);
            if (result)
            {
                return Ok("Cập nhật kế hoạch điều trị thành công");
            }
            else
            {
                return BadRequest("Cập nhật kế hoạch điều trị thất bại");
            }
        }
        [HttpGet("GetTreatmentPlanById/{treatmentPlanId}")]
        public IActionResult GetTreatmentPlanById(string treatmentPlanId)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem kế hoạch điều trị");
            }
            var treatmentPlanData = treatmentPlan.GetTreatmentPlanById(treatmentPlanId);
            if (treatmentPlanData == null)
            {
                return NotFound("Không tìm thấy kế hoạch điều trị với ID đã cho");
            }
            return Ok(treatmentPlanData);

        }

        [HttpGet("GetARVByPatient/{patientId}")]
        public IActionResult GetARVByPatient(string patientId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R005", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin ARV của bệnh nhân");
            }
            var ARVByPatient = userService.GetARVByPatientId(patientId);
            if (ARVByPatient == null)
            {
                return NotFound("Không tìm thấy thông tin ARV cho bệnh nhân với ID đã cho");
            }
            return Ok(ARVByPatient);
        }
        [HttpGet("GetPrescriptionByPatient/{patientId}")]
        public IActionResult GetPrescriptionByPatient(string patientId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R005", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin đơn thuốc của bệnh nhân");
            }
            var prescriptionByPatient = userService.GetPrescriptionByPatientId(patientId);
            if (prescriptionByPatient == null)
            {
                return NotFound("Không tìm thấy thông tin đơn thuốc cho bệnh nhân với ID đã cho");
            }
            return Ok(prescriptionByPatient);
        }

        // Patient xem được các kế hoạch điều trị của chính họ
        [Authorize(Roles = "R005")] // R005 là role của Patient
        [HttpGet("patient")]
        public IActionResult GetByPatientUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Patient not logged in");

            var patient = userService.GetPatientByUserId(userId);
            if (patient == null)
                return NotFound("Không tìm thấy thông tin bệnh nhân");

            var plans = _repository.GetByPatient(patient.PatientID);
            return Ok(plans);
        }

    }
}
