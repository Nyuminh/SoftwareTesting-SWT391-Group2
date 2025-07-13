using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R002" }; // Admin và Manager

            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin quyền !!!");
            }
            var roles = _roleService.GetAllRoles();
            return Ok(roles);
        }
        [HttpGet("GetRoleById/{roleId}")]
        public IActionResult GetRoleById(string roleId)
        {

            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R002" }; // Admin và Manager

            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền lấy quyền !!!");
            }
            var roleDetails = _roleService.GetRoleById(roleId);
            
            return Ok(roleDetails);
        }
    }
}
