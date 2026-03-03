using LogicLibrary.Dtos;

namespace DrocsidApi.Managers
{
    public interface IConversationManager
    {
        Task<bool?> SendMessageAsync(ConversationDto message, Guid conversationName);
        Task<bool?> UpdateMessageAsync(ConversationDto message, Guid conversationName);
        Task<bool?> DeleteMessageAsync(int messageId, Guid conversationName);
        Task<List<ConversationDto>?> GetConversationMessagesAsync(Guid conversationName);
        Task<ConversationDto?> GetMessageAsync(int messageId, Guid conversationName);
        Task<bool?> CreateConversationAsync(Guid conversationName);
        Task<bool?> DeleteConversationAsync(Guid conversationName);
        Task<LatestMessageDto?> GetLatestMessageAsync(Guid id, DateTime latestMessageDateTime);
    }

    public class ConversationManager : IConversationManager
    {
        private readonly IDatabaseManager databaseManager;

        public ConversationManager(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public async Task<bool?> SendMessageAsync(ConversationDto message, Guid conversationName)
        {
            try
            {
                if (message == null || conversationName == Guid.Empty)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(message.Message))
                {
                    return null;
                }

                if (message.SenderId <= 0)
                {
                    return null;
                }

                message.Sent = DateTime.Now;

                return await databaseManager.SendMessage(message, conversationName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool?> UpdateMessageAsync(ConversationDto message, Guid conversationName)
        {
            try
            {
                if (message == null || message.Id <= 0 || conversationName == Guid.Empty)
                {
                    return null;
                }

                var existingMessage = await GetMessageAsync(message.Id, conversationName);
                if (existingMessage == null)
                {
                    return false;
                }

                if (existingMessage.SenderId != message.SenderId)
                {
                    return false;
                }

                return await databaseManager.UpdateMessage(message, conversationName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool?> DeleteMessageAsync(int messageId, Guid conversationName)
        {
            try
            {
                if (messageId <= 0 || conversationName == Guid.Empty)
                {
                    return null;
                }

                var existingMessage = await GetMessageAsync(messageId, conversationName);
                if (existingMessage == null)
                {
                    return false;
                }

                return await databaseManager.DeleteMessage(messageId, conversationName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ConversationDto>?> GetConversationMessagesAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    Console.WriteLine("Conversation name is empty");
                    return null;
                }

                var messages = await databaseManager.GetConversationMessages(conversationName);
                await PopulateSenderNames(messages);
                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public async Task<ConversationDto?> GetMessageAsync(int messageId, Guid conversationName)
        {
            try
            {
                if (messageId <= 0 || conversationName == Guid.Empty)
                {
                    return null;
                }

                var message = await databaseManager.GetMessage(messageId, conversationName);
                if (message != null)
                {
                    await PopulateSenderName(message);
                }
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool?> CreateConversationAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return null;
                }

                return await databaseManager.CreateConversation(conversationName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool?> DeleteConversationAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return null;
                }

                return await databaseManager.DeleteConversation(conversationName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LatestMessageDto?> GetLatestMessageAsync(Guid id, DateTime latestMessageDateTime)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return null;
                }

                var latestMessage = await databaseManager.GetLatestMessage(id, latestMessageDateTime);

                return latestMessage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task PopulateSenderNames(List<ConversationDto> messages)
        {
            if (messages == null) return;

            foreach (var message in messages)
            {
                await PopulateSenderName(message);
            }
        }

        private async Task PopulateSenderName(ConversationDto message)
        {
            try
            {
                var user = await databaseManager.GetUser(message.SenderId);
                message.SenderName = user?.Username ?? "Unknown User";
            }
            catch (Exception)
            {
                message.SenderName = "Unknown User";
            }
        }
    }
}
