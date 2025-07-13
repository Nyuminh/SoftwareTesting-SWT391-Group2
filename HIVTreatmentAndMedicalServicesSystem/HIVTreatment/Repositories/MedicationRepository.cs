using HIVTreatment.Data;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Medication> GetAll()
        {
            return _context.Medication.ToList();
        }

        public Medication GetById(string medicationId)
        {
            return _context.Medication.FirstOrDefault(m => m.MedicationId == medicationId);
        }

    }
}
