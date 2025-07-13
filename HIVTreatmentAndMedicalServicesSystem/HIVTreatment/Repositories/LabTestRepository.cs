using System.Linq;
using HIVTreatment.Data;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly ApplicationDbContext _context;
        public LabTestRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<LabTest> GetAllLabTests()
        {
            return _context.LabTests.ToList();
        }

        public LabTest GetLabTestById(string labTestId)
        {
            return _context.LabTests.FirstOrDefault(l => l.LabTestID == labTestId);
        }
        public void AddLabTest(LabTest labTest)
        {
            _context.LabTests.Add(labTest);
            _context.SaveChanges();
        }

        public void UpdateLabTest(LabTest labTest)
        {
            _context.LabTests.Update(labTest);
            _context.SaveChanges();
        }

        public void DeleteLabTest(string labTestId)
        {
            var labTest = _context.LabTests.FirstOrDefault(l => l.LabTestID == labTestId);
            if (labTest != null)
            {
                _context.LabTests.Remove(labTest);
                _context.SaveChanges();
            }
        }
    }
}