using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        public PrescriptionService(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        public bool AddPrescription(PrescriptionDTO prescriptionDto)
        {
            var lastPrescription = _prescriptionRepository.GetLastPrescriptionById();
            int nextId = 1;
            if (lastPrescription != null && lastPrescription.PrescriptionID?.Length >= 8)
            {
                string startNumber = lastPrescription.PrescriptionID.Substring(2);
                if (int.TryParse(startNumber, out int parsed))
                {
                    nextId = parsed + 1;
                }
            }
            string newPrescriptionID = "PR" + nextId.ToString("D6");
            var newPrescription = new Prescription
            {
                PrescriptionID = newPrescriptionID,
                MedicalRecordID = prescriptionDto.MedicalRecordID,
                MedicationID = prescriptionDto.MedicationID,
                DoctorID = prescriptionDto.DoctorID,
                StartDate = prescriptionDto.StartDate,
                EndDate = prescriptionDto.EndDate,
                Dosage = prescriptionDto.Dosage,
                LineOfTreatment = prescriptionDto.LineOfTreatment
            };
            _prescriptionRepository.AddPrescription(newPrescription); 
            return true;
        }

        public List<Prescription> GetAllPrescription()
        {
            return _prescriptionRepository.GetAllPrescription();
        }

        public Prescription GetPrescriptionById(string prescriptionId)
        {
            return _prescriptionRepository.GetPrescriptionById(prescriptionId);
        }

        public List<Prescription> GetPrescriptionsByPatientForAdmin(string patientId)
        {
            return _prescriptionRepository.GetPrescriptionsByPatientForAdmin(patientId);
        }

        public List<Prescription> GetPrescriptionsByPatientForDoctor(string patientId, string doctorId)
        {
            return _prescriptionRepository.GetPrescriptionsByPatientForDoctor(patientId, doctorId);
        }


        public bool UpdatePrescription(UpdatePrescriptionDTO updatePrescriptionDTO)
        {
            var prescription = new Prescription
            {
                PrescriptionID = updatePrescriptionDTO.PrescriptionID,
                MedicalRecordID = updatePrescriptionDTO.MedicalRecordID,
                MedicationID = updatePrescriptionDTO.MedicationID,
                DoctorID = updatePrescriptionDTO.DoctorID,
                StartDate = updatePrescriptionDTO.StartDate,
                EndDate = updatePrescriptionDTO.EndDate,
                Dosage = updatePrescriptionDTO.Dosage,
                LineOfTreatment = updatePrescriptionDTO.LineOfTreatment
            };
            _prescriptionRepository.UpdatePrescription(prescription);
            return true;

        }
    }
}
