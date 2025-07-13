using System.ComponentModel.DataAnnotations;

namespace HIVTreatment.Models
{
    public class Roles
    {
        [Key]
        public string RoleId { get; set; }
        public string RoleName { get; set; } 
    }
}
