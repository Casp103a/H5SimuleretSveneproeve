using System.ComponentModel.DataAnnotations;

namespace DrocsidApi.Models
{
    public class ConversationModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public string? File { get; set; }
        public string? Reply { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public DateTime Sent { get; set; }
    }
}
