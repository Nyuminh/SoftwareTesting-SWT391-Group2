using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface IRoleService
    {
        List<Roles> GetAllRoles();
        Roles GetRoleById(string roleId);
    }
}
