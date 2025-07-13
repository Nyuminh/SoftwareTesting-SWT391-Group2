using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository _labTestRepository;
        private readonly ApplicationDbContext _context;
        public LabTestService(ILabTestRepository labTestRepository, ApplicationDbContext context)
        {
            _labTestRepository = labTestRepository;
            _context = context;
        }

        public List<LabTestDTO> GetAllLabTests()
        {
            var labTests = _labTestRepository.GetAllLabTests();
            var result = new List<LabTestDTO>();
            foreach (var l in labTests)
            {
                var booking = _context.BooksAppointments.FirstOrDefault(b => b.BookID == l.RequestID);
                var patientId = booking?.PatientID;

                result.Add(new LabTestDTO
                {
                    LabTestID = l.LabTestID,
                    RequestID = l.RequestID,
                    TreatmentPlantID = l.TreatmentPlantID,
                    TestName = l.TestName,
                    TestCode = l.TestCode,
                    TestType = l.TestType,
                    ResultValue = l.ResultValue,
                    CD4Initial = l.CD4Initial,
                    ViralLoadInitial = l.ViralLoadInitial,
                    Status = l.Status,
                    Description = l.Description,
                    PatientID = patientId
                });
            }
            return result;
        }

        public LabTestDTO GetLabTestById(string labTestId)
        {
            var labTest = _labTestRepository.GetLabTestById(labTestId);
            if (labTest == null) return null;

            var booking = _context.BooksAppointments.FirstOrDefault(b => b.BookID == labTest.RequestID);
            var patientId = booking?.PatientID;

            return new LabTestDTO
            {
                LabTestID = labTest.LabTestID,
                RequestID = labTest.RequestID,
                TreatmentPlantID = labTest.TreatmentPlantID,
                TestName = labTest.TestName,
                TestCode = labTest.TestCode,
                TestType = labTest.TestType,
                ResultValue = labTest.ResultValue,
                CD4Initial = labTest.CD4Initial,
                ViralLoadInitial = labTest.ViralLoadInitial,
                Status = labTest.Status,
                Description = labTest.Description,
                PatientID = patientId
            };
        }
        public void CreateLabTest(CreateLabTestDTO dto)
        {
            // Tạo LabTestID
            var lastLabTest = _context.LabTests
                .OrderByDescending(l => l.LabTestID)
                .FirstOrDefault();
            int nextId = 1;
            if (lastLabTest != null && int.TryParse(lastLabTest.LabTestID.Substring(2), out int lastId))
            {
                nextId = lastId + 1;
            }
            string newLabTestId = $"LT{nextId:D6}";

            var labTest = new LabTest
            {
                LabTestID = newLabTestId,
                RequestID = dto.RequestID,
                TreatmentPlantID = dto.TreatmentPlantID,
                TestName = dto.TestName,
                TestCode = dto.TestCode,
                TestType = dto.TestType,
                ResultValue = dto.ResultValue,
                CD4Initial = dto.CD4Initial,
                ViralLoadInitial = dto.ViralLoadInitial,
                Status = dto.Status,
                Description = dto.Description
            };

            _labTestRepository.AddLabTest(labTest);
        }

        public bool UpdateLabTest(string labTestId, UpdateLabTestDTO dto)
        {
            // Kiểm tra trùng TestCode (trừ bản đang sửa)
            if (_context.LabTests.Any(l => l.TestCode == dto.TestCode && l.LabTestID != labTestId))
            {
                throw new Exception("TestCode đã tồn tại, vui lòng chọn mã khác.");
            }

            // Lấy bản ghi cần sửa
            var labTest = _labTestRepository.GetLabTestById(labTestId);
            if (labTest == null)
                return false; // Không tìm thấy LabTest để cập nhật

            // Cập nhật các trường (không cập nhật LabTestID)
            labTest.RequestID = dto.RequestID;
            labTest.TreatmentPlantID = dto.TreatmentPlantID;
            labTest.TestName = dto.TestName;
            labTest.TestCode = dto.TestCode;
            labTest.TestType = dto.TestType;
            labTest.ResultValue = dto.ResultValue;
            labTest.CD4Initial = dto.CD4Initial;
            labTest.ViralLoadInitial = dto.ViralLoadInitial;
            labTest.Status = dto.Status;
            labTest.Description = dto.Description;

            // Lưu thay đổi
            _labTestRepository.UpdateLabTest(labTest);
            return true; // Thành công
        }

        public bool DeleteLabTest(string labTestId)
        {
            var labTest = _labTestRepository.GetLabTestById(labTestId);
            if (labTest == null)
                return false;
            _labTestRepository.DeleteLabTest(labTestId);
            return true;
        }
    }
}
