namespace HIVTreatment.DTOs
{
    public class DoctorScheduleDTO
    {
        public string ScheduleID { get; set; }
        public DateTime DateWork { get; set; }
        public int SlotNumber { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        //public DateTime? BookDate { get; set; }
    }
}
