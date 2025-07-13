using HIVTreatment.Controllers;
using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using Xunit;

namespace HIVTreatment.Tests
{
    public class TreatmentPlanControllerTests
    {
        private readonly Mock<ITreatmentPlanRepository> _mockRepository;
        private readonly Mock<ITreatmentPlan> _mockTreatmentPlanService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly TreatmentPlanController _controller;

        public TreatmentPlanControllerTests()
        {
            _mockRepository = new Mock<ITreatmentPlanRepository>();
            _mockUserService = new Mock<IUserService>();
            
            // Tạo service thực tế với mock repository
            var treatmentPlanService = new TreatmentPlan(_mockRepository.Object);
            
            // Sử dụng constructor thực tế của TreatmentPlanController
            _controller = new TreatmentPlanController(
                _mockRepository.Object, 
                _mockRepository.Object, 
                _mockUserService.Object);
        }

        // Helper method để setup user claims
        private void SetupUserClaims(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
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
        public void AddTreatmentPlan_WithValidDataAndAdminRole_ShouldReturnOk()
        {
            // Arrange
            var treatmentPlanDTO = CreateTreatmentPlanDTO();
            SetupUserClaims("admin123", "R001"); // Admin role

            // Mock repository để trả về null (không có treatment plan nào trước đó)
            _mockRepository.Setup(repo => repo.GetLastTreatmentPlantId())
                .Returns((HIVTreatment.Models.TreatmentPlan)null);

            // Mock AddTreatmentPlan method
            _mockRepository.Setup(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _controller.AddTreatmentPlan(treatmentPlanDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thêm kế hoạch điều trị thành công", okResult.Value);
            _mockRepository.Verify(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()), Times.Once);
        }

        [Fact]
        public void AddTreatmentPlan_WithValidDataAndDoctorOwnPlan_ShouldReturnOk()
        {
            // Arrange
            var treatmentPlanDTO = CreateTreatmentPlanDTO(doctorId: "doctor123");
            SetupUserClaims("doctor123", "R003"); // Doctor role, same as DoctorID

            // Mock repository để trả về null (không có treatment plan nào trước đó)
            _mockRepository.Setup(repo => repo.GetLastTreatmentPlantId())
                .Returns((HIVTreatment.Models.TreatmentPlan)null);

            // Mock AddTreatmentPlan method
            _mockRepository.Setup(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _controller.AddTreatmentPlan(treatmentPlanDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thêm kế hoạch điều trị thành công", okResult.Value);
            _mockRepository.Verify(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()), Times.Once);
        }

        [Fact]
        public void AddTreatmentPlan_WithNullData_ShouldReturnBadRequest()
        {
            // Arrange
            SetupUserClaims("admin123", "R001");

            // Act
            var result = _controller.AddTreatmentPlan(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dữ liệu không hợp lệ", badRequestResult.Value);
            _mockRepository.Verify(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()), Times.Never);
        }

       

        [Fact]
        public void AddTreatmentPlan_WithServiceFailure_ShouldReturnBadRequest()
        {
            // Arrange
            var treatmentPlanDTO = CreateTreatmentPlanDTO();
            SetupUserClaims("admin123", "R001");

            // Mock repository để trả về null (không có treatment plan nào trước đó)
            _mockRepository.Setup(repo => repo.GetLastTreatmentPlantId())
                .Returns((HIVTreatment.Models.TreatmentPlan)null);

            // Mock AddTreatmentPlan method để throw exception (simulate failure)
            _mockRepository.Setup(repo => repo.AddTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Throws(new Exception("Database error"));

            // Act
            var result = _controller.AddTreatmentPlan(treatmentPlanDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thêm kế hoạch điều trị thất bại", badRequestResult.Value);
        }


        #endregion

        #region UpdateTreatmentPlan Tests

        [Fact]
        public void UpdateTreatmentPlan_WithValidDataAndAdminRole_ShouldReturnOk()
        {
            // Arrange
            var updateDTO = CreateUpdateTreatmentPlanDTO();
            SetupUserClaims("admin123", "R001"); // Admin role

            // Mock UpdateTreatmentPlan method
            _mockRepository.Setup(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _controller.UpdateTreatmentPlan(updateDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật kế hoạch điều trị thành công", okResult.Value);
            _mockRepository.Verify(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()), Times.Once);
        }

        [Fact]
        public void UpdateTreatmentPlan_WithValidDataAndDoctorOwnPlan_ShouldReturnOk()
        {
            // Arrange
            var updateDTO = CreateUpdateTreatmentPlanDTO(doctorId: "doctor123");
            SetupUserClaims("doctor123", "R003"); // Doctor role, same as DoctorID

            // Mock UpdateTreatmentPlan method
            _mockRepository.Setup(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Verifiable();

            // Act
            var result = _controller.UpdateTreatmentPlan(updateDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật kế hoạch điều trị thành công", okResult.Value);
            _mockRepository.Verify(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()), Times.Once);
        }

        [Fact]
        public void UpdateTreatmentPlan_WithNullData_ShouldReturnBadRequest()
        {
            // Arrange
            SetupUserClaims("admin123", "R001");

            // Act
            var result = _controller.UpdateTreatmentPlan(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dữ liệu không hợp lệ", badRequestResult.Value);
            _mockRepository.Verify(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()), Times.Never);
        }

        

        [Fact]
        public void UpdateTreatmentPlan_WithServiceFailure_ShouldReturnBadRequest()
        {
            // Arrange
            var updateDTO = CreateUpdateTreatmentPlanDTO();
            SetupUserClaims("admin123", "R001");

            // Mock UpdateTreatmentPlan method để throw exception (simulate failure)
            _mockRepository.Setup(repo => repo.UpdateTreatmentPlan(It.IsAny<HIVTreatment.Models.TreatmentPlan>()))
                .Throws(new Exception("Database error"));

            // Act
            var result = _controller.UpdateTreatmentPlan(updateDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật kế hoạch điều trị thất bại", badRequestResult.Value);
        }

        

        #endregion
    }
} 