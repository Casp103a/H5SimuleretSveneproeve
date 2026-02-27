namespace DrocsidApi.Models
{
    public class FriendRequestModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverUsername { get; set; }
    }
}
