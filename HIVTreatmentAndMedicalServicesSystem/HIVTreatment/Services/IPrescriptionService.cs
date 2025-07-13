using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface IPrescriptionService
    {
        public bool AddPrescription(PrescriptionDTO prescriptionDto);
        public bool UpdatePrescription(UpdatePrescriptionDTO updatePrescriptionDTO);
        public List<Prescription> GetAllPrescription();
        public Prescription GetPrescriptionById(string prescriptionId);
        List<Prescription> GetPrescriptionsByPatientForDoctor(string patientId, string doctorId);

        // Admin xem tất cả thuốc theo bệnh nhân
        List<Prescription> GetPrescriptionsByPatientForAdmin(string patientId);
    }
}