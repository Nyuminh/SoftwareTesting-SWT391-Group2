namespace HIVTreatment.DTOs
{
    public class InfoPatientByIdDTO
    {
        public string PatientID { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string Allergy { get; set; }
    }
}
