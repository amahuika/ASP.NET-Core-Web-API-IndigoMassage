using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.DTO
{
    public class PriceDTO
    {

        public int id { get; set; }

        [Required]
        public int Duration { get; set; }
        [Required]
        public float Cost { get; set; }

        public int ServiceId { get; set; }
    }
}
