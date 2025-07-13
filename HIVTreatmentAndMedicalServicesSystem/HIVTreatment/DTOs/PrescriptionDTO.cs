namespace HIVTreatment.DTOs
{
    public class PrescriptionDTO
    {
        //public string PrescriptionID { get; set; }
        public string MedicalRecordID { get; set; }
        public string MedicationID { get; set; }
        public string DoctorID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Dosage { get; set; }
        public string LineOfTreatment { get; set; }
    }
}
