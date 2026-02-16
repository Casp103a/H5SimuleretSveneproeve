using System.ComponentModel.DataAnnotations;

namespace DrocsidApi.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
