using HIVTreatment.Controllers;
using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace HIVTreatment.Tests
{
    public class DoctorControllerTests
    {
        private readonly Mock<IDoctorService> _mockService;
        private readonly DoctorController _controller;

        public DoctorControllerTests()
        {
            _mockService = new Mock<IDoctorService>();
            _controller = new DoctorController(_mockService.Object);
        }

        #region GetAllARVProtocol Tests

        [Fact]
        public void GetAllARVRegiemns_WithValidRole_ShouldReturnOk()
        {
            // Arrange
            var expectedProtocols = new List<ARVProtocolDTO>
            {
                new ARVProtocolDTO
                {
                    ARVID = "AP000001",
                    ARVCode = "CODE001",
                    ARVName = "Protocol A",
                    Description = "First line treatment",
                    AgeRange = "18-65",
                    ForGroup = "Adults"
                },
                new ARVProtocolDTO
                {
                    ARVID = "AP000002",
                    ARVCode = "CODE002",
                    ARVName = "Protocol B",
                    Description = "Second line treatment",
                    AgeRange = "0-18",
                    ForGroup = "Children"
                }
            };

            _mockService.Setup(s => s.GetAllARVProtocol()).Returns(expectedProtocols);

            // Setup user claims for Admin role (R001)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.GetAllARVRegiemns();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProtocols = Assert.IsType<List<ARVProtocolDTO>>(okResult.Value);
            Assert.Equal(2, returnedProtocols.Count);
            Assert.Equal("AP000001", returnedProtocols[0].ARVID);
            Assert.Equal("CODE001", returnedProtocols[0].ARVCode);
        }

        [Fact]
        public void GetAllARVRegiemns_WithDoctorRole_ShouldReturnOk()
        {
            // Arrange
            var expectedProtocols = new List<ARVProtocolDTO>
            {
                new ARVProtocolDTO
                {
                    ARVID = "AP000001",
                    ARVCode = "CODE001",
                    ARVName = "Protocol A",
                    Description = "First line treatment",
                    AgeRange = "18-65",
                    ForGroup = "Adults"
                }
            };

            _mockService.Setup(s => s.GetAllARVProtocol()).Returns(expectedProtocols);

            // Setup user claims for Doctor role (R003)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123"),
                new Claim(ClaimTypes.Role, "R003")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.GetAllARVRegiemns();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProtocols = Assert.IsType<List<ARVProtocolDTO>>(okResult.Value);
            Assert.Single(returnedProtocols);
        }

        [Fact]
        public void GetAllARVRegiemns_WithUnauthorizedRole_ShouldReturnForbid()
        {
            // Arrange
            // Setup user claims for Patient role (R005) - not allowed
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "patient123"),
                new Claim(ClaimTypes.Role, "R005")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.GetAllARVRegiemns();

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void GetAllARVRegiemns_WithEmptyList_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllARVProtocol()).Returns(new List<ARVProtocolDTO>());

            // Setup user claims for Admin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.GetAllARVRegiemns();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không có phác đồ ARV nào.", notFoundResult.Value);
        }

        [Fact]
        public void GetAllARVRegiemns_WithNullData_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllARVProtocol()).Returns((List<ARVProtocolDTO>)null);

            // Setup user claims for Admin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.GetAllARVRegiemns();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không có phác đồ ARV nào.", notFoundResult.Value);
        }

        [Fact]
        public void GetAllARVRegiemns_WithNoUserClaims_ShouldReturnForbid()
        {
            // Arrange
            // No user claims set up

            // Act
            var result = _controller.GetAllARVRegiemns();

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        #endregion

        #region AddARVProtocol Tests

        [Fact]
        public void AddARVProtocol_WithValidDataAndAdminRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = "CODE001",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            _mockService.Setup(s => s.AddARVProtocol(dto)).Returns(true);

            // Setup user claims for Admin role (R001)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.AddARVProtocol(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thêm phác đồ ARV thành công.", okResult.Value);
            _mockService.Verify(s => s.AddARVProtocol(dto), Times.Once);
        }

        [Fact]
        public void AddARVProtocol_WithValidDataAndManagerRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = "CODE002",
                ARVName = "Protocol B",
                Description = "Second line treatment",
                AgeRange = "0-18",
                ForGroup = "Children"
            };

            _mockService.Setup(s => s.AddARVProtocol(dto)).Returns(true);

            // Setup user claims for Manager role (R002)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "manager123"),
                new Claim(ClaimTypes.Role, "R002")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.AddARVProtocol(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thêm phác đồ ARV thành công.", okResult.Value);
            _mockService.Verify(s => s.AddARVProtocol(dto), Times.Once);
        }


        [Theory]
        [InlineData("R003")] // Doctor
        [InlineData("R004")] // Staff
        [InlineData("R005")] // Patient
        public void AddARVProtocol_WithUnauthorizedRole_ShouldReturnForbid(string role)
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = "CODE001",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            // Setup user claims for unauthorized role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, role)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.AddARVProtocol(dto);

            // Assert
            Assert.IsType<ForbidResult>(result);
            _mockService.Verify(s => s.AddARVProtocol(It.IsAny<CreateARVProtocolDTO>()), Times.Never);
        }

  
      
        [Theory]
        [InlineData("R001")] // Admin
        [InlineData("R002")] // Manager
        public void AddARVProtocol_WithAuthorizedRoles_ShouldAllowAccess(string role)
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = "CODE001",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            _mockService.Setup(s => s.AddARVProtocol(dto)).Returns(true);

            // Setup user claims for the specified role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, role)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.AddARVProtocol(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thêm phác đồ ARV thành công.", okResult.Value);
        }

        [Theory]
        [InlineData("CODE001", "Protocol A", "First line", "18-65", "Adults")]
        [InlineData("CODE002", "Protocol B", "Second line", "0-18", "Children")]
        [InlineData("CODE003", "Protocol C", "Third line", "65+", "Elderly")]
        public void AddARVProtocol_WithVariousData_ShouldAddCorrectly(
            string arvCode, string arvName, string description, string ageRange, string forGroup)
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = arvCode,
                ARVName = arvName,
                Description = description,
                AgeRange = ageRange,
                ForGroup = forGroup
            };

            _mockService.Setup(s => s.AddARVProtocol(dto)).Returns(true);

            // Setup user claims for Admin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.AddARVProtocol(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thêm phác đồ ARV thành công.", okResult.Value);
            _mockService.Verify(s => s.AddARVProtocol(dto), Times.Once);
        }

        #endregion

        #region updateARVProtocol Tests

        [Fact]
        public void updateARVRegimen_WithValidDataAndAdminRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE002",
                ARVName = "Updated Protocol A",
                Description = "Updated first line treatment",
                AgeRange = "20-70",
                ForGroup = "Updated Adults"
            };

            _mockService.Setup(s => s.updateARVProtocol(dto)).Returns(true);

            // Setup user claims for Admin role (R001)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.updateARVRegimen(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật phác đồ ARV thành công.", okResult.Value);
            _mockService.Verify(s => s.updateARVProtocol(dto), Times.Once);
        }

        [Fact]
        public void updateARVRegimen_WithValidDataAndDoctorRole_ShouldReturnOk()
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE002",
                ARVName = "Updated Protocol A",
                Description = "Updated first line treatment",
                AgeRange = "20-70",
                ForGroup = "Updated Adults"
            };

            _mockService.Setup(s => s.updateARVProtocol(dto)).Returns(true);

            // Setup user claims for Doctor role (R003)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123"),
                new Claim(ClaimTypes.Role, "R003")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.updateARVRegimen(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật phác đồ ARV thành công.", okResult.Value);
        }

     
        [Fact]
        public void updateARVRegimen_WithServiceFailure_ShouldReturnNotFound()
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP999999",
                ARVCode = "CODE999",
                ARVName = "Non-existent Protocol",
                Description = "This protocol doesn't exist",
                AgeRange = "0-100",
                ForGroup = "All ages"
            };

            _mockService.Setup(s => s.updateARVProtocol(dto)).Returns(false);

            // Setup user claims for Admin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R001")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.updateARVRegimen(dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Phác đồ ARV không tồn tại hoặc cập nhật không thành công.", notFoundResult.Value);
        }

       

        [Theory]
        [InlineData("R001")] // Admin
        [InlineData("R003")] // Doctor
        public void updateARVRegimen_WithAuthorizedRoles_ShouldAllowAccess(string role)
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE002",
                ARVName = "Updated Protocol A",
                Description = "Updated first line treatment",
                AgeRange = "20-70",
                ForGroup = "Updated Adults"
            };

            _mockService.Setup(s => s.updateARVProtocol(dto)).Returns(true);

            // Setup user claims for the specified role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, role)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.updateARVRegimen(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cập nhật phác đồ ARV thành công.", okResult.Value);
        }

        [Theory]
        [InlineData("R002")] // Manager
        [InlineData("R004")] // Staff
        [InlineData("R005")] // Patient
        public void updateARVRegimen_WithUnauthorizedRoles_ShouldReturnForbid(string role)
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE002",
                ARVName = "Updated Protocol A",
                Description = "Updated first line treatment",
                AgeRange = "20-70",
                ForGroup = "Updated Adults"
            };

            // Setup user claims for the specified role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, role)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };

            // Act
            var result = _controller.updateARVRegimen(dto);

            // Assert
            Assert.IsType<ForbidResult>(result);
            _mockService.Verify(s => s.updateARVProtocol(It.IsAny<ARVProtocolDTO>()), Times.Never);
        }

        #endregion
    }
} 