
using HIVTreatment.Data;
using Microsoft.EntityFrameworkCore;

public class AppointmentRepository : IAppointmentRepository
{ 
    private readonly ApplicationDbContext _context;
    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<BooksAppointment>> GetAllAsync()
    {
        return await _context.BooksAppointments.ToListAsync();
    }
    public async Task<BooksAppointment> GetByIdAsync(string bookId)
    {
        return await _context.BooksAppointments.FindAsync(bookId);
    }
    public async Task<BooksAppointment> CreateAsync(BookAppointmentDTO dto)
    {
        var bookId = "B" + Guid.NewGuid().ToString("N").Substring(0, 9).ToUpper();
        var appointment = new BooksAppointment
        {
            BookID = bookId,
            // Removed PatientID assignment since BookAppointmentDTO does not contain PatientID
            DoctorID = dto.DoctorID,
            BookingType = dto.BookingType,
            BookDate = dto.BookDate,
            Status = "Pending",
            Note = dto.Note
        };
        _context.BooksAppointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }
}


