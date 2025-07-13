using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository iRoleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            iRoleRepository = roleRepository;
        }

        public List<Roles> GetAllRoles()
        {
            return iRoleRepository.GetAllRoles();
        }

        public Roles GetRoleById(string roleId)
        {
            return iRoleRepository.GetRoleById(roleId);
        }
    }
}
