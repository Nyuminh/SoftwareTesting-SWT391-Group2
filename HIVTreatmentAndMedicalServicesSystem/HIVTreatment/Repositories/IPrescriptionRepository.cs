using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IPrescriptionRepository
    {
        public void AddPrescription(Prescription prescription);
        public Prescription GetLastPrescriptionById();
        public void UpdatePrescription(Prescription prescription);
        public List<Prescription> GetAllPrescription();
        public Prescription GetPrescriptionById(string prescriptionId);
        List<Prescription> GetPrescriptionsByPatientForDoctor(string patientId, string doctorId);
        List<Prescription> GetPrescriptionsByPatientForAdmin(string patientId);

    }
}
