using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.Model
{
    public class Availability
    {
        [Key]
        public int Id { get; set; }


        public TimeSpan StartTime { get; set; }


        public TimeSpan EndTime { get; set; }


        public DateTime Date { get; set; }


        public string? ApplicationUserId { get; set; }









    }
}
