using LogicLibrary.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LogicLibrary.Models
{
    public class LoginTokens
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        public UserDto? CurrentUser { get; set; }
    }
}
