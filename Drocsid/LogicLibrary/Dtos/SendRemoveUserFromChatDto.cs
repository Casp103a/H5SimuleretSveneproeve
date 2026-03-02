namespace LogicLibrary.Dtos
{
    public class SendRemoveUserFromChatDto
    {
        public int UserId { get; set; }
        public UsersInChatJuncDto? Junc { get; set; }
    }
}
