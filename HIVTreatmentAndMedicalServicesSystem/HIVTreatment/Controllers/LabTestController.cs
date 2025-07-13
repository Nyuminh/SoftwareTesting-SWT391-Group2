using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HIVTreatment.Services;
using HIVTreatment.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R002,R003,R004")]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _labTestService;
        private readonly IUserService _userService;

        public LabTestController(ILabTestService labTestService, IUserService userService)
        {
            _labTestService = labTestService;
            _userService = userService;
        }

        [HttpGet("AllLabTests")]
        public IActionResult GetAllLabTests()
        {
            var labTests = _labTestService.GetAllLabTests();
            return Ok(labTests);
        }

        [HttpGet("LabTest/{LabTestID}")]
        [Authorize(Roles = "R001,R002,R003,R004,R005")]
        public IActionResult GetById(string LabTestID)
        {
            var userRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            var labTest = _labTestService.GetLabTestById(LabTestID);
            if (labTest == null)
                return NotFound("Không tìm thấy xét nghiệm.");

            // Bệnh nhân chỉ được xem kq của mình
            if (userRole == "R005")
            {
                var patient = _userService.GetPatientByUserId(userId);
                var patientId = patient?.PatientID;
                if (labTest.PatientID != patientId)
                    return Forbid("Bạn không có quyền xem kết quả này.");
            }

            return Ok(labTest);
        }

        [HttpPost("AddLabTest")]
        [Authorize(Roles = "R001,R002,R003,R004")]
        public IActionResult CreateLabTest([FromBody] CreateLabTestDTO dto)
        {
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            _labTestService.CreateLabTest(dto);
            return Ok("Tạo mới LabTest thành công!");
        }

        [HttpPut("UpdateLabTest/{labTestId}")]
        [Authorize(Roles = "R001,R002,R003,R004")]
        public IActionResult UpdateLabTest(string labTestId, [FromBody] UpdateLabTestDTO dto)
        {
            var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002", "R003", "R004" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền cập nhật LabTest!");
            }

            try
            {
                var updated = _labTestService.UpdateLabTest(labTestId, dto);
                if (!updated)
                {
                    return NotFound("LabTest không tồn tại hoặc cập nhật không thành công.");
                }
                return Ok("Cập nhật LabTest thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteLabTest/{labTestId}")]
        [Authorize(Roles = "R001,R002,R003,R004")]
        public IActionResult DeleteLabTest(string labTestId)
        {
            var deleted = _labTestService.DeleteLabTest(labTestId);
            if (!deleted)
                return NotFound("LabTest này không tồn tại hoặc đã bị xóa.");
            return Ok("Xóa LabTest thành công!");
        }


    }
}
