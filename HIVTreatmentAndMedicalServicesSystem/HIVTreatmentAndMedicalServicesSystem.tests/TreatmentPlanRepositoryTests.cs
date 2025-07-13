using HIVTreatment.Data;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HIVTreatment.Tests
{
    public class TreatmentPlanRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TreatmentPlanRepository _repository;

        public TreatmentPlanRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new TreatmentPlanRepository(_context);
        }

        // Helper method để tạo TreatmentPlan
        private TreatmentPlan CreateTreatmentPlan(
            string treatmentPlanId,
            string patientId,
            string doctorId,
            string arvProtocol = "Protocol A",
            int treatmentLine = 1,
            string diagnosis = "HIV Positive",
            string treatmentResult = "Stable")
        {
            return new TreatmentPlan
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
        public void AddTreatmentPlan_WithValidData_ShouldAddToDatabase()
        {
            // Arrange
            var treatmentPlan = CreateTreatmentPlan("TP001", "P001", "D001");

            // Act
            _repository.AddTreatmentPlan(treatmentPlan);

            // Assert
            var savedPlan = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP001");
            Assert.NotNull(savedPlan);
            Assert.Equal("TP001", savedPlan.TreatmentPlanID);
            Assert.Equal("P001", savedPlan.PatientID);
            Assert.Equal("D001", savedPlan.DoctorID);
            Assert.Equal("Protocol A", savedPlan.ARVProtocol);
            Assert.Equal(1, savedPlan.TreatmentLine);
            Assert.Equal("HIV Positive", savedPlan.Diagnosis);
            Assert.Equal("Stable", savedPlan.TreatmentResult);
        }

        [Fact]
        public void AddTreatmentPlan_WithNullTreatmentPlan_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.AddTreatmentPlan(null));
        }

       

        [Theory]
        [InlineData("TP001", "P001", "D001", "Protocol A", 1, "HIV Positive", "Stable")]
        [InlineData("TP002", "P002", "D002", "Protocol B", 2, "HIV Advanced", "Critical")]
        public void AddTreatmentPlan_WithVariousData_ShouldAddCorrectly(
            string treatmentPlanId, string patientId, string doctorId,
            string arvProtocol, int treatmentLine, string diagnosis, string treatmentResult)
        {
            // Arrange
            var treatmentPlan = CreateTreatmentPlan(treatmentPlanId, patientId, doctorId, arvProtocol, treatmentLine, diagnosis, treatmentResult);

            // Act
            _repository.AddTreatmentPlan(treatmentPlan);

            // Assert
            var savedPlan = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == treatmentPlanId);
            Assert.NotNull(savedPlan);
            Assert.Equal(treatmentPlanId, savedPlan.TreatmentPlanID);
            Assert.Equal(patientId, savedPlan.PatientID);
            Assert.Equal(doctorId, savedPlan.DoctorID);
            Assert.Equal(arvProtocol, savedPlan.ARVProtocol);
            Assert.Equal(treatmentLine, savedPlan.TreatmentLine);
            Assert.Equal(diagnosis, savedPlan.Diagnosis);
            Assert.Equal(treatmentResult, savedPlan.TreatmentResult);
        }

        [Fact]
        public void AddTreatmentPlan_WithSpecialCharacters_ShouldAddSuccessfully()
        {
            // Arrange
            var treatmentPlan = new TreatmentPlan
            {
                TreatmentPlanID = "TP-SPECIAL-001",
                PatientID = "P-001",
                DoctorID = "D-001",
                ARVProtocol = "Protocol with spaces & symbols!",
                TreatmentLine = 1,
                Diagnosis = "HIV+ with complications (Stage 3)",
                TreatmentResult = "Stable - monitoring required"
            };

            // Act
            _repository.AddTreatmentPlan(treatmentPlan);

            // Assert
            var savedPlan = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP-SPECIAL-001");
            Assert.NotNull(savedPlan);
            Assert.Equal("Protocol with spaces & symbols!", savedPlan.ARVProtocol);
            Assert.Equal("HIV+ with complications (Stage 3)", savedPlan.Diagnosis);
            Assert.Equal("Stable - monitoring required", savedPlan.TreatmentResult);
        }

        [Fact]
        public void AddTreatmentPlan_WithLongStrings_ShouldAddSuccessfully()
        {
            // Arrange
            var longProtocol = new string('A', 1000); // 1000 ký tự
            var longDiagnosis = new string('B', 500); // 500 ký tự
            var longResult = new string('C', 750); // 750 ký tự

            var treatmentPlan = new TreatmentPlan
            {
                TreatmentPlanID = "TP-LONG-001",
                PatientID = "P001",
                DoctorID = "D001",
                ARVProtocol = longProtocol,
                TreatmentLine = 1,
                Diagnosis = longDiagnosis,
                TreatmentResult = longResult
            };

            // Act
            _repository.AddTreatmentPlan(treatmentPlan);

            // Assert
            var savedPlan = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP-LONG-001");
            Assert.NotNull(savedPlan);
            Assert.Equal(longProtocol, savedPlan.ARVProtocol);
            Assert.Equal(longDiagnosis, savedPlan.Diagnosis);
            Assert.Equal(longResult, savedPlan.TreatmentResult);
        }

        #endregion

        #region UpdateTreatmentPlan Tests

        [Fact]
        public void UpdateTreatmentPlan_WithValidData_ShouldUpdateInDatabase()
        {
            // Arrange
            var originalPlan = CreateTreatmentPlan("TP001", "P001", "D001", "Protocol A", 1, "HIV Positive", "Stable");
            _context.TreatmentPlan.Add(originalPlan);
            _context.SaveChanges();

            var updatedPlan = CreateTreatmentPlan("TP001", "P001", "D001", "Protocol B", 2, "HIV Advanced", "Improving");

            // Act
            _repository.UpdateTreatmentPlan(updatedPlan);

            // Assert
            var result = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP001");
            Assert.NotNull(result);
            Assert.Equal("Protocol B", result.ARVProtocol);
            Assert.Equal(2, result.TreatmentLine);
            Assert.Equal("HIV Advanced", result.Diagnosis);
            Assert.Equal("Improving", result.TreatmentResult);
        }

        [Fact]
        public void UpdateTreatmentPlan_WithNullTreatmentPlan_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.UpdateTreatmentPlan(null));
        }

        [Fact]
        public void UpdateTreatmentPlan_WithNonExistentId_ShouldAddNewRecord()
        {
            // Arrange
            var nonExistentPlan = CreateTreatmentPlan("TP999", "P999", "D999", "New Protocol", 1, "New Diagnosis", "New Result");

            // Act
            _repository.UpdateTreatmentPlan(nonExistentPlan);

            // Assert
            var result = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP999");
            Assert.NotNull(result);
            Assert.Equal("P999", result.PatientID);
            Assert.Equal("D999", result.DoctorID);
            Assert.Equal("New Protocol", result.ARVProtocol);
        }

        [Fact]
        public void UpdateTreatmentPlan_WithPartialData_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var originalPlan = CreateTreatmentPlan("TP001", "P001", "D001", "Protocol A", 1, "HIV Positive", "Stable");
            _context.TreatmentPlan.Add(originalPlan);
            _context.SaveChanges();

            var updatedPlan = new TreatmentPlan
            {
                TreatmentPlanID = "TP001",
                PatientID = "P001", // Giữ nguyên
                DoctorID = "D001",  // Giữ nguyên
                ARVProtocol = "Protocol B", // Thay đổi
                TreatmentLine = 2,  // Thay đổi
                Diagnosis = "HIV Positive", // Giữ nguyên
                TreatmentResult = "Improving" // Thay đổi
            };

            // Act
            _repository.UpdateTreatmentPlan(updatedPlan);

            // Assert
            var result = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP001");
            Assert.NotNull(result);
            Assert.Equal("Protocol B", result.ARVProtocol); // Đã thay đổi
            Assert.Equal(2, result.TreatmentLine); // Đã thay đổi
            Assert.Equal("HIV Positive", result.Diagnosis); // Giữ nguyên
            Assert.Equal("Improving", result.TreatmentResult); // Đã thay đổi
        }

        [Theory]
        [InlineData("Protocol A", 1, "HIV Positive", "Stable")]
        [InlineData("Protocol B", 2, "HIV Advanced", "Critical")]
        public void UpdateTreatmentPlan_WithDifferentProtocols_ShouldUpdateCorrectly(
            string arvProtocol, int treatmentLine, string diagnosis, string treatmentResult)
        {
            // Arrange
            var originalPlan = CreateTreatmentPlan("TP001", "P001", "D001", "Old Protocol", 0, "Old Diagnosis", "Old Result");
            _context.TreatmentPlan.Add(originalPlan);
            _context.SaveChanges();

            var updatedPlan = CreateTreatmentPlan("TP001", "P001", "D001", arvProtocol, treatmentLine, diagnosis, treatmentResult);

            // Act
            _repository.UpdateTreatmentPlan(updatedPlan);

            // Assert
            var result = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP001");
            Assert.NotNull(result);
            Assert.Equal(arvProtocol, result.ARVProtocol);
            Assert.Equal(treatmentLine, result.TreatmentLine);
            Assert.Equal(diagnosis, result.Diagnosis);
            Assert.Equal(treatmentResult, result.TreatmentResult);
        }

    
       
        [Fact]
        public void UpdateTreatmentPlan_WithSpecialCharacters_ShouldUpdateCorrectly()
        {
            // Arrange
            var originalPlan = CreateTreatmentPlan("TP001", "P001", "D001", "Old Protocol", 1, "Old Diagnosis", "Old Result");
            _context.TreatmentPlan.Add(originalPlan);
            _context.SaveChanges();

            var updatedPlan = new TreatmentPlan
            {
                TreatmentPlanID = "TP001",
                PatientID = "P001",
                DoctorID = "D001",
                ARVProtocol = "Protocol with spaces & symbols!",
                TreatmentLine = 2,
                Diagnosis = "HIV+ with complications (Stage 3)",
                TreatmentResult = "Stable - monitoring required"
            };

            // Act
            _repository.UpdateTreatmentPlan(updatedPlan);

            // Assert
            var result = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP001");
            Assert.NotNull(result);
            Assert.Equal("Protocol with spaces & symbols!", result.ARVProtocol);
            Assert.Equal(2, result.TreatmentLine);
            Assert.Equal("HIV+ with complications (Stage 3)", result.Diagnosis);
            Assert.Equal("Stable - monitoring required", result.TreatmentResult);
        }

       

        [Fact]
        public void UpdateTreatmentPlan_MultipleUpdates_ShouldKeepLatestChanges()
        {
            // Arrange
            var originalPlan = CreateTreatmentPlan("TP001", "P001", "D001", "Protocol A", 1, "HIV Positive", "Stable");
            _context.TreatmentPlan.Add(originalPlan);
            _context.SaveChanges();

            // First update
            var firstUpdate = CreateTreatmentPlan("TP001", "P001", "D001", "Protocol B", 2, "HIV Advanced", "Critical");
            _repository.UpdateTreatmentPlan(firstUpdate);

            // Second update
            var secondUpdate = CreateTreatmentPlan("TP001", "P001", "D001", "Protocol C", 3, "HIV Resistant", "Improving");
            _repository.UpdateTreatmentPlan(secondUpdate);

            // Assert - Should have latest values
            var result = _context.TreatmentPlan.FirstOrDefault(tp => tp.TreatmentPlanID == "TP001");
            Assert.NotNull(result);
            Assert.Equal("Protocol C", result.ARVProtocol);
            Assert.Equal(3, result.TreatmentLine);
            Assert.Equal("HIV Resistant", result.Diagnosis);
            Assert.Equal("Improving", result.TreatmentResult);
        }

       

        #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 