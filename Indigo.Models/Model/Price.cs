using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.Model
{
    public class Price
    {
        [Key]
        public int Id { get; set; }
        public int Duration { get; set; }

        public float Cost { get; set; }

        public int ServiceId { get; set; }




    }
}
