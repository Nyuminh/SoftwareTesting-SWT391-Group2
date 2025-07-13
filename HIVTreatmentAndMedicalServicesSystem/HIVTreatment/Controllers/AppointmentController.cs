using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public AppointmentController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ===================== PATIENT =====================

        [HttpPost("booking")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
        {
            return await _bookingService.BookAppointment(dto, User);
        }

        [HttpPut("rejected/{id}")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> CancelAppointmentByPatient(string id, [FromBody] string reason)
        {
            return await _bookingService.CancelAppointmentByPatient(id, reason, User);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> GetMyAppointments()
        {
            return await _bookingService.GetMyAppointments(User);
        }

        [HttpPut("PatientCheckin/{id}")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> PatientCheckIn(string id)
        {
            return await _bookingService.PatientCheckIn(id, User);
        }

        // ===================== DOCTOR =====================

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> CancelAppointmentByDoctor(string id, [FromBody] string reason)
        {
            return await _bookingService.CancelAppointmentByDoctor(id, reason, User);
        }

        [HttpPut("DoctorCheckout/{id}")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> DoctorCheckout(string id)
        {
            return await _bookingService.DoctorCheckout(id, User);
        }

        [HttpGet("approved")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            return await _bookingService.GetDoctorAppointments(User);
        }

        [HttpGet("mine-patients")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> GetAppointmentsOfMyPatients()
        {
            return await _bookingService.GetAppointmentsOfMyPatients(User);
        }

        // ===================== STAFF =====================

        [HttpGet("all")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> GetAllAppointmentsForStaff()
        {
            return await _bookingService.GetAllAppointmentsForStaff();
        }
    }
}
