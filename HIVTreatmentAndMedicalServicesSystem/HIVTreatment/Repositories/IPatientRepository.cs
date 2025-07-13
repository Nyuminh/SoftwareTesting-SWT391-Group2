using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IPatientRepository
    {
        Patient GetByPatientId(string patientID);
        List<PatientDTO> GetAllPatient();

        Patient GetLastPatientId();
        void Add(Patient patient);
        void Update(Patient patient);
        InfoPatientDTO GetInfoPatientById(string patientId);
        InfoPatientByIdDTO GetInfoPatientByIdDTO(string patientId);

    }
}
