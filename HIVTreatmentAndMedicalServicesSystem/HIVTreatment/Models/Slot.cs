namespace HIVTreatment.Models
{
    public class Slot
    {
        public string SlotID { get; set; }
        public int SlotNumber { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
