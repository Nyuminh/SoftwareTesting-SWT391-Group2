using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IDoctorService
    {
        List<ARVProtocolDTO> GetAllARVProtocol();
        public List<InfoDoctorDTO> GetAllDoctors();

        InfoDoctorDTO GetInfoDoctorById(string doctorId);
        public InfoDoctorDTO GetInfoDoctorByUserId(string UserID);
        ARVProtocolDTO GetARVById(string ARVID);
        bool updateARVProtocol(ARVProtocolDTO ARVProtocolDTO);
        public List<DoctorScheduleDTO> GetScheduleByDoctorId(string doctorId);
        bool AddARVProtocol(CreateARVProtocolDTO dto);
    }
}
