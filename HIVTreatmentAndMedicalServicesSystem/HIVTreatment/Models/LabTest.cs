namespace HIVTreatment.Models
{
    public class LabTest
    {
        public string LabTestID { get; set; }
        public string RequestID { get; set; }
        public string TreatmentPlantID { get; set; }
        public string TestName { get; set; }
        public string TestCode { get; set; }
        public string TestType { get; set; }
        public string ResultValue { get; set; }
        public int? CD4Initial { get; set; }
        public int? ViralLoadInitial { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}