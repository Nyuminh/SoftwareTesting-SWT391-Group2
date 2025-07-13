using HIVTreatment.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Prescription
{
    public string PrescriptionID { get; set; }

    public string MedicalRecordID { get; set; }

    public string MedicationID { get; set; }
    public string DoctorID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Dosage { get; set; }
    public string LineOfTreatment { get; set; }
    //public string PatientID { get; set; }
    //public string MedicationName { get; set; }

    // Navigation
    //public virtual TreatmentPlan TreatmentPlan { get; set; }
    //public virtual Doctor Doctor { get; set; }
}
