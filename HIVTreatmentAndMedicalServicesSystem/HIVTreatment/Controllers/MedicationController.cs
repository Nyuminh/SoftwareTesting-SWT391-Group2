using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIVTreatment.Controllers
{
    [Authorize(Roles = "R001,R002, R003, R004")]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationService _service;

        public MedicationController(IMedicationService service)
        {
            _service = service;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var meds = _service.GetAll();
            return Ok(meds);
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(string id)
        {
            var med = _service.GetById(id);
            if (med == null)
                return NotFound("Không tìm thấy thuốc.");
            return Ok(med);
        }
    }
}
