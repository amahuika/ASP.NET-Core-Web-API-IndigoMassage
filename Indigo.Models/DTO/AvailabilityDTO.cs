using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.DTO
{
    public class AvailabilityDTO
    {

        [Required]
        public string StartTime { get; set; } = null!;

        [Required]
        public string EndTime { get; set; } = null!;

        [Required]
        public string Date { get; set; } = null!;
    }
}
