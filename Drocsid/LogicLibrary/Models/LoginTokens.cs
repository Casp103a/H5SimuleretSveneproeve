using System.ComponentModel.DataAnnotations;

namespace LogicLibrary.Models
{
    public class LoginTokens
    {
        [Required]
        public string? Token { get; set; }
    }
}
