using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface ILabTestService
    {
        List<LabTestDTO> GetAllLabTests();
        LabTestDTO GetLabTestById(string labTestId);
        void CreateLabTest(CreateLabTestDTO dto);
        public bool UpdateLabTest(string labTestId, UpdateLabTestDTO dto);
        bool DeleteLabTest(string labTestId);
    }
}
