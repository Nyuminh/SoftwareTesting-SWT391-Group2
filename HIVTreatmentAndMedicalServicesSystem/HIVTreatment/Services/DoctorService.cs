using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository idoctorRepository;
        public DoctorService(IDoctorRepository doctorRepository)
        {
            this.idoctorRepository = doctorRepository;
        }

        public List<ARVProtocolDTO> GetAllARVProtocol()
        {
            return idoctorRepository.GetAllARVProtocol();
        }

        public List<InfoDoctorDTO> GetAllDoctors()
        {
            return idoctorRepository.GetAllDoctors();
        }

        public ARVProtocolDTO GetARVById(string ARVID)
        {
            return idoctorRepository.GetARVById(ARVID);
        }

        public InfoDoctorDTO GetInfoDoctorById(string doctorId)
        {
            return idoctorRepository.GetInfoDoctorById(doctorId);
        }

        public List<DoctorScheduleDTO> GetScheduleByDoctorId(string doctorId)
        {
            return idoctorRepository.GetScheduleByDoctorId(doctorId);
        }

        public bool AddARVProtocol(CreateARVProtocolDTO dto)
        {
            // Kiểm tra trùng ARVCode hoặc ARVName
            var allProtocols = idoctorRepository.GetAllARVProtocol();
            if (allProtocols.Any(a => a.ARVCode == dto.ARVCode || a.ARVName == dto.ARVName))
                return false;

            // Sinh ARVID mới
            var last = allProtocols.OrderByDescending(a => a.ARVID).FirstOrDefault();
            int nextId = 1;
            if (last != null && int.TryParse(last.ARVID.Substring(2), out int lastId))
                nextId = lastId + 1;
            string newARVID = $"AP{nextId:D6}";

            var protocol = new ARVProtocol
            {
                ARVID = newARVID,
                ARVCode = dto.ARVCode,
                ARVName = dto.ARVName,
                Description = dto.Description,
                AgeRange = dto.AgeRange,
                ForGroup = dto.ForGroup
            };

            idoctorRepository.AddARVProtocol(protocol);
            return true;
        }
        public bool updateARVProtocol(ARVProtocolDTO ARVProtocolDTO)
        {
            var ARV = idoctorRepository.GetARVById(ARVProtocolDTO.ARVID);
            if (ARV == null)
            {
                return false; // ARV regimen not found
            }

            // Map ARVRegimenDTO to ARVRegimen model
            var ARVModel = new ARVProtocol
            {
                ARVID = ARVProtocolDTO.ARVID,
                ARVCode = ARVProtocolDTO.ARVCode,
                ARVName = ARVProtocolDTO.ARVName,
                Description = ARVProtocolDTO.Description,
                AgeRange = ARVProtocolDTO.AgeRange,
                ForGroup = ARVProtocolDTO.ForGroup
            };

            idoctorRepository.updateARVProtocol(ARVModel);
            return true; // Update successful
        }

        public InfoDoctorDTO GetInfoDoctorByUserId(string UserID)
        {
            return idoctorRepository.GetInfoDoctorByUserId(UserID);
        }
    }
}
