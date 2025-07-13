using System.ComponentModel.DataAnnotations;

namespace HIVTreatment.Models
{
    public class ARVProtocol
    {
        [Key]
        public string ARVID { get; set; }
        public string ARVCode { get; set; }
        public string ARVName { get; set; }
        public string Description { get; set; }
        public string AgeRange { get; set; }
        public string ForGroup { get; set; }
    }
}
