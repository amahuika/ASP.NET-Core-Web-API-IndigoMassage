
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Indigo.Models.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }

        public ICollection<Availability>? Availabilities { get; set; }
    }
}
