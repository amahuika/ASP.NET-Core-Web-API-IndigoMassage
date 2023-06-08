using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.DTO
{
    public class BookingCreateDTO
    {
        // User information
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        public string? Notes { get; set; }


        [Required]
        public string Date { get; set; } = null!;

        [Required]
        public string StartTime { get; set; } = null!;


        public string EndTime { get; set; } = null!;

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int PriceId { get; set; }






    }
}
