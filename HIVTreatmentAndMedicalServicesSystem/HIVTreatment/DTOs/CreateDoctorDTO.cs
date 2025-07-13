namespace HIVTreatment.DTOs
{
    public class CreateDoctorDTO
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int ExperienceYears { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
    }
}
