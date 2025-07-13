using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using HIVTreatment.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace HIVTreatment.Tests
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<IPrescriptionRepository> _mockRepo;
        private readonly PrescriptionService _service;

        public PrescriptionServiceTests()
        {
            _mockRepo = new Mock<IPrescriptionRepository>();
            _service = new PrescriptionService(_mockRepo.Object);
        }

       

        [Fact]
        public void AddPrescription_WithExistingPrescription_ShouldIncrementId()
        {
            // Arrange
            var dto = new PrescriptionDTO
            {
                MedicalRecordID = "TP002",
                MedicationID = "MED002",
                DoctorID = "DOC002",
                StartDate = new DateTime(2024, 2, 1),
                EndDate = new DateTime(2024, 2, 10),
                Dosage = "2 tablets",
                LineOfTreatment = "Second line"
            };
            var lastPrescription = new Prescription { PrescriptionID = "PR000005" };
            _mockRepo.Setup(r => r.GetLastPrescriptionById()).Returns(lastPrescription);
            _mockRepo.Setup(r => r.AddPrescription(It.IsAny<Prescription>()));

            // Act
            var result = _service.AddPrescription(dto);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.AddPrescription(It.Is<Prescription>(p => p.PrescriptionID == "PR000006")), Times.Once);
        }

        [Fact]
        public void AddPrescription_WithMalformedLastId_ShouldFallbackToOne()
        {
            // Arrange
            var dto = new PrescriptionDTO { MedicalRecordID = "TP003", MedicationID = "MED003", DoctorID = "DOC003", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(5), Dosage = "3 tablets", LineOfTreatment = "Third line" };
            var lastPrescription = new Prescription { PrescriptionID = "BADID" };
            _mockRepo.Setup(r => r.GetLastPrescriptionById()).Returns(lastPrescription);
            _mockRepo.Setup(r => r.AddPrescription(It.IsAny<Prescription>()));

            // Act
            var result = _service.AddPrescription(dto);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.AddPrescription(It.Is<Prescription>(p => p.PrescriptionID == "PR000001")), Times.Once);
        }

        [Fact]
        public void GetAllPrescription_ShouldReturnListFromRepository()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionID = "PR000001" },
                new Prescription { PrescriptionID = "PR000002" }
            };
            _mockRepo.Setup(r => r.GetAllPrescription()).Returns(prescriptions);

            // Act
            var result = _service.GetAllPrescription();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("PR000001", result[0].PrescriptionID);
        }

      

        [Fact]
        public void UpdatePrescription_WithValidData_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var dto = new UpdatePrescriptionDTO
            {
                PrescriptionID = "PR000001",
                MedicalRecordID = "TP001",
                MedicationID = "MED001",
                DoctorID = "DOC001",
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 1, 10),
                Dosage = "1 tablet",
                LineOfTreatment = "First line"
            };
            _mockRepo.Setup(r => r.UpdatePrescription(It.IsAny<Prescription>()));

            // Act
            var result = _service.UpdatePrescription(dto);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.UpdatePrescription(It.Is<Prescription>(p =>
                p.PrescriptionID == dto.PrescriptionID &&
                p.MedicalRecordID == dto.MedicalRecordID &&
                p.MedicationID == dto.MedicationID &&
                p.DoctorID == dto.DoctorID &&
                p.StartDate == dto.StartDate &&
                p.EndDate == dto.EndDate &&
                p.Dosage == dto.Dosage &&
                p.LineOfTreatment == dto.LineOfTreatment
            )), Times.Once);
        }
    }
} 