namespace Drocsid.Messages.Events
{
    internal class ChatGotNewMessagesEvent : PubSubEvent<ChatGotNewMessageArgs>
    {
    }

    internal class ChatGotNewMessageArgs
    {
        public Guid ChatGuid { get; set; }
        public int ChatId { get; set; }
    }
}
