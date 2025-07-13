using HIVTreatment.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Booking")]
public class BooksAppointment
{
    [Key]
    [StringLength(10)]
    public string BookID { get; set; }
    public string PatientID { get; set; }
    public string DoctorID { get; set; }
    public string BookingType { get; set; }
    public DateTime BookDate { get; set; }
    public string Status { get; set; }
    public string? Note { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
}