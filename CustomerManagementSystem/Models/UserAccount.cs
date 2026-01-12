using System.ComponentModel.DataAnnotations;

namespace CustomerManagementSystem.Models
{
    public class UserAccount
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)]
        public string Role { get; set; } = "User";
    }
}
