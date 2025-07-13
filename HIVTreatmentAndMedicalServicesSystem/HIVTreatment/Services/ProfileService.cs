using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository iUserRepository;
        private readonly IPatientRepository iPatientRepository;
        private readonly IDoctorRepository iDoctorRepository;

        public ProfileService(IUserRepository userRepository, IPatientRepository patientRepository, IDoctorRepository iDoctorRepository)
        {
            iUserRepository = userRepository;
            iPatientRepository = patientRepository;
            this.iDoctorRepository = iDoctorRepository;
        }

        public List<PatientDTO> GetAllPatient()
        {
            return iPatientRepository.GetAllPatient();
        }


        public InfoPatientDTO GetInfoPatientById(string patientId)
        {
            return iPatientRepository.GetInfoPatientById(patientId);
        }

        public InfoPatientByIdDTO GetInfoPatientByIdDTO(string patientId)
        {
            return iPatientRepository.GetInfoPatientByIdDTO(patientId);
        }

        public bool UpdateDoctorProfile(EditprofileDoctorDTO editProfileDoctorDTO)
        {
            var user = iUserRepository.GetByUserId(editProfileDoctorDTO.UserId);
            if (user == null)
            {
                return false; // User not found
            }
            var doctor = iDoctorRepository.GetByDoctorId(editProfileDoctorDTO.UserId);
            user.Fullname = editProfileDoctorDTO.Fullname;
            iUserRepository.Update(user);
            if (doctor == null)
            {
                var lastDoctor = iDoctorRepository.GetLastDoctorId();
                int nextId = 1;
                if (lastDoctor != null && lastDoctor.DoctorId?.Length >= 8)
                {
                    string numberPart = lastDoctor.DoctorId.Substring(2);
                    if (int.TryParse(numberPart, out int parsed))
                    {
                        nextId = parsed + 1;
                    }
                }
                string newDoctorID = "DT" + nextId.ToString("D6");
                doctor = new Doctor
                {
                    DoctorId = newDoctorID,
                    UserId = editProfileDoctorDTO.UserId,
                    Specialization = editProfileDoctorDTO.Specialization,
                    LicenseNumber = editProfileDoctorDTO.LicenseNumber,
                    ExperienceYears = editProfileDoctorDTO.ExperienceYears,
                };
                // Use Add instead of Update for new entities
                iDoctorRepository.Add(doctor);
            }
            else
            {
                // Update existing doctor data
                doctor.Specialization = editProfileDoctorDTO.Specialization;
                doctor.LicenseNumber = editProfileDoctorDTO.LicenseNumber;
                doctor.ExperienceYears = editProfileDoctorDTO.ExperienceYears;
                iDoctorRepository.Update(doctor);
            }
            return true;
        }

        public bool UpdateProfile(EditProfileUserDTO editProfileUserDTO)
        {
            var user = iUserRepository.GetByUserId(editProfileUserDTO.UserId);
            if (user == null)
            {
                return false; // User not found
            }
            
            user.Fullname = editProfileUserDTO.Fullname;
            user.Address = editProfileUserDTO.Address;
            user.Image = "patient.png"; // Optional, can be null
            iUserRepository.Update(user);

            var patient = iPatientRepository.GetByPatientId(editProfileUserDTO.UserId);
            if (patient == null)
            {
                var lastPatient = iPatientRepository.GetLastPatientId();
                int nextId = 1;
                if (lastPatient != null && lastPatient.PatientID?.Length >= 8)
                {
                    string numberPart = lastPatient.PatientID.Substring(2);
                    if (int.TryParse(numberPart, out int parsed))
                    {
                        nextId = parsed + 1;
                    }
                }
                string newPatientID = "PT" + nextId.ToString("D6");
                patient = new Patient
                {
                    PatientID = newPatientID,
                    UserID = editProfileUserDTO.UserId,
                    DateOfBirth = editProfileUserDTO.DateOfBirth,
                    Gender = editProfileUserDTO.Gender,
                    Phone = editProfileUserDTO.Phone,
                    BloodType = editProfileUserDTO.BloodType,
                    Allergy = editProfileUserDTO.Allergy,
                };
                // Use Add instead of Update for new entities
                iPatientRepository.Add(patient);
            }
            else
            {
                // Update existing patient data
                patient.DateOfBirth = editProfileUserDTO.DateOfBirth;
                patient.Gender = editProfileUserDTO.Gender;
                patient.Phone = editProfileUserDTO.Phone;
                patient.BloodType = editProfileUserDTO.BloodType;
                patient.Allergy = editProfileUserDTO.Allergy;
                iPatientRepository.Update(patient);
            }
            return true;
        }

    }
}