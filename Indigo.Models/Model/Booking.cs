using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indigo.Models.Model
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }

        public string? Status { get; set; }

        public int? ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        public int? PriceId { get; set; }

        [ForeignKey("PriceId")]
        public Price? Price { get; set; }


        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }




    }
}
