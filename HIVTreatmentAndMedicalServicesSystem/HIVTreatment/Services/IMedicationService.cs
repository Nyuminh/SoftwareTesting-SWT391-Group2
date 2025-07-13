using HIVTreatment.Models;
using System.Collections.Generic;

namespace HIVTreatment.Services
{
    public interface IMedicationService
    {
        List<Medication> GetAll();
        Medication GetById(string id);
    }
}
