using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.Model
{
    public class Service
    {
        [Key]
        public int Id { get; set; }


        public string? Name { get; set; }

        public string? Description { get; set; }


        public string? ImageUrl { get; set; }

        public ICollection<Price>? Prices { get; set; }

    }
}
