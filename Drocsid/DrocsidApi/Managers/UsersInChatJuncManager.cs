using LogicLibrary.Dtos;

namespace DrocsidApi.Managers
{
    public interface IUsersInChatJuncManager
    {
        Task<bool> CreateChatAsync(UsersInChatJuncDto chatJunc);
        Task<bool> UpdateChatAsync(UsersInChatJuncDto chatJunc);
        Task<bool> DeleteChatAsync(int id);
        Task<bool> AddUserToChatAsync(int userId, Guid conversationName);
        Task<bool> RemoveUserFromChatAsync(int userId, Guid conversationName);
        Task<List<UsersInChatJuncDto>?> GetAllChatsAsync();
        Task<UsersInChatJuncDto?> GetChatAsync(int id);
        Task<UsersInChatJuncDto?> GetChatByNameAsync(Guid conversationName);
        Task<List<UsersInChatJuncDto>?> GetUserChatsAsync(int userId);
        Task<List<int>?> GetChatParticipantsAsync(Guid conversationName);
    }

    public class UsersInChatJuncManager : IUsersInChatJuncManager
    {
        private readonly IDatabaseManager databaseManager;

        public UsersInChatJuncManager(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public async Task<bool> CreateChatAsync(UsersInChatJuncDto chatJunc)
        {
            try
            {
                if (chatJunc == null || chatJunc.UserIds == null || !chatJunc.UserIds.Any())
                {
                    return false;
                }

                var existingChat = await GetChatByNameAsync(chatJunc.ConversationName);
                if (existingChat != null)
                {
                    return false;
                }

                return await databaseManager.AddUsersInChatJunc(chatJunc);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateChatAsync(UsersInChatJuncDto chatJunc)
        {
            try
            {
                if (chatJunc == null || chatJunc.Id <= 0)
                {
                    return false;
                }

                var existingChat = await GetChatAsync(chatJunc.Id);
                if (existingChat == null)
                {
                    return false;
                }

                return await databaseManager.UpdateUsersInChatJunc(chatJunc);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteChatAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }

                var existingChat = await GetChatAsync(id);
                if (existingChat == null)
                {
                    return false;
                }

                return await databaseManager.DeleteUsersInChatJunc(id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddUserToChatAsync(int userId, Guid conversationName)
        {
            try
            {
                if (userId <= 0 || conversationName == Guid.Empty)
                {
                    return false;
                }

                var chat = await GetChatByNameAsync(conversationName);
                if (chat == null)
                {
                    return false;
                }

                if (chat.UserIds.Contains(userId))
                {
                    return false;
                }

                chat.UserIds.Add(userId);
                return await UpdateChatAsync(chat);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveUserFromChatAsync(int userId, Guid conversationName)
        {
            try
            {
                if (userId <= 0 || conversationName == Guid.Empty)
                {
                    return false;
                }

                var chat = await GetChatByNameAsync(conversationName);
                if (chat == null)
                {
                    return false;
                }

                if (!chat.UserIds.Contains(userId))
                {
                    return false;
                }

                chat.UserIds.Remove(userId);
                return await UpdateChatAsync(chat);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<UsersInChatJuncDto>?> GetAllChatsAsync()
        {
            try
            {
                var result = await databaseManager.GetAllUsersInChatJunc();

                if (result == null)
                    return null;

                var chats = new List<UsersInChatJuncDto>();

                foreach (var chat in result)
                {
                    chats.Add(new UsersInChatJuncDto
                    {
                        Id = chat.Id,
                        ConversationName = chat.ConversationName,
                        UserIds = chat.UserIds
                    });
                }

                return chats;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UsersInChatJuncDto?> GetChatAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null;
                }

                var result = await databaseManager.GetUsersInChatJunc(id);

                if (result == null)
                    return null;

                var chat = new UsersInChatJuncDto()
                {
                    Id = result.Id,
                    ConversationName = result.ConversationName,
                    UserIds = result.UserIds
                };

                return chat;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UsersInChatJuncDto?> GetChatByNameAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return null;
                }

                return await databaseManager.GetUsersInChatJuncByName(conversationName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<UsersInChatJuncDto>?> GetUserChatsAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return null;
                }

                var chats = await databaseManager.GetUserChats(userId);

                if (chats == null)
                    return null;

                var convertedChats = new List<UsersInChatJuncDto>();

                foreach (var chat in chats)
                {
                    convertedChats.Add(new UsersInChatJuncDto
                    {
                        Id = chat.Id,
                        ConversationName = chat.ConversationName,
                        UserIds = chat.UserIds
                    });
                }

                return convertedChats;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<int>?> GetChatParticipantsAsync(Guid conversationName)
        {
            try
            {
                if (conversationName == Guid.Empty)
                {
                    return null;
                }

                var chat = await GetChatByNameAsync(conversationName);
                return chat?.UserIds;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
