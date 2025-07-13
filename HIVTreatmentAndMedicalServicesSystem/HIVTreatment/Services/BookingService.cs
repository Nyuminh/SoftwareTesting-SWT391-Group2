using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HIVTreatment.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> BookAppointment(BookAppointmentDTO dto, ClaimsPrincipal user)
        {
           
        
        
        var userId = user.FindFirst("PatientID")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return new UnauthorizedObjectResult("Patient not logged in");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null) return new NotFoundObjectResult("Patient not found");

            var isWorking = await _context.DoctorWorkSchedules.AnyAsync(w =>
                w.DoctorID == dto.DoctorID && w.DateWork.Date == dto.BookDate.Date);
            if (!isWorking) return new BadRequestObjectResult("Bác sĩ không làm việc vào thời gian này.");

            var isConflict = await _context.BooksAppointments.AnyAsync(b =>
      b.DoctorID == dto.DoctorID && b.BookDate.Date == dto.BookDate.Date && b.Status == "Thành công");
            if (isConflict) return new ConflictObjectResult("Bác sĩ đã có lịch hẹn trong ngày này.");

            var appointment = new BooksAppointment
            {
                BookID = "BK" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                PatientID = patient.PatientID,
                DoctorID = dto.DoctorID,
                BookingType = dto.BookingType,
                BookDate = dto.BookDate,
                Status = "Thành công",
                Note = dto.Note
            };

            _context.BooksAppointments.Add(appointment);
            await _context.SaveChangesAsync();

            var fullAppointment = await _context.BooksAppointments
                .Include(b => b.Patient).ThenInclude(p => p.User)
                .Include(b => b.Doctor)
                .FirstOrDefaultAsync(b => b.BookID == appointment.BookID);


            return new OkObjectResult(new
            {
                fullAppointment.BookID,
                fullAppointment.BookingType,
                fullAppointment.BookDate,
                fullAppointment.Status,
                fullAppointment.Note,
                PatientFullname = fullAppointment.Patient.User.Fullname,
            });
        }

        public async Task<IActionResult> CancelAppointmentByPatient(string id, string reason, ClaimsPrincipal user)
        {
            var userId = user.FindFirst("PatientID")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return new UnauthorizedObjectResult("Patient not logged in");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null) return new NotFoundObjectResult("Patient not found");

            var appointment = await _context.BooksAppointments.FirstOrDefaultAsync(a => a.BookID == id && a.PatientID == patient.PatientID);
            if (appointment == null) return new NotFoundObjectResult("Appointment not found");

            if (appointment.Status != "Đã xác nhận" && appointment.Status != "Thành công")
                return new BadRequestObjectResult("Chỉ có thể huỷ lịch đã xác nhận hoặc thành công.");

            appointment.Status = "Đã hủy";
            appointment.Note = reason;
            await _context.SaveChangesAsync();

            return new OkObjectResult("Appointment cancelled by patient.");
        }

        public async Task<IActionResult> GetMyAppointments(ClaimsPrincipal user)
        {
            var userId = user.FindFirst("PatientID")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null) return new NotFoundObjectResult("Patient not found");

            var list = await _context.BooksAppointments
                .Where(a => a.PatientID == patient.PatientID)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate.Date)
.ToListAsync();

            var result = list.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Status,
                a.Note,
                a.Patient
            });

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> PatientCheckIn(string id, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null) return new NotFoundObjectResult("Patient not found");

            var appointment = await _context.BooksAppointments.FirstOrDefaultAsync(a => a.BookID == id && a.PatientID == patient.PatientID);
            if (appointment == null) return new NotFoundObjectResult("Appointment not found");

            if (appointment.Status != "Thành công")
                return new BadRequestObjectResult("Only appointments with status 'Thành công' can be checked in.");

            appointment.Status = "Đã xác nhận";
            await _context.SaveChangesAsync();

            return new OkObjectResult("Patient check-in confirmed.");
        }

        public async Task<IActionResult> DoctorCheckout(string id, ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null) return new UnauthorizedObjectResult("Doctor not found");

            var appointment = await _context.BooksAppointments.FirstOrDefaultAsync(a => a.BookID == id && a.DoctorID == doctor.DoctorId);
            if (appointment == null) return new NotFoundObjectResult("Appointment not found");

            if (appointment.Status != "Đã xác nhận")
                return new BadRequestObjectResult("Patient has not checked in yet.");

            appointment.Status = "Đã khám";
            await _context.SaveChangesAsync();

            return new OkObjectResult("Doctor checkout completed.");
        }

        public async Task<IActionResult> CancelAppointmentByDoctor(string id, string reason, ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null) return new UnauthorizedObjectResult("Doctor not found");

            var appointment = await _context.BooksAppointments.FirstOrDefaultAsync(a => a.BookID == id && a.DoctorID == doctor.DoctorId);
            if (appointment == null) return new NotFoundObjectResult("Appointment not found");

            if (appointment.Status != "Thành công")
                return new BadRequestObjectResult("Only appointments with status 'Thành công' can be cancelled.");

            appointment.Status = "Đã hủy";
            appointment.Note = reason;
            await _context.SaveChangesAsync();

            return new OkObjectResult("Appointment cancelled by doctor.");
        }

        public async Task<IActionResult> GetDoctorAppointments(ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null) return new UnauthorizedObjectResult("Doctor not found");

            var list = await _context.BooksAppointments
                .Where(a => a.DoctorID == doctor.DoctorId && a.Status == "Thành công")
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate.Date).
ToListAsync();

            var result = list.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Patient,
                a.Status
            });

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> GetAppointmentsOfMyPatients(ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null) return new UnauthorizedObjectResult("Doctor not found");

            var appointments = await _context.BooksAppointments
                .Where(a => a.DoctorID == doctor.DoctorId)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate.Date).ToListAsync();

            var result = appointments.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Patient,
                a.Status,
                a.Note
            });

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> GetAllAppointmentsForStaff()
        {
            var list = await _context.BooksAppointments
                .Where(a => a.Status == "Thành công" || a.Status == "Đã hủy")
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate.Date).ToListAsync();

            var result = list.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Patient,
                a.Status
            });

            return new OkObjectResult(result);
        }
    }
}
