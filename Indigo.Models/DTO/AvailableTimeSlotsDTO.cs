namespace Indigo.Models.DTO
{
    public class AvailableTimeSlotsDTO
    {

        public int SortOrder { get; set; }



        public string? DayOfWeek { get; set; }

        public string? DisplayDate { get; set; }
        public string? Date { get; set; }

        public List<AvailableSlotsDTO>? AvailableTimeSlots { get; set; }
    }
}
