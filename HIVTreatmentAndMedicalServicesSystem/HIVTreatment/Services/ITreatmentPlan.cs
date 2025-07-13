using HIVTreatment.DTOs;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Services
{
    public interface ITreatmentPlan
    {
        public bool AddTreatmentPlan(TreatmentPlanDTO treatmentPlanDTO);
        public bool UpdateTreatmentPlan(UpdateTreatmentPlanDTO updateTreatmentPlanDTO);
        public UpdateTreatmentPlanDTO GetTreatmentPlanById(string treatmentPlanId);
    }
}
