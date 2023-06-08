namespace Indigo.Models.DTO
{
    public class BookingUpdateDTO
    {

        public int BookingId { get; set; }
        public string? Status { get; set; } = null!;

        public string? Notes { get; set; } = null!;

        public string? Date { get; set; } = null!;

        public string? StartTime { get; set; } = null!;

        public int Duration { get; set; }

        public int? PriceId { get; set; }

        public int? ServiceId { get; set; }

    }
}
