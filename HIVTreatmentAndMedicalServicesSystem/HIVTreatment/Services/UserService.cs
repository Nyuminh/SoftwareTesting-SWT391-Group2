using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HIVTreatment.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _context = context;
        }

        public List<ARVByPatientDTO> GetARVByPatientId(string patientId)
        {
            return _userRepository.GetARVByPatientId(patientId);
        }

        public User GetByUserId(string userId)
        {
            return _userRepository.GetByUserId(userId);
        }

        public List<PrescriptionByPatient> GetPrescriptionByPatientId(string patientId)
        {return _userRepository.GetPrescriptionByPatientId(patientId);
        }

        //login jwt token
        public UserLoginResponse Login(string email, string password)
        {
            try
            {
                var user = _userRepository.GetByEmail(email);
                if (user == null || user.Password != password)
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                if (key.Length < 16)
                {
                    throw new Exception("JWT Key must be at least 16 characters long");
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleId)
        };

                // Nếu là bác sĩ thì thêm claim DoctorID
                if (user.RoleId == "R003")
                {
                    var doctor = _userRepository.GetDoctorByUserId(user.UserId);
                    if (doctor != null)
                    {
                        claims.Add(new Claim("DoctorID", doctor.DoctorId)); // Lưu ý: doctor.DoctorId chứ không phải DoctorID
                    }
                }

                if (!double.TryParse(_configuration["Jwt:ExpiryInHours"], out double expiryHours))
                {
                    expiryHours = 3;
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(expiryHours),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return new UserLoginResponse
                {
                    UserId = user.UserId,
                    RoleId = user.RoleId,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Token = tokenHandler.WriteToken(token)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating token: {ex.Message}");
                throw;
            }
        }


        public User Register(User user)
        {
            if (_userRepository.EmailExists(user.Email)) return null;

            var lastUser = _userRepository.GetLastUser();
            int nextId = 1;
            if (lastUser != null)
            {
                string numberPart = lastUser.UserId.Substring(3);
                if (int.TryParse(numberPart, out int parsed))
                    nextId = parsed + 1;
            }

            user.UserId = "UI" + nextId.ToString("D6");
            _userRepository.Add(user);
            return user;
        }


        public bool ResetPassword(string email, string newPassword)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null || user.RoleId != "R005") // chỉ Patient mới được reset
            {
                return false;
            }

            user.Password = newPassword; // bạn có thể hash ở đây
            _userRepository.UpdatePassword(email, newPassword);
            return true;
        }
        public Patient GetPatientByUserId(string userId)
        {
            return _context.Patients
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserID == userId);
        }

        public List<PrescriptionByPatient> GetPrescriptionsOfPatient(string userId)
        {
            return _userRepository.GetPrescriptionsOfPatient(userId);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User AddUser(UserDTO userDTO)
        {
            if (_userRepository.EmailExists(userDTO.Email)) return null;

            var lastUser = _userRepository.GetLastUser();
            int nextId = 1;
            if (lastUser != null)
            {
                string numberPart = lastUser.UserId.Substring(3);
                if (int.TryParse(numberPart, out int parsed))
                    nextId = parsed + 1;
            }

            string newUserId = "UI" + nextId.ToString("D6");

            var user = new User
            {
                UserId = newUserId,
                Fullname = userDTO.Fullname,
                Email = userDTO.Email,
                Password = userDTO.Password, 
                RoleId = userDTO.RoleId,
                Address = userDTO.Address,
                Image = "patient.png"
            };
            _userRepository.Add(user);
            return user;
        }

        public bool UpdateUser(UpdateUserDTO userDTO)
        {
            var existingUser = _userRepository.GetByUserId(userDTO.UserId);
            if (existingUser == null) return false;
            existingUser.RoleId = userDTO.RoleId;
            existingUser.Fullname = userDTO.Fullname; 
            existingUser.Email = userDTO.Email; 
            existingUser.Password = userDTO.Password;
            existingUser.Address = userDTO.Address;
            existingUser.Image = "patient.png"; 
            _userRepository.Update(existingUser);
            return true;
        }

        public List<StaffDTO> GetAllStaff()
        {
            var staffUsers = _userRepository.GetUsersByRole("R004");
            return staffUsers.Select(u => new StaffDTO
            {
                UserId = u.UserId,
                Fullname = u.Fullname,
                Email = u.Email,
                Address = u.Address,
                Image = u.Image
            }).ToList();
        }

        public StaffDTO GetStaffById(string userId)
        {
            var user = _userRepository.GetStaffById(userId);
            if (user == null) return null;
            return new StaffDTO
            {
                UserId = user.UserId,
                Fullname = user.Fullname,
                Email = user.Email,
                Address = user.Address,
                Image = user.Image
            };
        }

        public User AddStaff(CreateStaffDTO staffDTO)
        {
            if (_userRepository.EmailExists(staffDTO.Email)) return null;

            var lastUser = _userRepository.GetLastUser();
            int nextId = 1;
            if (lastUser != null)
            {
                string numberPart = lastUser.UserId.Substring(3);
                if (int.TryParse(numberPart, out int parsed))
                    nextId = parsed + 1;
            }

            string newUserId = "UI" + nextId.ToString("D6");

            var user = new User
            {
                UserId = newUserId,
                Fullname = staffDTO.Fullname,
                Email = staffDTO.Email,
                Password = staffDTO.Password,
                RoleId = "R004", // Staff
                Address = staffDTO.Address,
                Image = string.IsNullOrEmpty(staffDTO.Image) ? "staff.png" : staffDTO.Image
            };
            _userRepository.Add(user);
            return user;
        }

        public bool UpdateStaff(String userId, UpdateStaffDTO staffDTO)
        {
            var user = _userRepository.GetByUserId(userId);
            if (user == null || user.RoleId != "R004")
                return false; // Chỉ cho phép sửa staff

            // Kiểm tra trùng email
            if (user.Email != staffDTO.Email && _userRepository.EmailExists(staffDTO.Email))
                return false;

            user.Fullname = staffDTO.Fullname;
            user.Email = staffDTO.Email;
            user.Address = staffDTO.Address;
            user.Image = string.IsNullOrEmpty(staffDTO.Image) ? "staff.png" : staffDTO.Image;

            _userRepository.Update(user);
            return true;
        }
    }



}