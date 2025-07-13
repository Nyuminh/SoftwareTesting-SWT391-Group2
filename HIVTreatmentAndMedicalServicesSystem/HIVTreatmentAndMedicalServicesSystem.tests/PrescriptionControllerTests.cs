using HIVTreatment.Controllers;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace HIVTreatment.Tests
{
    public class PrescriptionControllerTests
    {
        private readonly Mock<IPrescriptionService> _mockService;
        private readonly PrescriptionController _controller;

        public PrescriptionControllerTests()
        {
            _mockService = new Mock<IPrescriptionService>();
            _controller = new PrescriptionController(_mockService.Object);
        }

        #region AddPrescription
        [Fact]
        public void AddPrescription_WithValidDataAndAllowedRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new PrescriptionDTO { MedicalRecordID = "TP001" };
            _mockService.Setup(s => s.AddPrescription(dto)).Returns(true);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.AddPrescription(dto);
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Kê đơn thuốc thành công", ok.Value);
            _mockService.Verify(s => s.AddPrescription(dto), Times.Once);
        }

        [Fact]
        public void AddPrescription_WithValidDataAndDoctorRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new PrescriptionDTO { MedicalRecordID = "TP002" };
            _mockService.Setup(s => s.AddPrescription(dto)).Returns(true);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user2"),
                new Claim(ClaimTypes.Role, "R003")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.AddPrescription(dto);
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Kê đơn thuốc thành công", ok.Value);
        }

     
        [Fact]
        public void AddPrescription_WithUnauthorizedRole_ShouldReturnForbid()
        {
            // Arrange
            var dto = new PrescriptionDTO { MedicalRecordID = "TP003" };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user3"),
                new Claim(ClaimTypes.Role, "R005")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.AddPrescription(dto);
            // Assert
            Assert.IsType<ForbidResult>(result);
            _mockService.Verify(s => s.AddPrescription(It.IsAny<PrescriptionDTO>()), Times.Never);
        }

        [Fact]
        public void AddPrescription_WhenServiceReturnsFalse_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new PrescriptionDTO { MedicalRecordID = "TP004" };
            _mockService.Setup(s => s.AddPrescription(dto)).Returns(false);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user4"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.AddPrescription(dto);
            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Kê đơn thuốc không thành công", badRequest.Value);
        }
        #endregion

        #region UpdatePrescription
        [Fact]
        public void UpdatePrescription_WithValidDataAndAllowedRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new UpdatePrescriptionDTO { PrescriptionID = "PR001" };
            _mockService.Setup(s => s.UpdatePrescription(dto)).Returns(true);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.UpdatePrescription(dto);
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật đơn thuốc thành công", ok.Value);
            _mockService.Verify(s => s.UpdatePrescription(dto), Times.Once);
        }

     

        [Fact]
        public void UpdatePrescription_WithUnauthorizedRole_ShouldReturnForbid()
        {
            // Arrange
            var dto = new UpdatePrescriptionDTO { PrescriptionID = "PR002" };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user2"),
                new Claim(ClaimTypes.Role, "R005")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.UpdatePrescription(dto);
            // Assert
            Assert.IsType<ForbidResult>(result);
            _mockService.Verify(s => s.UpdatePrescription(It.IsAny<UpdatePrescriptionDTO>()), Times.Never);
        }

        [Fact]
        public void UpdatePrescription_WhenServiceReturnsFalse_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new UpdatePrescriptionDTO { PrescriptionID = "PR003" };
            _mockService.Setup(s => s.UpdatePrescription(dto)).Returns(false);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user3"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.UpdatePrescription(dto);
            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật đơn thuốc không thành công", badRequest.Value);
        }
        #endregion

        #region GetAllPrescription
        [Fact]
        public void GetAllPrescriptions_WithAllowedRole_ShouldReturnOkWithList()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionID = "PR001" },
                new Prescription { PrescriptionID = "PR002" }
            };
            _mockService.Setup(s => s.GetAllPrescription()).Returns(prescriptions);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.GetAllPrescriptions();
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Prescription>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void GetAllPrescriptions_WithUnauthorizedRole_ShouldReturnBadRequest()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user2"),
                new Claim(ClaimTypes.Role, "R005")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            // Act
            var result = _controller.GetAllPrescriptions();
            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bạn không có quyền xem đơn thuốc!", badRequest.Value);
        }
        #endregion
    }
} 