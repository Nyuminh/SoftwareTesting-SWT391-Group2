namespace HIVTreatment.Models
{
    public class Patient
    {
        public string PatientID { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string BloodType { get; set; }
        public string Allergy { get; set; }
        
    }
}
