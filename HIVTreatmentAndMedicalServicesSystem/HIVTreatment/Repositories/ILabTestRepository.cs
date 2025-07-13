using System.Collections.Generic;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface ILabTestRepository
    {
        List<LabTest> GetAllLabTests();
        LabTest GetLabTestById(string labTestId);
        void AddLabTest(LabTest labTest);
        void UpdateLabTest(LabTest labTest);
        void DeleteLabTest(string labTestId);
    }
}