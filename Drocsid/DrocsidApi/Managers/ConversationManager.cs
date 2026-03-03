using DrocsidApi.Models;
using LogicLibrary.Dtos;

namespace DrocsidApi.Managers
{
    public interface IConversationManager
    {
        Task<bool> SendMessageAsync(ConversationDto message, Guid conversationName);
        Task<bool> UpdateMessageAsync(ConversationDto message, Guid conversationName);
        Task<bool> DeleteMessageAsync(int messageId, Guid conversationName);
        Task<List<ConversationDto>?> GetConversationMessagesAsync(Guid conversationName);
        Task<ConversationDto?> GetMessageAsync(int messageId, Guid conversationName);
        Task<bool> CreateConversationAsync(Guid conversationName);
        Task<bool> DeleteConversationAsync(Guid conversationName);
        Task<List<ConversationDto>?> GetLatestMessagesAsync(Guid conversationName, DateTime latestMessageDateTime);
        Task<bool> IsThisLatestMessageAsync(Guid conversationName, int id);
    }

    public class ConversationManager : IConversationManager
    {
        private readonly IDatabaseManager databaseManager;

        public ConversationManager(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public async Task<bool> SendMessageAsync(ConversationDto message, Guid conversationName)
        {
            try
            {
                if (message == null || conversationName == Guid.Empty)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(message.Message))
                {
                    return false;
                }

                if (message.SenderId <= 0)
                {
                    return false;
                }

                message.Sent = DateTime.Now;

                return await databaseManager.SendMessage(message, conversationName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateMessageAsync(ConversationDto message, Guid conversationName)
        {
            try
            {
                if (message == null || message.Id <= 0 || conversationName == Guid.Empty)
                {
                    return false;
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
                return false;
            }
        }

        public async Task<bool> DeleteMessageAsync(int messageId, Guid conversationName)
        {
            try
            {
                if (messageId <= 0 || conversationName == Guid.Empty)
                {
                    return false;
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
                return false;
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

                var convertedMessages = new List<ConversationDto>();
                foreach (var message in messages)
                {
                    convertedMessages.Add(new ConversationDto
                    {
                        Id = message.Id,
                        Message = message.Message,
                        File = message.File,
                        Reply = message.Reply,
                        SenderId = message.SenderId,
                        SenderName = message.SenderName,
                        Sent = message.Sent
                    });
                }

                return convertedMessages;
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
                
                var convertedMessage = new ConversationDto
                {
                    Id = message.Id,
                    Message = message.Message,
                    File = message.File,
                    Reply = message.Reply,
                    SenderId = message.SenderId,
                    SenderName = message.SenderName,
                    Sent = message.Sent
                };

                return convertedMessage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateConversationAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return false;
                }

                return await databaseManager.CreateConversation(conversationName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteConversationAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return false;
                }

                return await databaseManager.DeleteConversation(conversationName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<ConversationDto>?> GetLatestMessagesAsync(Guid conversationName, DateTime latestMessageDateTime)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return null;
                }

                var latestMessage = await databaseManager.GetLatestMessage(conversationName, latestMessageDateTime);

                var convertedMessages = new List<ConversationDto>();
                foreach (var message in latestMessage)
                {
                    convertedMessages.Add(new ConversationDto
                    {
                        Id = message.Id,
                        Message = message.Message,
                        File = message.File,
                        Reply = message.Reply,
                        SenderId = message.SenderId,
                        SenderName = message.SenderName,
                        Sent = message.Sent
                    });
                }

                return convertedMessages;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> IsThisLatestMessageAsync(Guid conversationName, int id)
        {
            try
            {
                return await databaseManager.IsThisLatestMessage(conversationName, id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task PopulateSenderNames(List<ConversationModel> messages)
        {
            if (messages == null) return;

            foreach (var message in messages)
            {
                await PopulateSenderName(message);
            }
        }

        private async Task PopulateSenderName(ConversationModel message)
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
