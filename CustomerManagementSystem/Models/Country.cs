using System.ComponentModel.DataAnnotations;

namespace CustomerManagementSystem.Models
{
    public class Country
    {
        public int CountryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryName { get; set; } = string.Empty;
    }
}
