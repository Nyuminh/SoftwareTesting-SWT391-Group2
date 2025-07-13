using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext context;
        public PrescriptionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void AddPrescription(Prescription prescription)
        {
            context.Add(prescription);
            context.SaveChanges();
        }

        public List<Prescription> GetAllPrescription()
        {
            return context.Prescription.ToList();
        }

        public Prescription GetLastPrescriptionById()
        {
            return context.Prescription
                .OrderByDescending(p => Convert.ToInt32(p.PrescriptionID.Substring(3)))
                .FirstOrDefault();
        }

        public Prescription GetPrescriptionById(string prescriptionId)
        {
            return context.Prescription
                .FirstOrDefault(p => p.PrescriptionID == prescriptionId);
        }

        public List<Prescription> GetPrescriptionsByPatientForAdmin(string patientId)
        {
            return context.Prescription
                .Join(context.TreatmentPlan,
                      p => p.MedicalRecordID,        
                      t => t.TreatmentPlanID,
                      (p, t) => new { Prescription = p, TreatmentPlan = t })
                .Where(pt => pt.TreatmentPlan.PatientID == patientId)
                .Select(pt => pt.Prescription)
                .ToList();
        }

        public List<Prescription> GetPrescriptionsByPatientForDoctor(string patientId, string doctorId)
        {
            return context.Prescription
                .Join(context.TreatmentPlan,
                      p => p.MedicalRecordID,
                      t => t.TreatmentPlanID,
                      (p, t) => new { Prescription = p, TreatmentPlan = t })
                .Where(pt => pt.TreatmentPlan.PatientID == patientId && pt.Prescription.DoctorID == doctorId)
                .Select(pt => pt.Prescription)
                .ToList();
        }


        public void UpdatePrescription(Prescription prescriptionDto)
        {
            context.Update(prescriptionDto);
            context.SaveChanges();
        }
    }
}
