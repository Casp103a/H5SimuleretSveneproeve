using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LogicLibrary.Dtos
{
    public class FriendRequestDto
    {
        [JsonConstructor]
        private FriendRequestDto()
        {
            
        }

        public FriendRequestDto(int senderId, string senderUsername, int receiverId, string receiverUsername)
        {
            SenderId = senderId;
            SenderUsername = senderUsername;
            ReceiverId = receiverId;
            ReceiverUsername = receiverUsername;
        }

        public FriendRequestDto(int id, int senderId, string senderUsername, int receiverId, string receiverUsername)
        {
            Id = id;
            SenderId = senderId;
            SenderUsername = senderUsername;
            ReceiverId = receiverId;
            ReceiverUsername = receiverUsername;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public string SenderUsername { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string ReceiverUsername { get; set; }
    }

    public class AddFriendRequestDto
    {
        [Required]
        public int SenderId { get; set; }

        [Required]
        public string? ReceiverUsername { get; set; }
    }
}
