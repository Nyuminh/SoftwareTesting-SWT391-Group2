using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IMedicationRepository
    {
        List<Medication> GetAll();
        Medication GetById(string medicationId);
    }
}
