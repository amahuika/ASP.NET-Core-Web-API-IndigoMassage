using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.DTO
{
    public class ServiceUpdateDTO
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public IFormFile? Image { get; set; }
    }
}
