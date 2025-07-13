namespace HIVTreatment.DTOs
{
    public class PrescriptionByPatient
    {
        public string PrescriptionID { get; set; }

        public string MedicalRecordID { get; set; }
        public string MedicalName { get; set; }
        public string MedicationID { get; set; }
        public string DoctorID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Dosage { get; set; }
        public string LineOfTreatment { get; set; }
        public string TreatmentPlanID { get; set; }
        public string PatientID { get; set; }
        public string ARVProtocol { get; set; }
        public int TreatmentLine { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentResult { get; set; }
        public string FullnameDoctor { get; set; }
        public string FullnamePatient { get; set; }
    }
}
