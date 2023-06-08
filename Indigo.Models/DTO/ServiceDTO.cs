using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.DTO
{
    public class ServiceDTO
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public IFormFile? Image { get; set; }




    }
}
