using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetLastUser()
        {
            return _context.Users
                .OrderByDescending(u => Convert.ToInt32(u.UserId.Substring(3)))
                .FirstOrDefault();
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        

        public User GetByUserId(string UserId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == UserId);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void UpdatePassword(string email, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.Password = newPassword; // Bạn có thể hash password tại đây nếu muốn
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }
        public Doctor GetDoctorByUserId(string userId)
        {
            return _context.Doctors.FirstOrDefault(d => d.UserId == userId);
        }

        public List<ARVByPatientDTO> GetARVByPatientId(string patientId)
        {
            var result = (from tp in _context.TreatmentPlan
                          join pt in _context.Patients on tp.PatientID equals pt.PatientID
                          join doc in _context.Doctors on tp.DoctorID equals doc.DoctorId
                          join arv in _context.ARVProtocol on tp.ARVProtocol equals arv.ARVID
                          join userPatient in _context.Users on pt.UserID equals userPatient.UserId
                          join userDoctor in _context.Users on doc.UserId equals userDoctor.UserId
                          where tp.PatientID == patientId
                          select new ARVByPatientDTO
                          {
                              ARVID = arv.ARVID,
                              ARVCode = arv.ARVCode,
                              ARVName = arv.ARVName,
                              Description = arv.Description,
                              AgeRange = arv.AgeRange,
                              ForGroup = arv.ForGroup,

                              TreatmentPlanID = tp.TreatmentPlanID,
                              PatientID = tp.PatientID,
                              DoctorID = tp.DoctorID,
                              ARVProtocol = tp.ARVProtocol,
                              TreatmentLine = tp.TreatmentLine,
                              Diagnosis = tp.Diagnosis,
                              TreatmentResult = tp.TreatmentResult,

                              FullnameDoctor = userDoctor.Fullname,
                              FullnamePatient = userPatient.Fullname
                          }).ToList();

            return result;
        }

        public List<PrescriptionByPatient> GetPrescriptionByPatientId(string patientId)
        {
            var result = (from p in _context.Prescription
                          join tp in _context.TreatmentPlan on p.MedicalRecordID equals tp.TreatmentPlanID
                          join doc in _context.Doctors on p.DoctorID equals doc.DoctorId
                          join pat in _context.Patients on tp.PatientID equals pat.PatientID
                          join userPatient in _context.Users on pat.UserID equals userPatient.UserId
                          join userDoctor in _context.Users on doc.UserId equals userDoctor.UserId
                          join med in _context.Medication on p.MedicationID equals med.MedicationId
                          where tp.PatientID == patientId
                          select new PrescriptionByPatient
                          {
                              PrescriptionID = p.PrescriptionID,
                              MedicalRecordID = p.MedicalRecordID,
                              MedicalName = med.MedicationName,
                              MedicationID = p.MedicationID,
                              DoctorID = p.DoctorID,
                              StartDate = p.StartDate,
                              EndDate = p.EndDate,
                              Dosage = p.Dosage,
                              LineOfTreatment = p.LineOfTreatment,
                              TreatmentPlanID = tp.TreatmentPlanID,
                              PatientID = tp.PatientID,
                              ARVProtocol = tp.ARVProtocol,
                              TreatmentLine = tp.TreatmentLine,
                              Diagnosis = tp.Diagnosis,
                              TreatmentResult = tp.TreatmentResult,
                              FullnameDoctor = userDoctor.Fullname,
                              FullnamePatient = userPatient.Fullname
                          }).ToList();

            return result;
        }
        public List<PrescriptionByPatient> GetPrescriptionsOfPatient(string userId)
        {
            //var patient = _context.Patients
            //    .Include(p => p.User)
            //    .FirstOrDefault(p => p.UserID == userId);

            //if (patient == null) return new List<PrescriptionByPatient>();

            //var prescriptions = (
            //    from p in _context.Prescription
            //    join d in _context.Doctors on p.DoctorID equals d.DoctorId
            //    join uDoc in _context.Users on d.UserId equals uDoc.UserId
            //    join tp in _context.TreatmentPlan on p.PatientID equals tp.PatientID
            //    join uPat in _context.Users on patient.UserID equals uPat.UserId
            //    where p.PatientID == patient.PatientID
            //    select new PrescriptionByPatient
            //    {
            //        PrescriptionID = p.PrescriptionID,
            //        MedicalRecordID = p.MedicalRecordID,
            //        MedicalName = p.MedicationName,
            //        MedicationID = p.MedicationID,
            //        DoctorID = p.DoctorID,
            //        StartDate = p.StartDate,
            //        EndDate = p.EndDate,
            //        Dosage = p.Dosage,
            //        LineOfTreatment = p.LineOfTreatment,
            //        TreatmentPlanID = tp.TreatmentPlanID,
            //        PatientID = p.PatientID,
            //        ARVProtocol = tp.ARVProtocol,
            //        TreatmentLine = tp.TreatmentLine,
            //        Diagnosis = tp.Diagnosis,
            //        TreatmentResult = tp.TreatmentResult,
            //        FullnameDoctor = uDoc.Fullname,
            //        FullnamePatient = uPat.Fullname
            //    }).ToList();

            //return prescriptions;
            return null;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public List<User> GetUsersByRole(string roleId)
        {
            return _context.Users
                .Where(u => u.RoleId == roleId)
                .ToList();
        }

        public User GetStaffById(string userId)
        {
            return _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == "R004");
        }
    }

}


