using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface IProfileService
    {
        bool UpdateProfile(EditProfileUserDTO editProfileUserDTO);

        bool UpdateDoctorProfile(EditprofileDoctorDTO editProfileDoctorDTO);
        List<PatientDTO> GetAllPatient();

        InfoPatientDTO GetInfoPatientById(string patientId);
        InfoPatientByIdDTO GetInfoPatientByIdDTO(string patientId);

    }
}
