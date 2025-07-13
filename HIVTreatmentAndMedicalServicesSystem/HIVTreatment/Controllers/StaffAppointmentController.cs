using HIVTreatment.Data;
using HIVTreatment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/staff/appointments")]
    public class StaffAppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StaffAppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private User GetCurrentUser()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        private bool IsStaff(User user) => user.RoleId == "R004";

        /// <summary>
        /// Danh sách lịch đã khám
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("completed")]
        public IActionResult GetCompletedAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();

            var appointments = _context.BooksAppointments
                .Where(a => a.Status == "Đã khám")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToList();

            return Ok(appointments);
        }

        /// <summary>
        /// Danh sách lịch đã đặt (Thành công)
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("successful")]
        public IActionResult GetSuccessfulAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();

            var appointments = _context.BooksAppointments
                .Where(a => a.Status == "Thành công")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToList();

            return Ok(appointments);
        }

        /// <summary>
        /// Danh sách lịch đã hủy
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("cancelled")]
        public IActionResult GetCancelledAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();

            var appointments = _context.BooksAppointments
                .Where(a => a.Status == "Đã hủy")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToList();

            return Ok(appointments);
        }

        /// <summary>
        /// Tất cả lịch hẹn: Thành công + Đã hủy + Đã khám
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("all")]
        public IActionResult GetAllAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();

            var appointments = _context.BooksAppointments
                .Where(a => a.Status == "Thành công" || a.Status == "Đã hủy" || a.Status == "Đã khám")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToList();

            return Ok(appointments);
        }
    }
}
