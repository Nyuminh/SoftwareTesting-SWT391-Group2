using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using HIVTreatment.Services;
using Moq;
using Xunit;

namespace HIVTreatment.Tests
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _mockRepository;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _mockRepository = new Mock<IDoctorRepository>();
            _service = new DoctorService(_mockRepository.Object);
        }

        #region AddARVProtocol Tests

        [Fact]
        public void AddARVProtocol_WithValidData_ShouldReturnTrue()
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

            var existingProtocols = new List<ARVProtocolDTO>
            {
                new ARVProtocolDTO { ARVID = "AP000001", ARVCode = "CODE000", ARVName = "Protocol 0" }
            };

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(existingProtocols);
            _mockRepository.Setup(r => r.AddARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.AddARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddARVProtocol(It.Is<ARVProtocol>(p => 
                p.ARVID == "AP000002" && 
                p.ARVCode == "CODE001" && 
                p.ARVName == "Protocol A")), Times.Once);
        }

        [Fact]
        public void AddARVProtocol_WithDuplicateARVCode_ShouldReturnFalse()
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

            var existingProtocols = new List<ARVProtocolDTO>
            {
                new ARVProtocolDTO { ARVID = "AP000001", ARVCode = "CODE001", ARVName = "Existing Protocol" }
            };

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(existingProtocols);

            // Act
            var result = _service.AddARVProtocol(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.AddARVProtocol(It.IsAny<ARVProtocol>()), Times.Never);
        }

        [Fact]
        public void AddARVProtocol_WithDuplicateARVName_ShouldReturnFalse()
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = "CODE002",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            var existingProtocols = new List<ARVProtocolDTO>
            {
                new ARVProtocolDTO { ARVID = "AP000001", ARVCode = "CODE001", ARVName = "Protocol A" }
            };

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(existingProtocols);

            // Act
            var result = _service.AddARVProtocol(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.AddARVProtocol(It.IsAny<ARVProtocol>()), Times.Never);
        }

        [Fact]
        public void AddARVProtocol_WithEmptyDatabase_ShouldGenerateFirstID()
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

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(new List<ARVProtocolDTO>());
            _mockRepository.Setup(r => r.AddARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.AddARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddARVProtocol(It.Is<ARVProtocol>(p => p.ARVID == "AP000001")), Times.Once);
        }

        [Fact]
        public void AddARVProtocol_WithMultipleExistingProtocols_ShouldGenerateCorrectID()
        {
            // Arrange
            var dto = new CreateARVProtocolDTO
            {
                ARVCode = "CODE999",
                ARVName = "Protocol Z",
                Description = "Last line treatment",
                AgeRange = "65+",
                ForGroup = "Elderly"
            };

            var existingProtocols = new List<ARVProtocolDTO>
            {
                new ARVProtocolDTO { ARVID = "AP000001", ARVCode = "CODE001", ARVName = "Protocol A" },
                new ARVProtocolDTO { ARVID = "AP000005", ARVCode = "CODE005", ARVName = "Protocol E" },
                new ARVProtocolDTO { ARVID = "AP000010", ARVCode = "CODE010", ARVName = "Protocol J" }
            };

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(existingProtocols);
            _mockRepository.Setup(r => r.AddARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.AddARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddARVProtocol(It.Is<ARVProtocol>(p => p.ARVID == "AP000011")), Times.Once);
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

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(new List<ARVProtocolDTO>());
            _mockRepository.Setup(r => r.AddARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.AddARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddARVProtocol(It.Is<ARVProtocol>(p => 
                p.ARVCode == arvCode && 
                p.ARVName == arvName && 
                p.Description == description && 
                p.AgeRange == ageRange && 
                p.ForGroup == forGroup)), Times.Once);
        }

        #endregion

        #region updateARVProtocol Tests

        [Fact]
        public void updateARVProtocol_WithValidData_ShouldReturnTrue()
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

            var existingProtocol = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE001",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            _mockRepository.Setup(r => r.GetARVById("AP000001")).Returns(existingProtocol);
            _mockRepository.Setup(r => r.updateARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.updateARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.updateARVProtocol(It.Is<ARVProtocol>(p => 
                p.ARVID == "AP000001" && 
                p.ARVCode == "CODE002" && 
                p.ARVName == "Updated Protocol A")), Times.Once);
        }

        [Fact]
        public void updateARVProtocol_WithNonExistentID_ShouldReturnFalse()
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

            _mockRepository.Setup(r => r.GetARVById("AP999999")).Returns((ARVProtocolDTO)null);

            // Act
            var result = _service.updateARVProtocol(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.updateARVProtocol(It.IsAny<ARVProtocol>()), Times.Never);
        }

        [Fact]
        public void updateARVProtocol_WithNullDTO_ShouldReturnFalse()
        {
            // Act
            var result = _service.updateARVProtocol(null);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.updateARVProtocol(It.IsAny<ARVProtocol>()), Times.Never);
        }

        [Theory]
        [InlineData("CODE002", "Updated Protocol A", "Updated description", "20-70", "Updated Adults")]
        [InlineData("CODE003", "Protocol B", "Second line treatment", "0-18", "Children")]
        [InlineData("CODE004", "Protocol C", "Third line treatment", "65+", "Elderly")]
        public void updateARVProtocol_WithDifferentData_ShouldUpdateCorrectly(
            string arvCode, string arvName, string description, string ageRange, string forGroup)
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = arvCode,
                ARVName = arvName,
                Description = description,
                AgeRange = ageRange,
                ForGroup = forGroup
            };

            var existingProtocol = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE001",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            _mockRepository.Setup(r => r.GetARVById("AP000001")).Returns(existingProtocol);
            _mockRepository.Setup(r => r.updateARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.updateARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.updateARVProtocol(It.Is<ARVProtocol>(p => 
                p.ARVCode == arvCode && 
                p.ARVName == arvName && 
                p.Description == description && 
                p.AgeRange == ageRange && 
                p.ForGroup == forGroup)), Times.Once);
        }

        [Fact]
        public void updateARVProtocol_WithEmptyStrings_ShouldUpdateCorrectly()
        {
            // Arrange
            var dto = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "",
                ARVName = "",
                Description = "",
                AgeRange = "",
                ForGroup = ""
            };

            var existingProtocol = new ARVProtocolDTO
            {
                ARVID = "AP000001",
                ARVCode = "CODE001",
                ARVName = "Protocol A",
                Description = "First line treatment",
                AgeRange = "18-65",
                ForGroup = "Adults"
            };

            _mockRepository.Setup(r => r.GetARVById("AP000001")).Returns(existingProtocol);
            _mockRepository.Setup(r => r.updateARVProtocol(It.IsAny<ARVProtocol>()));

            // Act
            var result = _service.updateARVProtocol(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.updateARVProtocol(It.Is<ARVProtocol>(p => 
                p.ARVCode == "" && 
                p.ARVName == "" && 
                p.Description == "" && 
                p.AgeRange == "" && 
                p.ForGroup == "")), Times.Once);
        }

        #endregion

        #region GetAllARVProtocol Tests

        [Fact]
        public void GetAllARVProtocol_WithMultipleProtocols_ShouldReturnAllProtocols()
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
                },
                new ARVProtocolDTO
                {
                    ARVID = "AP000003",
                    ARVCode = "CODE003",
                    ARVName = "Protocol C",
                    Description = "Third line treatment",
                    AgeRange = "65+",
                    ForGroup = "Elderly"
                }
            };

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(expectedProtocols);

            // Act
            var result = _service.GetAllARVProtocol();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("AP000001", result[0].ARVID);
            Assert.Equal("CODE001", result[0].ARVCode);
            Assert.Equal("Protocol A", result[0].ARVName);
            Assert.Equal("First line treatment", result[0].Description);
            Assert.Equal("18-65", result[0].AgeRange);
            Assert.Equal("Adults", result[0].ForGroup);
        }

        [Fact]
        public void GetAllARVProtocol_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(new List<ARVProtocolDTO>());

            // Act
            var result = _service.GetAllARVProtocol();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllARVProtocol_WithSingleProtocol_ShouldReturnOneProtocol()
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

            _mockRepository.Setup(r => r.GetAllARVProtocol()).Returns(expectedProtocols);

            // Act
            var result = _service.GetAllARVProtocol();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("AP000001", result[0].ARVID);
            Assert.Equal("CODE001", result[0].ARVCode);
            Assert.Equal("Protocol A", result[0].ARVName);
        }

    
        #endregion
    }
} 