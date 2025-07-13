using HIVTreatment.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Services
{
    public interface IBookingService
    {
        Task<IActionResult> BookAppointment(BookAppointmentDTO dto, ClaimsPrincipal user);
        Task<IActionResult> CancelAppointmentByPatient(string id, string reason, ClaimsPrincipal user);
        Task<IActionResult> GetMyAppointments(ClaimsPrincipal user);
        Task<IActionResult> PatientCheckIn(string id, ClaimsPrincipal user);
        Task<IActionResult> DoctorCheckout(string id, ClaimsPrincipal user);
        Task<IActionResult> CancelAppointmentByDoctor(string id, string reason, ClaimsPrincipal user);
        Task<IActionResult> GetAppointmentsOfMyPatients(ClaimsPrincipal user);
        Task<IActionResult> GetDoctorAppointments(ClaimsPrincipal user);
        Task<IActionResult> GetAllAppointmentsForStaff();
    }
}
