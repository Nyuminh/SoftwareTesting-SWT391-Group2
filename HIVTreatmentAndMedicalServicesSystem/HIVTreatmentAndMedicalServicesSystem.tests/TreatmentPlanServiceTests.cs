using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using HIVTreatment.Services;
using Moq;
using Xunit;
using TreatmentPlan = HIVTreatment.Services.TreatmentPlan;

namespace HIVTreatment.Tests
{
    public class TreatmentPlanServiceTests
    {
        private readonly Mock<ITreatmentPlanRepository> _mockRepository;
        private readonly TreatmentPlan _treatmentPlanService;

        public TreatmentPlanServiceTests()
        {
            _mockRepository = new Mock<ITreatmentPlanRepository>();
            _treatmentPlanService = new TreatmentPlan(_mockRepository.Object);
        }

        // Helper method để tạo TreatmentPlanDTO
        private TreatmentPlanDTO CreateTreatmentPlanDTO(
            string patientId = "P001",
            string doctorId = "D001",
            string arvProtocol = "Protocol A",
            int treatmentLine = 1,
            string diagnosis = "HIV Positive",
            string treatmentResult = "Stable")
        {
            return new TreatmentPlanDTO
            {
                PatientID = patientId,
                DoctorID = doctorId,
                ARVProtocol = arvProtocol,
                TreatmentLine = treatmentLine,
                Diagnosis = diagnosis,
                TreatmentResult = treatmentResult
            };
        }

        // Helper method để tạo UpdateTreatmentPlanDTO
        private UpdateTreatmentPlanDTO CreateUpdateTreatmentPlanDTO(
            string treatmentPlanId = "TP000001",
            string patientId = "P001",
            string doctorId = "D001",
            string arvProtocol = "Protocol A",
            int treatmentLine = 1,
            string diagnosis = "HIV Positive",
            string treatmentResult = "Stable")
        {
            return new UpdateTreatmentPlanDTO
            {
                TreatmentPlanID = treatmentPlanId,
                PatientID = patientId,
                DoctorID = doctorId,
                ARVProtocol = arvProtocol,
                TreatmentLine = treatmentLine,
                Diagnosis = diagnosis,
                TreatmentResult = treatmentResult
            };
        }

        #region AddTreatmentPlan Tests

        [Fact]
        public void AddTreatmentPlan_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var treatmentPlanDTO = CreateTreatmentPlanDTO();
            
            // Mock repository để trả về null (không có treatment plan nào trước đó)
            _mockRepository.Setup(repo => repo.GetLastTreatmentPlantId())
                .Returns((HIVTreatment.Models.TreatmentPlan)null);

            // Mock AddTreatmentPlan method
            _mockRepository.Setup(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _treatmentPlanService.AddTreatmentPlan(treatmentPlanDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddTreatmentPlan(It.Is<HIVTreatment.Models.TreatmentPlan>(tp => 
                tp.TreatmentPlanID == "TP000001" &&
                tp.PatientID == "P001" &&
                tp.DoctorID == "D001" &&
                tp.ARVProtocol == "Protocol A" &&
                tp.TreatmentLine == 1 &&
                tp.Diagnosis == "HIV Positive" &&
                tp.TreatmentResult == "Stable"
            )), Times.Once);
        }

        [Fact]
        public void AddTreatmentPlan_WithExistingTreatmentPlans_ShouldGenerateCorrectId()
        {
            // Arrange
            var treatmentPlanDTO = CreateTreatmentPlanDTO();
            
            // Mock repository để trả về treatment plan cuối cùng
            var lastTreatmentPlan = new HIVTreatment.Models.TreatmentPlan
            {
                TreatmentPlanID = "TP000999",
                PatientID = "P999",
                DoctorID = "D999"
            };
            
            _mockRepository.Setup(repo => repo.GetLastTreatmentPlantId())
                .Returns(lastTreatmentPlan);

            // Mock AddTreatmentPlan method
            _mockRepository.Setup(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _treatmentPlanService.AddTreatmentPlan(treatmentPlanDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddTreatmentPlan(It.Is<HIVTreatment.Models.TreatmentPlan>(tp => 
                tp.TreatmentPlanID == "TP001000" // 999 + 1 = 1000, format D6 = 001000
            )), Times.Once);
        }

        [Fact]
        public void AddTreatmentPlan_WithInvalidLastId_ShouldGenerateDefaultId()
        {
            // Arrange
            var treatmentPlanDTO = CreateTreatmentPlanDTO();
            
            // Mock repository để trả về treatment plan với ID không hợp lệ
            var lastTreatmentPlan = new HIVTreatment.Models.TreatmentPlan
            {
                TreatmentPlanID = "TP", // ID quá ngắn
                PatientID = "P999",
                DoctorID = "D999"
            };
            
            _mockRepository.Setup(repo => repo.GetLastTreatmentPlantId())
                .Returns(lastTreatmentPlan);

            // Mock AddTreatmentPlan method
            _mockRepository.Setup(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _treatmentPlanService.AddTreatmentPlan(treatmentPlanDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddTreatmentPlan(It.Is<HIVTreatment.Models.TreatmentPlan>(tp => 
                tp.TreatmentPlanID == "TP000001" // Default ID khi không parse được
            )), Times.Once);
        }

        #endregion

        #region UpdateTreatmentPlan Tests

        [Fact]
        public void UpdateTreatmentPlan_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var updateDTO = CreateUpdateTreatmentPlanDTO(
                treatmentPlanId: "TP000001",
                patientId: "P001",
                doctorId: "D001",
                arvProtocol: "Protocol B",
                treatmentLine: 2,
                diagnosis: "HIV Advanced",
                treatmentResult: "Improving"
            );

            // Mock UpdateTreatmentPlan method
            _mockRepository.Setup(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _treatmentPlanService.UpdateTreatmentPlan(updateDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateTreatmentPlan(It.Is<HIVTreatment.Models.TreatmentPlan>(tp => 
                tp.TreatmentPlanID == "TP000001" &&
                tp.PatientID == "P001" &&
                tp.DoctorID == "D001" &&
                tp.ARVProtocol == "Protocol B" &&
                tp.TreatmentLine == 2 &&
                tp.Diagnosis == "HIV Advanced" &&
                tp.TreatmentResult == "Improving"
            )), Times.Once);
        }

        [Fact]
        public void UpdateTreatmentPlan_WithEmptyStrings_ShouldReturnTrue()
        {
            // Arrange
            var updateDTO = CreateUpdateTreatmentPlanDTO(
                treatmentPlanId: "TP000001",
                patientId: "P001",
                doctorId: "D001",
                arvProtocol: "",
                treatmentLine: 0,
                diagnosis: "",
                treatmentResult: ""
            );

            // Mock UpdateTreatmentPlan method
            _mockRepository.Setup(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _treatmentPlanService.UpdateTreatmentPlan(updateDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateTreatmentPlan(It.Is<HIVTreatment.Models.TreatmentPlan>(tp => 
                tp.TreatmentPlanID == "TP000001" &&
                tp.ARVProtocol == "" &&
                tp.TreatmentLine == 0 &&
                tp.Diagnosis == "" &&
                tp.TreatmentResult == ""
            )), Times.Once);
        }

        [Fact]
        public void UpdateTreatmentPlan_WithSpecialCharacters_ShouldReturnTrue()
        {
            // Arrange
            var updateDTO = CreateUpdateTreatmentPlanDTO(
                treatmentPlanId: "TP-SPECIAL-001",
                patientId: "P-001",
                doctorId: "D-001",
                arvProtocol: "Protocol with spaces & symbols!",
                treatmentLine: 3,
                diagnosis: "HIV+ with complications (Stage 3)",
                treatmentResult: "Stable - monitoring required"
            );

            // Mock UpdateTreatmentPlan method
            _mockRepository.Setup(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _treatmentPlanService.UpdateTreatmentPlan(updateDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateTreatmentPlan(It.Is<HIVTreatment.Models.TreatmentPlan>(tp => 
                tp.TreatmentPlanID == "TP-SPECIAL-001" &&
                tp.PatientID == "P-001" &&
                tp.DoctorID == "D-001" &&
                tp.ARVProtocol == "Protocol with spaces & symbols!" &&
                tp.TreatmentLine == 3 &&
                tp.Diagnosis == "HIV+ with complications (Stage 3)" &&
                tp.TreatmentResult == "Stable - monitoring required"
            )), Times.Once);
        }

        #endregion

        #region GetTreatmentPlanById Tests

        [Fact]
        public void GetTreatmentPlanById_WithValidId_ShouldReturnDTO()
        {
            // Arrange
            string treatmentPlanId = "TP000001";
            var expectedDTO = new UpdateTreatmentPlanDTO
            {
                TreatmentPlanID = "TP000001",
                PatientID = "P001",
                DoctorID = "D001",
                ARVProtocol = "Protocol A",
                TreatmentLine = 1,
                Diagnosis = "HIV Positive",
                TreatmentResult = "Stable"
            };

            _mockRepository.Setup(repo => repo.GetTreatmentPlanById(treatmentPlanId))
                .Returns(expectedDTO);

            // Act
            var result = _treatmentPlanService.GetTreatmentPlanById(treatmentPlanId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDTO.TreatmentPlanID, result.TreatmentPlanID);
            Assert.Equal(expectedDTO.PatientID, result.PatientID);
            Assert.Equal(expectedDTO.DoctorID, result.DoctorID);
            Assert.Equal(expectedDTO.ARVProtocol, result.ARVProtocol);
            Assert.Equal(expectedDTO.TreatmentLine, result.TreatmentLine);
            Assert.Equal(expectedDTO.Diagnosis, result.Diagnosis);
            Assert.Equal(expectedDTO.TreatmentResult, result.TreatmentResult);
        }

        #endregion
    }
} 