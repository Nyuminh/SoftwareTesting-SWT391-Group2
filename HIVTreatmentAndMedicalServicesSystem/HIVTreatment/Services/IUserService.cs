using HIVTreatment.Models;
using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IUserService
    {
        UserLoginResponse Login(string email, string password);
        User Register(User user);
        User GetByUserId(string userId);
        bool ResetPassword(string email, string newPassword);

        public List<ARVByPatientDTO> GetARVByPatientId(string patientId);
        public List<PrescriptionByPatient> GetPrescriptionByPatientId(string patientId);
        Patient GetPatientByUserId(string userId);

        List<PrescriptionByPatient> GetPrescriptionsOfPatient(string userId);
        List<User> GetAllUsers();
        User AddUser(UserDTO userDTO);
        bool UpdateUser(UpdateUserDTO userDTO);
        List<StaffDTO> GetAllStaff();
        StaffDTO GetStaffById(string userId);
        User AddStaff(CreateStaffDTO staffDTO);
        bool UpdateStaff(string userId,UpdateStaffDTO staffDTO);

    }
}
