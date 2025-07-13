using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IDoctorRepository
    {
        Doctor GetByDoctorId(string doctorId);

        Doctor GetLastDoctorId();

        void Add(Doctor doctor);

        void Update(Doctor doctor);
        List<ARVProtocolDTO> GetAllARVProtocol();
        List<InfoDoctorDTO> GetAllDoctors();
        InfoDoctorDTO GetInfoDoctorById(string patientId);

        ARVProtocolDTO GetARVById(string ARVID);

        void updateARVProtocol(ARVProtocol ARVProtocol);
        public List<DoctorScheduleDTO> GetScheduleByDoctorId(string doctorId);
        public InfoDoctorDTO GetInfoDoctorByUserId(string UserID);
        void AddARVProtocol(ARVProtocol protocol);
    }
}
