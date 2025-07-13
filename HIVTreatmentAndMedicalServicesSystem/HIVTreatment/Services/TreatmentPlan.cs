using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public class TreatmentPlan : ITreatmentPlan
    {
        private readonly ITreatmentPlanRepository treatmentPlanRepo;

        public TreatmentPlan(ITreatmentPlanRepository treatmentPlanRepository)
        {
            treatmentPlanRepo = treatmentPlanRepository;
        }

        public bool AddTreatmentPlan(TreatmentPlanDTO treatmentPlanDTO)
        {
            var lastTreatmentPlan = treatmentPlanRepo.GetLastTreatmentPlantId();
            int nextId = 1;
            if (lastTreatmentPlan != null && lastTreatmentPlan.TreatmentPlanID?.Length >= 8)
            {
                string startNumber = lastTreatmentPlan.TreatmentPlanID.Substring(2);
                if (int.TryParse(startNumber, out int parsed))
                {
                    nextId = parsed + 1;
                }
            }
            string newTreatmentPlanID = "TP" + nextId.ToString("D6");
            var treatmentPlan = new HIVTreatment.Models.TreatmentPlan // Fully qualify the type to avoid conflict
            {
                TreatmentPlanID = newTreatmentPlanID,
                PatientID = treatmentPlanDTO.PatientID,
                DoctorID = treatmentPlanDTO.DoctorID,
                ARVProtocol = treatmentPlanDTO.ARVProtocol,
                TreatmentLine = treatmentPlanDTO.TreatmentLine,
                Diagnosis = treatmentPlanDTO.Diagnosis,
                TreatmentResult = treatmentPlanDTO.TreatmentResult
            };

            treatmentPlanRepo.AddTreatmentPlan(treatmentPlan);

            return true;
        }

        public UpdateTreatmentPlanDTO GetTreatmentPlanById(string treatmentPlanId)
        {
            return treatmentPlanRepo.GetTreatmentPlanById(treatmentPlanId);
        }

        public bool UpdateTreatmentPlan(UpdateTreatmentPlanDTO updateTreatmentPlanDTO)
        {
            var updatedTreatmentPlan = new HIVTreatment.Models.TreatmentPlan
            {
                TreatmentPlanID = updateTreatmentPlanDTO.TreatmentPlanID,
                PatientID = updateTreatmentPlanDTO.PatientID,
                DoctorID = updateTreatmentPlanDTO.DoctorID,
                ARVProtocol = updateTreatmentPlanDTO.ARVProtocol,
                TreatmentLine = updateTreatmentPlanDTO.TreatmentLine,
                Diagnosis = updateTreatmentPlanDTO.Diagnosis,
                TreatmentResult = updateTreatmentPlanDTO.TreatmentResult
            };
            treatmentPlanRepo.UpdateTreatmentPlan(updatedTreatmentPlan);
            return true;

        }
    }
}
