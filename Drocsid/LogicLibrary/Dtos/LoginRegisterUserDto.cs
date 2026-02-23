using System.ComponentModel.DataAnnotations;

namespace LogicLibrary.Dtos
{
    public class LoginRegisterUserDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [MinLength(8)]
        public string? Password { get; set; }
    }
}
