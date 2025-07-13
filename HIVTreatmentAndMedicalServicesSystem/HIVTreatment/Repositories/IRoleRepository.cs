using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IRoleRepository
    {
        List<Roles> GetAllRoles();
        Roles GetRoleById(string roleId);
    }
}
