using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HIVTreatment.Models;
using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HIVTreatment.Services;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R002")]


    public class ManagerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorService _doctorService;
        private readonly IUserService _userService;

        public ManagerController(ApplicationDbContext context, IUserRepository userRepository, IDoctorService doctorService,IUserService userService)
        {
            _context = context;
            _userRepository = userRepository;
            _doctorService = doctorService;
            _userService = userService;
        }

        [HttpGet("AllUsers")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            if (users == null || !users.Any())
            {
                return NotFound("Không có người dùng nào.");
            }
            return Ok(users);
        }

        [HttpGet("User/{userId}")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetUserById(string userId)
        {
            var user = _userService.GetByUserId(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            return Ok(user);
        }

        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] CreateDoctorDTO dto)
        {
            // kiểm tra email đã tồn tại chưa
            if (_userRepository.EmailExists(dto.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            // Tạo UserId
            var lastUser = _userRepository.GetLastUser();
            int nextUserId = 1;
            if (lastUser != null && int.TryParse(lastUser.UserId.Substring(3), out int lastId))
            {
                nextUserId = lastId + 1;
            }
            string newUserId = $"UI{nextUserId:D6}";


            //Tạo User mới
            var user = new User
            {
                UserId = newUserId,
                RoleId = "R003", //Doctor
                Fullname = dto.FullName,
                Password = dto.Password,
                Email = dto.Email,
                Address = dto.Address,
                Image = dto.Image
            };

            try
            {
                _userRepository.Add(user); // Lưu User vào database
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Tạo user thất bại: {ex.InnerException?.Message ?? ex.Message}");
            }

            // Tạo DoctorId
            // DT + 6 số
            var lastDoctor = _context.Doctors.OrderByDescending(d => d.DoctorId).FirstOrDefault();
            int nextDoctorId = 1;
            if (lastDoctor != null && int.TryParse(lastDoctor.DoctorId.Substring(2), out int lastDId))
            {
                nextDoctorId = lastDId + 1;
            }
            string newDoctorId = $"DT{nextDoctorId:D6}";

            // Tạo Doctor mới và gán UserId vừa tạo
            var doctor = new Doctor
            {
                DoctorId = newDoctorId,
                UserId = newUserId, // Gán UserId từ User vừa tạo
                Specialization = dto.Specialization,
                LicenseNumber = dto.LicenseNumber,
                ExperienceYears = dto.ExperienceYears
            };

            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Thêm bác sĩ mới thất bại: {ex.Message}");
            }

            return Ok(new { message = "Thêm bác sĩ thành công.", doctorId = newDoctorId, userId = newUserId });
        }

        [HttpGet("AllDoctors")]
        public IActionResult GetAllDoctors()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem danh sách bác sĩ!");
            }
            var doctors = _doctorService.GetAllDoctors();
            if (doctors == null || !doctors.Any())
            {
                return NotFound("Không có bác sĩ nào.");
            }
            return Ok(doctors);
        }

        [HttpGet("InfoDoctor/{doctorId}")]
        public IActionResult GetInfoDoctorById(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin bác sĩ!");
            }
            var doctorInfo = _doctorService.GetInfoDoctorById(doctorId);
            if (doctorInfo == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");
            }
            return Ok(doctorInfo);
        }

        [HttpGet("DoctorWorkSchedule/{doctorId}")]
        public IActionResult GetScheduleByDoctorId(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem lịch làm việc của bác sĩ!");
            }
            var schedule = _doctorService.GetScheduleByDoctorId(doctorId);
            if (schedule == null || !schedule.Any())
            {
                return NotFound("Không tìm thấy lịch làm việc của bác sĩ.");
            }
            return Ok(schedule);
        }

        [HttpPut("UpdateDoctor/{doctorId}")]
        public async Task<IActionResult> EditDoctor(string doctorId, [FromBody] EditDoctorDTO dto)
        {
            // 1. Tìm doctor theo doctorId
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor == null)
                return NotFound("Không tìm thấy bác sĩ.");

            // 2. Tìm user theo UserId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == doctor.UserId);
            if (user == null)
                return NotFound("Không tìm thấy tài khoản bác sĩ.");

            // 3. Kiểm tra trùng email (nếu email thay đổi)
            if (user.Email != dto.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != user.UserId);
                if (emailExists)
                    return BadRequest("Email đã tồn tại.");
            }

            // 4. Kiểm tra trùng LicenseNumber (nếu thay đổi)
            if (doctor.LicenseNumber != dto.LicenseNumber)
            {
                var licenseExists = await _context.Doctors.AnyAsync(d => d.LicenseNumber == dto.LicenseNumber && d.DoctorId != doctorId);
                if (licenseExists)
                    return BadRequest("Số giấy phép đã tồn tại.");
            }

            // 5. Cập nhật thông tin
            user.Fullname = dto.FullName;
            user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.Image)) user.Image = dto.Image;

            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.ExperienceYears = dto.ExperienceYears;

            // 6. Lưu thay đổi
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thông tin bác sĩ thành công." });
        }

        [HttpGet("AllDoctorWorkSchedules")]
        public IActionResult GetAllDoctorWorkSchedules()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem lịch làm việc của bác sĩ!");
            }

            var schedules = (from dws in _context.DoctorWorkSchedules
                             join d in _context.Doctors on dws.DoctorID equals d.DoctorId
                             join u in _context.Users on d.UserId equals u.UserId
                             join s in _context.Slot on dws.SlotID equals s.SlotID
                             select new
                             {
                                 ScheduleID = dws.ScheduleID,
                                 DoctorID = d.DoctorId,
                                 DoctorName = u.Fullname,
                                 SlotID = s.SlotID,
                                 SlotNumber = s.SlotNumber,
                                 StartTime = s.StartTime,
                                 EndTime = s.EndTime,
                                 DateWork = dws.DateWork
                             }).ToList();

            if (schedules == null || !schedules.Any())
            {
                return NotFound("Không có lịch làm việc nào.");
            }
            return Ok(schedules);
        }


        [HttpGet("DoctorWorkScheduleDetail/{scheduleId}")]
        public IActionResult GetDoctorWorkScheduleDetail(string scheduleId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem chi tiết lịch làm việc!");
            }

            var schedule = (from dws in _context.DoctorWorkSchedules
                            join d in _context.Doctors on dws.DoctorID equals d.DoctorId
                            join u in _context.Users on d.UserId equals u.UserId
                            join s in _context.Slot on dws.SlotID equals s.SlotID
                            where dws.ScheduleID == scheduleId
                            select new
                            {
                                ScheduleID = dws.ScheduleID,
                                DoctorID = d.DoctorId,
                                DoctorName = u.Fullname,
                                SlotID = s.SlotID,
                                SlotNumber = s.SlotNumber,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime,
                                DateWork = dws.DateWork
                            }).FirstOrDefault();

            if (schedule == null)
            {
                return NotFound("Không tìm thấy lịch làm việc này.");
            }
            return Ok(schedule);
        }

        [HttpPut("UpdateDoctorWorkSchedule/{scheduleId}")]
        public async Task<IActionResult> EditDoctorWorkSchedule(string scheduleId, [FromBody] EditDoctorWorkScheduleDTO dto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền chỉnh sửa lịch làm việc!");
            }

            var schedule = await _context.DoctorWorkSchedules.FirstOrDefaultAsync(dws => dws.ScheduleID == scheduleId);
            if (schedule == null)
            {
                return NotFound("Không tìm thấy lịch làm việc này.");
            }

            // Kiểm tra DoctorID hợp lệ
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == dto.DoctorID);
            if (doctor == null)
            {
                return BadRequest("DoctorID không hợp lệ.");
            }

            // Kiểm tra SlotID hợp lệ
            var slot = await _context.Slot.FirstOrDefaultAsync(s => s.SlotID == dto.SlotID);
            if (slot == null)
            {
                return BadRequest("SlotID không hợp lệ.");
            }

            // Kiểm tra trùng lịch
            bool isDuplicate = await _context.DoctorWorkSchedules.AnyAsync(dws =>
                dws.DoctorID == dto.DoctorID &&
                dws.SlotID == dto.SlotID &&
                dws.DateWork.Date == dto.DateWork.Date &&
                dws.ScheduleID != scheduleId
            );
            if (isDuplicate)
            {
                return BadRequest("Bác sĩ đã có lịch làm việc trùng ca và ngày này.");
            }

            // Cập nhật thông tin
            schedule.DoctorID = dto.DoctorID;
            schedule.SlotID = dto.SlotID;
            schedule.DateWork = dto.DateWork;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật lịch làm việc thành công." });
        }

        [HttpPost("AddDoctorWorkSchedule")]
        public async Task<IActionResult> AddDoctorWorkSchedule([FromBody] EditDoctorWorkScheduleDTO dto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền thêm lịch làm việc!");
            }

            // Kiểm tra DoctorID hợp lệ
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == dto.DoctorID);
            if (doctor == null)
            {
                return BadRequest("DoctorID không hợp lệ.");
            }

            // Kiểm tra SlotID hợp lệ
            var slot = await _context.Slot.FirstOrDefaultAsync(s => s.SlotID == dto.SlotID);
            if (slot == null)
            {
                return BadRequest("SlotID không hợp lệ.");
            }

            // Kiểm tra trùng lịch
            bool isDuplicate = await _context.DoctorWorkSchedules.AnyAsync(dws =>
                dws.DoctorID == dto.DoctorID &&
                dws.SlotID == dto.SlotID &&
                dws.DateWork.Date == dto.DateWork.Date
            );
            if (isDuplicate)
            {
                return BadRequest("Bác sĩ đã có lịch làm việc trùng ca và ngày này.");
            }

            // Tạo ScheduleID mới (DW + 6 số)
            var lastSchedule = _context.DoctorWorkSchedules.OrderByDescending(dws => dws.ScheduleID).FirstOrDefault();
            int nextScheduleId = 1;
            if (lastSchedule != null && int.TryParse(lastSchedule.ScheduleID.Substring(2), out int lastId))
            {
                nextScheduleId = lastId + 1;
            }
            string newScheduleId = $"DW{nextScheduleId:D6}";

            // Tạo mới lịch làm việc
            var newSchedule = new DoctorWorkSchedule
            {
                ScheduleID = newScheduleId,
                DoctorID = dto.DoctorID,
                SlotID = dto.SlotID,
                DateWork = dto.DateWork
            };

            _context.DoctorWorkSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm lịch làm việc thành công.", scheduleId = newScheduleId });
        }

        [HttpGet("AllARVProtocol")]
        public IActionResult GetAllARVRegiemns()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem ARV Protocol!");
            }
            // Fix: Invoke the delegate to get the actual list before calling Any()
            var arvProtocols = _doctorService.GetAllARVProtocol();
            if (arvProtocols == null || !arvProtocols.Any())
            {
                return NotFound("Không có phác đồ ARV nào.");
            }
            return Ok(arvProtocols);
        }

        [HttpGet("ARVProtocol/{ARVProtocolID}")]
        public IActionResult GetARVById(string ARVProtocolID)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002","R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem phác đồ ARV!");
            }
            var arvProtocol = _doctorService.GetARVById(ARVProtocolID);
            if (arvProtocol == null)
            {
                return NotFound("Không tìm thấy phác đồ ARV.");
            }
            return Ok(arvProtocol);
        }

        [HttpPost("AddARVProtocol")]
        [Authorize(Roles = "R001,R002")]
        public IActionResult AddARVProtocol([FromBody] CreateARVProtocolDTO dto)
        {
            var result = _doctorService.AddARVProtocol(dto);
            if (!result)
                return BadRequest("Phác đồ ARV đã tồn tại hoặc dữ liệu không hợp lệ.");
            return Ok("Thêm phác đồ ARV thành công.");
        }

        [HttpPut("UpdateARVProtocol")]
        public IActionResult updateARVRegimen(ARVProtocolDTO ARVProtocolDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001","R002", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền sửa phác đồ ARV!");
            }
            var arvUpdated = _doctorService.updateARVProtocol(ARVProtocolDTO);
            if (!arvUpdated)
            {
                return NotFound("Phác đồ ARV không tồn tại hoặc cập nhật không thành công.");
            }
            return Ok("Cập nhật phác đồ ARV thành công.");
        }

        [HttpGet("AllStaff")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetAllStaff()
        {
            var staffList = _userService.GetAllStaff();
            if (staffList == null || !staffList.Any())
            {
                return NotFound("Không có staff nào.");
            }
            return Ok(staffList);
        }
        [HttpGet("Staff/{userId}")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetStaffById(string userId)
        {
            var staff = _userService.GetStaffById(userId);
            if (staff == null)
                return NotFound("Không tìm thấy staff.");
            return Ok(staff);
        }

        [HttpPost("AddStaff")]
        [Authorize(Roles = "R001,R002")]
        public IActionResult AddStaff([FromBody] CreateStaffDTO staffDTO)
        {
            var user = _userService.AddStaff(staffDTO);
            if (user == null)
                return BadRequest("Email đã tồn tại hoặc dữ liệu không hợp lệ.");
            return Ok(new { message = "Thêm staff thành công.", userId = user.UserId });
        }

        [HttpPut("UpdateStaff/{userId}")]
        public IActionResult UpdateStaff(string userId, [FromBody] UpdateStaffDTO staffDTO)
        {
            var result = _userService.UpdateStaff(userId, staffDTO);
            if (!result)
                return BadRequest("Cập nhật staff thất bại (không tìm thấy staff hoặc email đã tồn tại).");
            return Ok("Cập nhật staff thành công.");
        }

    }


}