using System.ComponentModel.DataAnnotations;

namespace DrocsidApi.Models
{
    public class UsersInChatJuncModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public List<int> UserIds { get; set; }
        [Required]
        public Guid ConversationName { get; set; }
        [Required]
        public string ConversationDisplayName { get; set; }
    }
}
