using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIVTreatment.Models
{
    [Table("DoctorWorkSchedule")]
    public class DoctorWorkSchedule
    {
        [Key]
        public string ScheduleID { get; set; }
        public string DoctorID { get; set; }
        public string SlotID { get; set; }
        public DateTime DateWork { get; set; }
    }
}
