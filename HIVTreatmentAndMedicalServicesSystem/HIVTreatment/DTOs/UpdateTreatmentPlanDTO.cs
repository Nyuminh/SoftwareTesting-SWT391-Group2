namespace HIVTreatment.DTOs
{
    public class UpdateTreatmentPlanDTO
    {
        public string TreatmentPlanID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public string ARVProtocol { get; set; }
        public int TreatmentLine { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentResult { get; set; }
    }
}
