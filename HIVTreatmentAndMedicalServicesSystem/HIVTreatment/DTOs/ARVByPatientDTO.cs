namespace HIVTreatment.DTOs
{
    public class ARVByPatientDTO
    {
        public string ARVID { get; set; }
        public string ARVCode { get; set; }
        public string ARVName { get; set; }
        public string Description { get; set; }
        public string AgeRange { get; set; }
        public string ForGroup { get; set; }
        public string TreatmentPlanID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public string ARVProtocol { get; set; }
        public int TreatmentLine { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentResult { get; set; }
        public string FullnameDoctor { get; set; }
        public string FullnamePatient { get; set; }
    }
}
