using LogicLibrary.Dtos;

namespace Drocsid.Messages.Query
{
    internal class GetLatestMessageQuery : PubSubEvent<GetLatestMessageQueryArgs>
    {
    }

    internal class GetLatestMessageQueryArgs
    {
        public Guid ConversationName { get; set; }
        public int ChatId { get; set; }
        public ConversationDto Message { get; set; }
    }
}
