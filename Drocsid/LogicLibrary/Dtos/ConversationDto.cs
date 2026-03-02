namespace LogicLibrary.Dtos
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? File { get; set; }
        public string? Reply { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public DateTime Sent { get; set; }
    }
}
