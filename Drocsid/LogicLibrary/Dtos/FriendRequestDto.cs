using System.ComponentModel.DataAnnotations;

namespace LogicLibrary.Dtos
{
    public class FriendRequestDto
    {
        public FriendRequestDto(int id, int senderId, string senderUsername, int reciverId, string reciverUsername)
        {
            Id = id;
            SenderId = senderId;
            SenderUsername = senderUsername;
            ReciverId = reciverId;
            ReciverUsername = reciverUsername;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public string SenderUsername { get; set; }

        [Required]
        public int ReciverId { get; set; }

        [Required]
        public string ReciverUsername { get; set; }
    }
}
