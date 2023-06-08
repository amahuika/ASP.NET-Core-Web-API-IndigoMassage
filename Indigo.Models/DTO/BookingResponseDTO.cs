namespace Indigo.Models.DTO
{
    public class BookingResponseDTO
    {

        public int BookingId { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Notes { get; set; }

        // service details

        public string? serviceName { get; set; }
        public string? Date { get; set; }

        public string? DisplayDate { get; set; }

        public int? ServiceId { get; set; }

        public int? PriceId { get; set; }
        public string? Time { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public string? Cost { get; set; }

        public string? Duration { get; set; }

        public string? Status { get; set; }





    }
}
