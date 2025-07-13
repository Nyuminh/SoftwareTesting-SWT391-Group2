using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext context;
        public DoctorRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(Doctor doctor)
        {
            context.Add(doctor);
            context.SaveChanges();
        }

        public void AddARVProtocol(ARVProtocol protocol)
        {
            context.ARVProtocol.Add(protocol);
            context.SaveChanges();
        }

        public List<ARVProtocolDTO> GetAllARVProtocol()
        {
            var result = (from a in context.ARVProtocol
                          select new ARVProtocolDTO
                          {
                              ARVID = a.ARVID,
                              ARVCode = a.ARVCode,
                              ARVName = a.ARVName,
                              Description = a.Description,
                              AgeRange = a.AgeRange,
                              ForGroup = a.ForGroup

                          }).ToList();
            return result;
        }

        public List<InfoDoctorDTO> GetAllDoctors()
        {
            var result = (from d in context.Doctors
                          join u in context.Users on d.UserId equals u.UserId
                          select new InfoDoctorDTO
                          {
                              DoctorId = d.DoctorId,
                              UserID = u.UserId,
                              
                              Fullname = u.Fullname,
                              Email = u.Email,
                              Specialization = d.Specialization,
                              LicenseNumber = d.LicenseNumber,
                              ExperienceYears = d.ExperienceYears
                          }).ToList();
            return result;
        }

        public ARVProtocolDTO GetARVById(string ARVID)
        {
            var result = (from a in context.ARVProtocol
                          where a.ARVID == ARVID
                          select new ARVProtocolDTO
                          {
                              ARVID = a.ARVID,
                              ARVCode = a.ARVCode,
                              ARVName = a.ARVName,
                              Description = a.Description,
                              AgeRange = a.AgeRange,
                              ForGroup = a.ForGroup
                          }).FirstOrDefault();
            return result;
        }

        public Doctor GetByDoctorId(string doctorId)
        {
            return context.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
        }

        public InfoDoctorDTO GetInfoDoctorById(string DoctorID)
        {
            var result = (from d in context.Doctors
                          join u in context.Users on d.UserId equals u.UserId
                          where d.DoctorId == DoctorID
                          select new InfoDoctorDTO
                          {   DoctorId = d.DoctorId,
                              UserID = u.UserId,
                              Fullname = u.Fullname,
                              Email = u.Email,
                              Specialization = d.Specialization,
                              LicenseNumber = d.LicenseNumber,
                              ExperienceYears = d.ExperienceYears
                          }).FirstOrDefault();
            return result;
        }

        public InfoDoctorDTO GetInfoDoctorByUserId(string UserID)
        {
            var result = (from d in context.Doctors
                          join u in context.Users on d.UserId equals u.UserId
                          where d.UserId == UserID
                          select new InfoDoctorDTO
                          {
                              DoctorId = d.DoctorId,
                              UserID = u.UserId,
                              Fullname = u.Fullname,
                              Email = u.Email,
                              Specialization = d.Specialization,
                              LicenseNumber = d.LicenseNumber,
                              ExperienceYears = d.ExperienceYears
                          }).FirstOrDefault();
            return result;
        }

        public Doctor GetLastDoctorId()
        {
            return context.Doctors.OrderByDescending(d => Convert.ToInt32(d.DoctorId.Substring(3))).FirstOrDefault();
        }

        public List<DoctorScheduleDTO> GetScheduleByDoctorId(string doctorId)
        {
            var result = (from s in context.DoctorWorkSchedules
                          join slot in context.Slot on s.SlotID equals slot.SlotID
                          join d in context.Doctors on s.DoctorID equals d.DoctorId
                          where s.DoctorID == doctorId
                          select new DoctorScheduleDTO
                          {
                              ScheduleID = s.ScheduleID,
                              DateWork = s.DateWork,
                              SlotNumber = slot.SlotNumber,
                              StartTime = slot.StartTime,
                              EndTime = slot.EndTime,
                          }).ToList();

            return result;
        }

        public void Update(Doctor doctor)
        {
            context.Update(doctor);
            context.SaveChanges();
        }

        public void updateARVProtocol(ARVProtocol ARVProtocol)
        {
            context.Update(ARVProtocol);
            context.SaveChanges();
        }

    }
}
