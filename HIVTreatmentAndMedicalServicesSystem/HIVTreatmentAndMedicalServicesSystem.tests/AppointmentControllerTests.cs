using HIVTreatment.Controllers;
using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace HIVTreatment.Tests.Controllers
{
    public class AppointmentControllerTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly AppointmentController _controller;
        private readonly ClaimsPrincipal _user;

        public AppointmentControllerTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _controller = new AppointmentController(_mockBookingService.Object);

            // Setup mock user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "R005")
            };
            _user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };
        }

        [Fact]
        public async Task BookAppointment_CallsService_ReturnsResult()
        {
            // Arrange
            var dto = new BookAppointmentDTO
            {
                DoctorID = "D001",
                BookingType = "Consultation",
                BookDate = DateTime.Now.AddDays(1),
                Note = "Test appointment"
            };
            var expectedResult = new OkObjectResult("Appointment created");
            _mockBookingService.Setup(s => s.BookAppointment(dto, _user))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.BookAppointment(dto);

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.BookAppointment(dto, _user), Times.Once);
        }

        [Fact]
        public async Task CancelAppointmentByPatient_CallsService_ReturnsResult()
        {
            // Arrange
            var appointmentId = "BK123456";
            var reason = "Cannot attend";
            var expectedResult = new OkObjectResult("Appointment cancelled");
            _mockBookingService.Setup(s => s.CancelAppointmentByPatient(appointmentId, reason, _user))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CancelAppointmentByPatient(appointmentId, reason);

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.CancelAppointmentByPatient(appointmentId, reason, _user), Times.Once);
        }

        [Fact]
        public async Task GetMyAppointments_CallsService_ReturnsResult()
        {
            // Arrange
            var expectedResult = new OkObjectResult(new[] { new { BookID = "BK123456" } });
            _mockBookingService.Setup(s => s.GetMyAppointments(_user))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetMyAppointments();

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.GetMyAppointments(_user), Times.Once);
        }

        [Fact]
        public async Task PatientCheckIn_CallsService_ReturnsResult()
        {
            // Arrange
            var appointmentId = "BK123456";
            var expectedResult = new OkObjectResult("Check-in successful");
            _mockBookingService.Setup(s => s.PatientCheckIn(appointmentId, _user))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.PatientCheckIn(appointmentId);

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.PatientCheckIn(appointmentId, _user), Times.Once);
        }

        [Fact]
        public async Task CancelAppointmentByDoctor_CallsService_ReturnsResult()
        {
            // Arrange
            var appointmentId = "BK123456";
            var reason = "Emergency";
            var expectedResult = new OkObjectResult("Appointment cancelled");

            // Setup doctor user
            var doctorClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123"),
                new Claim(ClaimTypes.Role, "R003")
            };
            var doctorUser = new ClaimsPrincipal(new ClaimsIdentity(doctorClaims));
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = doctorUser };

            _mockBookingService.Setup(s => s.CancelAppointmentByDoctor(appointmentId, reason, doctorUser))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CancelAppointmentByDoctor(appointmentId, reason);

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.CancelAppointmentByDoctor(appointmentId, reason, doctorUser), Times.Once);
        }

        [Fact]
        public async Task DoctorCheckout_CallsService_ReturnsResult()
        {
            // Arrange
            var appointmentId = "BK123456";
            var expectedResult = new OkObjectResult("Checkout successful");

            // Setup doctor user
            var doctorClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123"),
                new Claim(ClaimTypes.Role, "R003")
            };
            var doctorUser = new ClaimsPrincipal(new ClaimsIdentity(doctorClaims));
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = doctorUser };

            _mockBookingService.Setup(s => s.DoctorCheckout(appointmentId, doctorUser))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.DoctorCheckout(appointmentId);

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.DoctorCheckout(appointmentId, doctorUser), Times.Once);
        }

        [Fact]
        public async Task GetDoctorAppointments_CallsService_ReturnsResult()
        {
            // Arrange
            var expectedResult = new OkObjectResult(new[] { new { BookID = "BK123456" } });

            // Setup doctor user
            var doctorClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123"),
                new Claim(ClaimTypes.Role, "R003")
            };
            var doctorUser = new ClaimsPrincipal(new ClaimsIdentity(doctorClaims));
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = doctorUser };

            _mockBookingService.Setup(s => s.GetDoctorAppointments(doctorUser))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDoctorAppointments();

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.GetDoctorAppointments(doctorUser), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentsOfMyPatients_CallsService_ReturnsResult()
        {
            // Arrange
            var expectedResult = new OkObjectResult(new[] { new { BookID = "BK123456" } });

            // Setup doctor user
            var doctorClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor123"),
                new Claim(ClaimTypes.Role, "R003")
            };
            var doctorUser = new ClaimsPrincipal(new ClaimsIdentity(doctorClaims));
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = doctorUser };

            _mockBookingService.Setup(s => s.GetAppointmentsOfMyPatients(doctorUser))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetAppointmentsOfMyPatients();

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.GetAppointmentsOfMyPatients(doctorUser), Times.Once);
        }

        [Fact]
        public async Task GetAllAppointmentsForStaff_CallsService_ReturnsResult()
        {
            // Arrange
            var expectedResult = new OkObjectResult(new[] { new { BookID = "BK123456" } });

            // Setup staff user
            var staffClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "staff123"),
                new Claim(ClaimTypes.Role, "R004")
            };
            var staffUser = new ClaimsPrincipal(new ClaimsIdentity(staffClaims));
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = staffUser };

            _mockBookingService.Setup(s => s.GetAllAppointmentsForStaff())
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetAllAppointmentsForStaff();

            // Assert
            Assert.Same(expectedResult, result);
            _mockBookingService.Verify(s => s.GetAllAppointmentsForStaff(), Times.Once);
        }
    }
}