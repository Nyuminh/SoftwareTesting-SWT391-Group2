using HIVTreatment.Data;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HIVTreatment.Tests
{
    public class PrescriptionRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly PrescriptionRepository _repository;

        public PrescriptionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new PrescriptionRepository(_context);
        }

        private Prescription CreatePrescription(
            string id = "PRE001",
            string medicalRecordId = "TP001",
            string medicationId = "MED001",
            string doctorId = "DOC001",
            string dosage = "1 tablet",
            string lineOfTreatment = "First line",
            string startDate = "2024-01-01",
            string endDate = "2024-01-10"
        )
        {
            return new Prescription
            {
                PrescriptionID = id,
                MedicalRecordID = medicalRecordId,
                MedicationID = medicationId,
                DoctorID = doctorId,
                Dosage = dosage,
                LineOfTreatment = lineOfTreatment,
                StartDate = DateTime.Parse(startDate),
                EndDate = DateTime.Parse(endDate)
            };
        }

        [Fact]
        public void AddPrescription_WithValidData_ShouldAddToDatabase()
        {
            // Arrange
            var prescription = CreatePrescription();

            // Act
            _repository.AddPrescription(prescription);

            // Assert
            var result = _context.Prescription.FirstOrDefault(p => p.PrescriptionID == "PRE001");
            Assert.NotNull(result);
            Assert.Equal("TP001", result.MedicalRecordID);
            Assert.Equal("MED001", result.MedicationID);
            Assert.Equal("DOC001", result.DoctorID);
            Assert.Equal("1 tablet", result.Dosage);
            Assert.Equal("First line", result.LineOfTreatment);
            Assert.Equal(DateTime.Parse("2024-01-01"), result.StartDate);
            Assert.Equal(DateTime.Parse("2024-01-10"), result.EndDate);
        }

      

        [Fact]
        public void GetAllPrescription_WithMultiplePrescriptions_ShouldReturnAll()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                CreatePrescription("PRE001"),
                CreatePrescription("PRE002", "TP002", "MED002", "DOC002", "2 tablets", "Second line", "2024-02-01", "2024-02-10"),
                CreatePrescription("PRE003", "TP003", "MED003", "DOC003", "3 tablets", "Third line", "2024-03-01", "2024-03-10")
            };
            _context.Prescription.AddRange(prescriptions);
            _context.SaveChanges();

            // Act
            var result = _repository.GetAllPrescription();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.PrescriptionID == "PRE002" && p.MedicationID == "MED002" && p.LineOfTreatment == "Second line");
        }

   

        [Fact]
        public void UpdatePrescription_WithValidData_ShouldUpdateInDatabase()
        {
            // Arrange
            var prescription = CreatePrescription();
            _context.Prescription.Add(prescription);
            _context.SaveChanges();

            // Modify some fields
            prescription.MedicationID = "MED999";
            prescription.Dosage = "2 tablets";
            prescription.LineOfTreatment = "Updated line";
            prescription.StartDate = DateTime.Parse("2024-05-01");
            prescription.EndDate = DateTime.Parse("2024-05-10");

            // Act
            _repository.UpdatePrescription(prescription);

            // Assert
            var result = _context.Prescription.FirstOrDefault(p => p.PrescriptionID == "PRE001");
            Assert.NotNull(result);
            Assert.Equal("MED999", result.MedicationID);
            Assert.Equal("2 tablets", result.Dosage);
            Assert.Equal("Updated line", result.LineOfTreatment);
            Assert.Equal(DateTime.Parse("2024-05-01"), result.StartDate);
            Assert.Equal(DateTime.Parse("2024-05-10"), result.EndDate);
        }

       
        [Fact]
        public void UpdatePrescription_WithNullPrescription_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _repository.UpdatePrescription(null));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
