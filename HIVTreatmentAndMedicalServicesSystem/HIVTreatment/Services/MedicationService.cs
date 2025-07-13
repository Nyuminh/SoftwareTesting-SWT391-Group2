using HIVTreatment.Models;
using HIVTreatment.Repositories;
using System.Collections.Generic;

namespace HIVTreatment.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository _repository;

        public MedicationService(IMedicationRepository repository)
        {
            _repository = repository;
        }

        public List<Medication> GetAll()
        {
            return _repository.GetAll();
        }

        public Medication GetById(string id)
        {
            return _repository.GetById(id);
        }
    }
}
