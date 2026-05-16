using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CNPM_Nhom12.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = "";

        [StringLength(200)]
        public string Address { get; set; } = "";

       
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
    }
}