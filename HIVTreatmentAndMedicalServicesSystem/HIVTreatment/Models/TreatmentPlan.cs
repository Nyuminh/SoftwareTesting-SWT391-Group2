using System.ComponentModel.DataAnnotations.Schema;

namespace HIVTreatment.Models
{
   
    public class TreatmentPlan
    {
        public string TreatmentPlanID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public string ARVProtocol { get; set; }
        public int TreatmentLine { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentResult { get; set; }

        public Patient Patient { get; set; }

        public Doctor Doctor { get; set; }


    }
}
