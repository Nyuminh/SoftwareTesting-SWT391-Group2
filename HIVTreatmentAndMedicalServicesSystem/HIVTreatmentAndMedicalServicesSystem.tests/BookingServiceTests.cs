using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace HIVTreatment.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly BookingService _service;
        private readonly ApplicationDbContext _dbContext;
        private readonly ClaimsPrincipal _patientUser;
        private readonly ClaimsPrincipal _doctorUser;

        public BookingServiceTests()
        {
            // Setup mock DB context with in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BookingServiceTestDb" + Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            SetupTestData(_dbContext);

            _service = new BookingService(_dbContext);

            // Setup mock patient user
            var patientClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            };
            _patientUser = new ClaimsPrincipal(new ClaimsIdentity(patientClaims));

            // Setup mock doctor user
            var doctorClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123")
            };
            _doctorUser = new ClaimsPrincipal(new ClaimsIdentity(doctorClaims));
        }

        private void SetupTestData(ApplicationDbContext context)
        {
            // Add test role
            var userRole = new Roles { RoleId = "R001", RoleName = "Patient" };
            var doctorRole = new Roles { RoleId = "R002", RoleName = "Doctor" };
            context.Roles.Add(userRole);
            context.Roles.Add(doctorRole);
            context.SaveChanges();

            // Add test users with all required properties
            var patientUser = new HIVTreatment.Models.User
            {
                UserId = "user123",
                Fullname = "Test Patient",
                Email = "patient@test.com",
                Password = "password123",
                Address = "123 Patient St",
                RoleId = "R001"
            };

            var doctorUser = new HIVTreatment.Models.User
            {
                UserId = "doctor123",
                Fullname = "Test Doctor",
                Email = "doctor@test.com",
                Password = "password123",
                Address = "456 Doctor Ave",
                RoleId = "R002"
            };

            context.Users.Add(patientUser);
            context.Users.Add(doctorUser);
            context.SaveChanges();

            // Add test patients
            var patient = new Patient
            {
                PatientID = "P001",
                UserID = "user123",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                Phone = "1234567890",
                BloodType = "O+",
                Allergy = "None"
            };
            context.Patients.Add(patient);
            context.SaveChanges();

            // Add test doctors
            var doctor = new Doctor
            {
                DoctorId = "D001",
                UserId = "doctor123",
                Specialization = "HIV Treatment",
                LicenseNumber = "DOC12345",
                ExperienceYears = 5
            };
            context.Doctors.Add(doctor);
            context.SaveChanges();

            // Add work schedules
            var workSchedule = new DoctorWorkSchedule
            {
                ScheduleID = "WS001",
                DoctorID = "D001",
                SlotID = "S001",
                DateWork = DateTime.Today.AddDays(1)
            };
            context.DoctorWorkSchedules.Add(workSchedule);
            context.SaveChanges();

            // Add test appointments
            context.BooksAppointments.AddRange(
                new BooksAppointment
                {
                    BookID = "BK123456",
                    PatientID = "P001",
                    DoctorID = "D001",
                    BookingType = "Consultation",
                    BookDate = DateTime.Today.AddDays(2),
                    Status = "Thành công",
                    Note = "Test appointment"
                },
                new BooksAppointment
                {
                    BookID = "BK234567",
                    PatientID = "P001",
                    DoctorID = "D001",
                    BookingType = "Followup",
                    BookDate = DateTime.Today.AddDays(5),
                    Status = "Đã xác nhận",
                    Note = "Test follow-up"
                }
            );
            context.SaveChanges();
        }

        // Book Appointment Tests
        [Fact]
        public async Task BookAppointment_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new BookAppointmentDTO
            {
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(1),
                Note = "Test booking"
            };

            // Act
            var result = await _service.BookAppointment(dto, _patientUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            dynamic appointmentData = okResult.Value;
            Assert.StartsWith("BK", appointmentData.BookID);
            Assert.Equal(dto.BookingType, appointmentData.BookingType);
            Assert.Equal("Thành công", appointmentData.Status);
        }

        [Fact]
        public async Task BookAppointment_WithPastDate_ThrowsException()
        {
            // Arrange
            var dto = new BookAppointmentDTO
            {
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(-1),
                Note = "Test booking"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.BookAppointment(dto, _patientUser));
            Assert.Equal("Ngày đặt lịch không hợp lệ", exception.Message);
        }

        [Fact]
        public async Task BookAppointment_WithNonWorkingDoctorDate_ThrowsException()
        {
            // Arrange
            var dto = new BookAppointmentDTO
            {
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(3), // No work schedule for this date
                Note = "Test booking"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.BookAppointment(dto, _patientUser));
            Assert.Equal("Bác sĩ không làm việc vào thời gian này", exception.Message);
        }

        [Fact]
        public async Task BookAppointment_WithConflictingAppointment_ThrowsException()
        {
            // Arrange
            var dto = new BookAppointmentDTO
            {
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(2), // Existing appointment date
                Note = "Test booking"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.BookAppointment(dto, _patientUser));
            Assert.Equal("Bác sĩ đã có lịch hẹn trong ngày này", exception.Message);
        }

        // CancelAppointmentByPatient Tests
        [Fact]
        public async Task CancelAppointmentByPatient_WithValidData_ReturnsOkResult()
        {
            // Arrange - using existing appointment "BK234567" with status "Đã xác nhận"
            string appointmentId = "BK234567";
            string reason = "Patient cancellation reason";

            // Act
            var result = await _service.CancelAppointmentByPatient(appointmentId, reason, _patientUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Appointment cancelled by patient.", okResult.Value);
        }

        [Fact]
        public async Task CancelAppointmentByPatient_WithInvalidStatus_ReturnsBadRequestResult()
        {
            // First create an appointment with invalid status
            var appointment = new BooksAppointment
            {
                BookID = "BK999999",
                PatientID = "P001",
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(10),
                Status = "Đã hủy",  // Already cancelled
                Note = "Cannot cancel this"
            };
            _dbContext.BooksAppointments.Add(appointment);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.CancelAppointmentByPatient("BK999999", "Trying to cancel", _patientUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // GetMyAppointments Tests
        [Fact]
        public async Task GetMyAppointments_WithValidPatient_ReturnsOkResult()
        {
            // Act
            var result = await _service.GetMyAppointments(_patientUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var appointments = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, appointments.Count());
        }

        // PatientCheckIn Tests
        [Fact]
        public async Task PatientCheckIn_WithValidData_ReturnsOkResult()
        {
            // Arrange - using existing appointment with status "Thành công"
            string appointmentId = "BK123456";

            // Act
            var result = await _service.PatientCheckIn(appointmentId, _patientUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Patient check-in confirmed.", okResult.Value);
        }

        [Fact]
        public async Task PatientCheckIn_WithInvalidStatus_ReturnsBadRequestResult()
        {
            // Create an appointment with invalid status for check-in
            var appointment = new BooksAppointment
            {
                BookID = "BK888888",
                PatientID = "P001",
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(1),
                Status = "Đã khám",  // Already completed
                Note = "Cannot check in"
            };
            _dbContext.BooksAppointments.Add(appointment);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.PatientCheckIn("BK888888", _patientUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // DoctorCheckout Tests
        [Fact]
        public async Task DoctorCheckout_WithValidData_ReturnsOkResult()
        {
            // Create an appointment with "Đã xác nhận" status
            var appointment = new BooksAppointment
            {
                BookID = "BK777777",
                PatientID = "P001",
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Today.AddDays(1),
                Status = "Đã xác nhận",
                Note = "Ready for checkout"
            };
            _dbContext.BooksAppointments.Add(appointment);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.DoctorCheckout("BK777777", _doctorUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Doctor checkout completed.", okResult.Value);
        }

        [Fact]
        public async Task DoctorCheckout_WithInvalidStatus_ReturnsBadRequestResult()
        {
            // Using appointment with status "Thành công" without check-in
            string appointmentId = "BK123456";

            // Act
            var result = await _service.DoctorCheckout(appointmentId, _doctorUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // CancelAppointmentByDoctor Tests
        [Fact]
        public async Task CancelAppointmentByDoctor_WithValidData_ReturnsOkResult()
        {
            // Arrange
            string appointmentId = "BK123456"; // "Thành công" status
            string reason = "Doctor cancellation reason";

            // Act
            var result = await _service.CancelAppointmentByDoctor(appointmentId, reason, _doctorUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Appointment cancelled by doctor.", okResult.Value);
        }

        [Fact]
        public async Task CancelAppointmentByDoctor_WithInvalidStatus_ReturnsBadRequestResult()
        {
            // Arrange
            string appointmentId = "BK234567"; // "Đã xác nhận" status
            string reason = "Cannot cancel confirmed appointment";

            // Act
            var result = await _service.CancelAppointmentByDoctor(appointmentId, reason, _doctorUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // GetDoctorAppointments Tests
        [Fact]
        public async Task GetDoctorAppointments_WithValidDoctor_ReturnsOkResult()
        {
            // Act
            var result = await _service.GetDoctorAppointments(_doctorUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var appointments = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.NotEmpty(appointments);
        }

        // GetAppointmentsOfMyPatients Tests
        [Fact]
        public async Task GetAppointmentsOfMyPatients_WithValidDoctor_ReturnsOkResult()
        {
            // Act
            var result = await _service.GetAppointmentsOfMyPatients(_doctorUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var appointments = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.NotEmpty(appointments);
        }

        // GetAllAppointmentsForStaff Tests
        [Fact]
        public async Task GetAllAppointmentsForStaff_ReturnsOkResult()
        {
            // Act
            var result = await _service.GetAllAppointmentsForStaff();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var appointments = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.NotEmpty(appointments);
        }
    }
}