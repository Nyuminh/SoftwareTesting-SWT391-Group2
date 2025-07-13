using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HIVTreatment.Tests
{
    public class DoctorRepositoryARVTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DoctorRepository _repository;

        public DoctorRepositoryARVTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new DoctorRepository(_context);
        }

        // Helper method để tạo ARVProtocol
        private ARVProtocol CreateARVProtocol(
            string arvId = "ARV001",
            string arvCode = "CODE001",
            string arvName = "Protocol A",
            string description = "First line treatment",
            string ageRange = "18-65",
            string forGroup = "Adults")
        {
            return new ARVProtocol
            {
                ARVID = arvId,
                ARVCode = arvCode,
                ARVName = arvName,
                Description = description,
                AgeRange = ageRange,
                ForGroup = forGroup
            };
        }

        #region AddARVProtocol Tests

        [Fact]
        public void AddARVProtocol_WithValidData_ShouldAddToDatabase()
        {
            // Arrange
            var arvProtocol = CreateARVProtocol();

            // Act
            _repository.AddARVProtocol(arvProtocol);

            // Assert
            var savedProtocol = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV001");
            Assert.NotNull(savedProtocol);
            Assert.Equal("ARV001", savedProtocol.ARVID);
            Assert.Equal("CODE001", savedProtocol.ARVCode);
            Assert.Equal("Protocol A", savedProtocol.ARVName);
            Assert.Equal("First line treatment", savedProtocol.Description);
            Assert.Equal("18-65", savedProtocol.AgeRange);
            Assert.Equal("Adults", savedProtocol.ForGroup);
        }

        [Fact]
        public void AddARVProtocol_WithNullARVProtocol_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.AddARVProtocol(null));
        }

        [Fact]
        public void AddARVProtocol_WithEmptyStrings_ShouldAddSuccessfully()
        {
            // Arrange
            var arvProtocol = new ARVProtocol
            {
                ARVID = "ARV002",
                ARVCode = "",
                ARVName = "",
                Description = "",
                AgeRange = "",
                ForGroup = ""
            };

            // Act
            _repository.AddARVProtocol(arvProtocol);

            // Assert
            var savedProtocol = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV002");
            Assert.NotNull(savedProtocol);
            Assert.Equal("", savedProtocol.ARVCode);
            Assert.Equal("", savedProtocol.ARVName);
            Assert.Equal("", savedProtocol.Description);
            Assert.Equal("", savedProtocol.AgeRange);
            Assert.Equal("", savedProtocol.ForGroup);
        }

        [Theory]
        [InlineData("ARV001", "CODE001", "Protocol A", "First line", "18-65", "Adults")]
        [InlineData("ARV002", "CODE002", "Protocol B", "Second line", "0-18", "Children")]
        [InlineData("ARV003", "CODE003", "Protocol C", "Third line", "65+", "Elderly")]
        public void AddARVProtocol_WithVariousData_ShouldAddCorrectly(
            string arvId, string arvCode, string arvName, string description, string ageRange, string forGroup)
        {
            // Arrange
            var arvProtocol = CreateARVProtocol(arvId, arvCode, arvName, description, ageRange, forGroup);

            // Act
            _repository.AddARVProtocol(arvProtocol);

            // Assert
            var savedProtocol = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == arvId);
            Assert.NotNull(savedProtocol);
            Assert.Equal(arvId, savedProtocol.ARVID);
            Assert.Equal(arvCode, savedProtocol.ARVCode);
            Assert.Equal(arvName, savedProtocol.ARVName);
            Assert.Equal(description, savedProtocol.Description);
            Assert.Equal(ageRange, savedProtocol.AgeRange);
            Assert.Equal(forGroup, savedProtocol.ForGroup);
        }

        [Fact]
        public void AddARVProtocol_WithSpecialCharacters_ShouldAddSuccessfully()
        {
            // Arrange
            var arvProtocol = new ARVProtocol
            {
                ARVID = "ARV-SPECIAL-001",
                ARVCode = "CODE-001",
                ARVName = "Protocol with spaces & symbols!",
                Description = "Treatment for HIV+ patients (Stage 2-3)",
                AgeRange = "18-65 years",
                ForGroup = "Adults & Adolescents"
            };

            // Act
            _repository.AddARVProtocol(arvProtocol);

            // Assert
            var savedProtocol = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV-SPECIAL-001");
            Assert.NotNull(savedProtocol);
            Assert.Equal("Protocol with spaces & symbols!", savedProtocol.ARVName);
            Assert.Equal("Treatment for HIV+ patients (Stage 2-3)", savedProtocol.Description);
            Assert.Equal("Adults & Adolescents", savedProtocol.ForGroup);
        }

        [Fact]
        public void AddARVProtocol_WithLongStrings_ShouldAddSuccessfully()
        {
            // Arrange
            var longName = new string('A', 500);
            var longDescription = new string('B', 1000);
            var longGroup = new string('C', 200);

            var arvProtocol = new ARVProtocol
            {
                ARVID = "ARV-LONG-001",
                ARVCode = "CODE001",
                ARVName = longName,
                Description = longDescription,
                AgeRange = "18-65",
                ForGroup = longGroup
            };

            // Act
            _repository.AddARVProtocol(arvProtocol);

            // Assert
            var savedProtocol = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV-LONG-001");
            Assert.NotNull(savedProtocol);
            Assert.Equal(longName, savedProtocol.ARVName);
            Assert.Equal(longDescription, savedProtocol.Description);
            Assert.Equal(longGroup, savedProtocol.ForGroup);
        }

        #endregion

        #region GetAllARVProtocol Tests

        [Fact]
        public void GetAllARVProtocol_WithMultipleProtocols_ShouldReturnAllProtocols()
        {
            // Arrange
            var protocols = new List<ARVProtocol>
            {
                CreateARVProtocol("ARV001", "CODE001", "Protocol A", "First line", "18-65", "Adults"),
                CreateARVProtocol("ARV002", "CODE002", "Protocol B", "Second line", "0-18", "Children"),
                CreateARVProtocol("ARV003", "CODE003", "Protocol C", "Third line", "65+", "Elderly")
            };

            _context.ARVProtocol.AddRange(protocols);
            _context.SaveChanges();

            // Act
            var result = _repository.GetAllARVProtocol();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.ARVID == "ARV001");
            Assert.Contains(result, p => p.ARVID == "ARV002");
            Assert.Contains(result, p => p.ARVID == "ARV003");
        }

        [Fact]
        public void GetAllARVProtocol_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var result = _repository.GetAllARVProtocol();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllARVProtocol_WithSingleProtocol_ShouldReturnOneProtocol()
        {
            // Arrange
            var protocol = CreateARVProtocol();
            _context.ARVProtocol.Add(protocol);
            _context.SaveChanges();

            // Act
            var result = _repository.GetAllARVProtocol();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ARV001", result[0].ARVID);
            Assert.Equal("CODE001", result[0].ARVCode);
            Assert.Equal("Protocol A", result[0].ARVName);
            Assert.Equal("First line treatment", result[0].Description);
            Assert.Equal("18-65", result[0].AgeRange);
            Assert.Equal("Adults", result[0].ForGroup);
        }

      


        #endregion

        #region updateARVProtocol Tests

        [Fact]
        public void updateARVProtocol_WithValidData_ShouldUpdateInDatabase()
        {
            // Arrange
            var originalProtocol = CreateARVProtocol();
            _context.ARVProtocol.Add(originalProtocol);
            _context.SaveChanges();

            var updatedProtocol = CreateARVProtocol(
                arvId: "ARV001",
                arvCode: "CODE002",
                arvName: "Updated Protocol A",
                description: "Updated first line treatment",
                ageRange: "20-70",
                forGroup: "Updated Adults"
            );

            // Act
            _repository.updateARVProtocol(updatedProtocol);

            // Assert
            var result = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV001");
            Assert.NotNull(result);
            Assert.Equal("CODE002", result.ARVCode);
            Assert.Equal("Updated Protocol A", result.ARVName);
            Assert.Equal("Updated first line treatment", result.Description);
            Assert.Equal("20-70", result.AgeRange);
            Assert.Equal("Updated Adults", result.ForGroup);
        }

        [Fact]
        public void updateARVProtocol_WithNullARVProtocol_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.updateARVProtocol(null));
        }

        [Fact]
        public void updateARVProtocol_WithNonExistentId_ShouldAddNewRecord()
        {
            // Arrange
            var nonExistentProtocol = CreateARVProtocol("ARV999", "CODE999", "New Protocol", "New description", "0-100", "All ages");

            // Act
            _repository.updateARVProtocol(nonExistentProtocol);

            // Assert
            var result = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV999");
            Assert.NotNull(result);
            Assert.Equal("CODE999", result.ARVCode);
            Assert.Equal("New Protocol", result.ARVName);
            Assert.Equal("New description", result.Description);
            Assert.Equal("0-100", result.AgeRange);
            Assert.Equal("All ages", result.ForGroup);
        }

        [Fact]
        public void updateARVProtocol_WithPartialData_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var originalProtocol = CreateARVProtocol();
            _context.ARVProtocol.Add(originalProtocol);
            _context.SaveChanges();

            var updatedProtocol = new ARVProtocol
            {
                ARVID = "ARV001", // Giữ nguyên
                ARVCode = "CODE002", // Thay đổi
                ARVName = "Protocol A", // Giữ nguyên
                Description = "Updated description", // Thay đổi
                AgeRange = "18-65", // Giữ nguyên
                ForGroup = "Updated Adults" // Thay đổi
            };

            // Act
            _repository.updateARVProtocol(updatedProtocol);

            // Assert
            var result = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV001");
            Assert.NotNull(result);
            Assert.Equal("CODE002", result.ARVCode); // Đã thay đổi
            Assert.Equal("Protocol A", result.ARVName); // Giữ nguyên
            Assert.Equal("Updated description", result.Description); // Đã thay đổi
            Assert.Equal("18-65", result.AgeRange); // Giữ nguyên
            Assert.Equal("Updated Adults", result.ForGroup); // Đã thay đổi
        }

        [Theory]
        [InlineData("CODE002", "Updated Protocol A", "Updated description", "20-70", "Updated Adults")]
        [InlineData("CODE003", "Protocol B", "Second line treatment", "0-18", "Children")]
        [InlineData("CODE004", "Protocol C", "Third line treatment", "65+", "Elderly")]
        public void updateARVProtocol_WithDifferentData_ShouldUpdateCorrectly(
            string arvCode, string arvName, string description, string ageRange, string forGroup)
        {
            // Arrange
            var originalProtocol = CreateARVProtocol();
            _context.ARVProtocol.Add(originalProtocol);
            _context.SaveChanges();

            var updatedProtocol = CreateARVProtocol(
                arvId: "ARV001",
                arvCode: arvCode,
                arvName: arvName,
                description: description,
                ageRange: ageRange,
                forGroup: forGroup
            );

            // Act
            _repository.updateARVProtocol(updatedProtocol);

            // Assert
            var result = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV001");
            Assert.NotNull(result);
            Assert.Equal(arvCode, result.ARVCode);
            Assert.Equal(arvName, result.ARVName);
            Assert.Equal(description, result.Description);
            Assert.Equal(ageRange, result.AgeRange);
            Assert.Equal(forGroup, result.ForGroup);
        }

        [Fact]
        public void updateARVProtocol_WithEmptyStrings_ShouldUpdateCorrectly()
        {
            // Arrange
            var originalProtocol = CreateARVProtocol();
            _context.ARVProtocol.Add(originalProtocol);
            _context.SaveChanges();

            var updatedProtocol = new ARVProtocol
            {
                ARVID = "ARV001",
                ARVCode = "",
                ARVName = "",
                Description = "",
                AgeRange = "",
                ForGroup = ""
            };

            // Act
            _repository.updateARVProtocol(updatedProtocol);

            // Assert
            var result = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV001");
            Assert.NotNull(result);
            Assert.Equal("", result.ARVCode);
            Assert.Equal("", result.ARVName);
            Assert.Equal("", result.Description);
            Assert.Equal("", result.AgeRange);
            Assert.Equal("", result.ForGroup);
        }

        [Fact]
        public void updateARVProtocol_MultipleUpdates_ShouldKeepLatestChanges()
        {
            // Arrange
            var originalProtocol = CreateARVProtocol();
            _context.ARVProtocol.Add(originalProtocol);
            _context.SaveChanges();

            // First update
            var firstUpdate = CreateARVProtocol("ARV001", "CODE002", "Protocol B", "Second line", "20-70", "Updated Adults");
            _repository.updateARVProtocol(firstUpdate);

            // Second update
            var secondUpdate = CreateARVProtocol("ARV001", "CODE003", "Protocol C", "Third line", "0-100", "All ages");
            _repository.updateARVProtocol(secondUpdate);

            // Assert - Should have latest values
            var result = _context.ARVProtocol.FirstOrDefault(a => a.ARVID == "ARV001");
            Assert.NotNull(result);
            Assert.Equal("CODE003", result.ARVCode);
            Assert.Equal("Protocol C", result.ARVName);
            Assert.Equal("Third line", result.Description);
            Assert.Equal("0-100", result.AgeRange);
            Assert.Equal("All ages", result.ForGroup);
        }

        #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 