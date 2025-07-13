using HIVTreatment.Data;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext context;
        public RoleRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public List<Roles> GetAllRoles()
        {
            return context.Roles.ToList();
        }
        public Roles GetRoleById(string roleId)
        {
           return context.Roles.FirstOrDefault(r => r.RoleId == roleId);
        }
    }
    
}
