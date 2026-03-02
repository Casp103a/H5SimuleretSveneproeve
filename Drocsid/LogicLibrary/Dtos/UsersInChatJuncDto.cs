namespace LogicLibrary.Dtos
{
    public class UsersInChatJuncDto
    {
        public int Id { get; set; }
        public List<int> UserIds { get; set; }
        public int ConversationId { get; set; }

    }
}
