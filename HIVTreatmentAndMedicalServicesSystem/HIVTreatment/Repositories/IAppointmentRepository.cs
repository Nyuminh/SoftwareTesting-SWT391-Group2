
    public interface IAppointmentRepository 
{
    Task<IEnumerable<BooksAppointment>> GetAllAsync();
    Task<BooksAppointment> GetByIdAsync(string bookId);
    Task<BooksAppointment> CreateAsync(BookAppointmentDTO dto);
}

