using DrocsidApi.Models;
using Npgsql;

namespace DrocsidApi.Managers
{
    public interface IDatabaseManager
    {
        Task<bool> AddUser(UserModel user);
        Task<bool> UpdateUser(UserModel user);
        Task<bool> DeleteUser(int userId);
        Task<List<UserModel>> GetAllUsers();
        Task<UserModel?> GetUser(string username);
        Task<UserModel?> GetUser(int id);

        Task<List<RefreshTokens>> GetAllRefreshTokens();
        Task<bool> RemoveRefreshToken(Guid refreshTokenId);
        Task<bool> AddRefreshToken(RefreshTokens refreshToken);
        Task<bool> UpdateRefreshToken(Guid TokenId, bool HasBeenUsed);

        Task<bool> AddFriendRequest(FriendRequestModel friendRequest);
        Task<bool> RemoveFriendRequest(FriendRequestModel friendRequest);
        Task<List<FriendRequestModel>> GetAllFriendRequests();
        Task<List<FriendRequestModel>> GetUsersFriendRequests(int userId);
        Task<FriendRequestModel?> GetFriendRequest(int senderId, int receiverId);

        Task<bool> AddFriend(FriendModel friendModel);
        Task<bool> RemoveFriend(FriendModel friendModel);
        Task<FriendModel?> GetFriendByUserIds(int user1Id, int user2Id);
        Task<FriendModel?> GetFriendById(int id);
        Task<List<FriendModel>> FindUsersFriends(int userId);

        Task<bool> SendMessage(ConversationModel message, Guid conversationName);
        Task<bool> UpdateMessage(ConversationModel message, Guid conversationName);
        Task<bool> DeleteMessage(int messageId, Guid conversationName);
        Task<List<ConversationModel>> GetConversationMessages(Guid conversationName);
        Task<ConversationModel?> GetMessage(int messageId, Guid conversationName);
        Task<bool> CreateConversation(Guid conversationName);
        Task<bool> DeleteConversation(Guid conversationName);
        Task<List<ConversationModel>> GetLatestMessage(Guid conversationName, DateTime latestMessageDateTime);
        Task<bool> IsThisLatestMessage(Guid conversationName, int id);

        Task<bool> AddUsersInChatJunc(UsersInChatJuncModel chatJunc);
        Task<bool> UpdateUsersInChatJunc(UsersInChatJuncModel chatJunc);
        Task<bool> DeleteUsersInChatJunc(int id);
        Task<List<UsersInChatJuncModel>> GetAllUsersInChatJunc();
        Task<UsersInChatJuncModel?> GetUsersInChatJunc(int id);
        Task<UsersInChatJuncModel?> GetUsersInChatJuncByName(Guid conversationName);
        Task<List<UsersInChatJuncModel>> GetUserChats(int userId);
    }

    internal class DatabaseManager : IDatabaseManager, IDisposable
    {
        private readonly string connectionString;
        private readonly NpgsqlDataSource dataSource;
        private readonly IConfiguration configurationManager;

        public DatabaseManager(IConfiguration configurationManager)
        {
            this.configurationManager = configurationManager;
            var conn = this.configurationManager.GetConnectionString("connectionString");
            if (conn != null)
                connectionString = conn;

            if (connectionString != null)
                dataSource = NpgsqlDataSource.Create(connectionString);
            else
                return;
            SetupDatabase().Wait();
        }

        private async Task SetupDatabase()
        {
            if (!await CheckIfUserTableExists())
                await CreateUserTable();

            if (!await CheckIfRefreshTokenTableExists())
                await CreateRefreshTokenTable();

            if (!await CheckIfFriendRequestTableExists())
                await CreateFriendRequestTable();

            if (!await CheckIfFriendsTableExists())
                await CreateFriendsTable();

            if (!await CheckIfUsersInChatJuncTableExists())
                await CreateUsersInChatJuncTable();
        }

        #region users

        private async Task<bool> CheckIfUserTableExists()
        {
            await using (var cmd = dataSource.CreateCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'users')"))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        private async Task CreateUserTable()
        {
            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS users (id SERIAL PRIMARY KEY, username TEXT NOT NULL, password TEXT NOT NULL, isActive BOOLEAN NOT NULL, refreshTokenId TEXT)"))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> AddUser(UserModel user)
        {
            await using (var cmd = dataSource.CreateCommand("INSERT INTO users (username, password, isActive) VALUES (@username, @password, @isActive)"))
            {
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("isActive", user.IsActive);
                var result = await cmd.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> UpdateUser(UserModel user)
        {
            await using (var cmd = dataSource.CreateCommand("UPDATE users SET username = @username, password = @password, isActive = @isActive, refreshTokenId = @refreshTokenId WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", user.Id);
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("isActive", user.IsActive);
                cmd.Parameters.AddWithValue("refreshTokenId", user.RefreshTokenId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            await using (var cmd = dataSource.CreateCommand("DELETE FROM users WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", userId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();

            await using (var cmd = dataSource.CreateCommand("SELECT id, username, password, isActive, refreshTokenId FROM users"))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        IsActive = reader.GetBoolean(3),
                        RefreshTokenId = reader.GetString(4),
                    });
                }
            }
            return users;
        }

        public async Task<UserModel?> GetUser(string username)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, username, password, isActive, refreshTokenId FROM users WHERE username = @username"))
            {
                cmd.Parameters.AddWithValue("username", username);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        IsActive = reader.GetBoolean(3),
                        RefreshTokenId = reader.IsDBNull(4) ? null : reader.GetString(4),
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<UserModel?> GetUser(int id)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, username, password, isActive, refreshTokenId FROM users WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", id);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        IsActive = reader.GetBoolean(3),
                        RefreshTokenId = reader.GetString(4)
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region refreshTokens

        private async Task CreateRefreshTokenTable()
        {
            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS refreshtokens (id uuid PRIMARY KEY, userId INT NOT NULL REFERENCES users(id), token TEXT NOT NULL, expires TIMESTAMP NOT NULL, hasBeenUsed BOOLEAN NOT NULL)"))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> CheckIfRefreshTokenTableExists()
        {
            await using (var cmd = dataSource.CreateCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'refreshtokens')"))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        public async Task<List<RefreshTokens>> GetAllRefreshTokens()
        {
            List<RefreshTokens> refreshTokens = new List<RefreshTokens>();

            await using (var cmd = dataSource.CreateCommand("SELECT id, userId, token, expires, hasBeenUsed FROM refreshtokens"))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    refreshTokens.Add(new RefreshTokens
                    {
                        ID = reader.GetGuid(0),
                        RefreshToken = reader.GetString(2),
                        RefreshTokenExpiry = reader.GetDateTime(3),
                        RefreshTokenIsUsed = reader.GetBoolean(4),
                        UserId = reader.GetInt32(1)
                    });
                }
            }
            return refreshTokens;
        }

        public async Task<bool> RemoveRefreshToken(Guid refreshTokenId)
        {
            await using (var cmd = dataSource.CreateCommand("DELETE FROM refreshtokens WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", refreshTokenId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> AddRefreshToken(RefreshTokens refreshToken)
        {
            await using (var cmd = dataSource.CreateCommand("INSERT INTO refreshtokens (id, userId, token, expires, hasBeenUsed) VALUES (@id, @userId, @token, @expires, @hasBeenUsed)"))
            {
                cmd.Parameters.AddWithValue("id", refreshToken.ID);
                cmd.Parameters.AddWithValue("userId", refreshToken.UserId);
                cmd.Parameters.AddWithValue("token", refreshToken.RefreshToken);
                cmd.Parameters.AddWithValue("expires", refreshToken.RefreshTokenExpiry);
                cmd.Parameters.AddWithValue("hasBeenUsed", refreshToken.RefreshTokenIsUsed);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> UpdateRefreshToken(Guid TokenId, bool HasBeenUsed)
        {
            await using (var cmd = dataSource.CreateCommand("UPDATE refreshtokens SET hasBeenUsed = @hasBeenUsed WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", TokenId);
                cmd.Parameters.AddWithValue("hasBeenUsed", HasBeenUsed);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        #endregion

        #region friendRequest
        private async Task<bool> CheckIfFriendRequestTableExists()
        {
            await using (var cmd = dataSource.CreateCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'friendrequests')"))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        private async Task CreateFriendRequestTable()
        {
            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS friendrequests (id SERIAL PRIMARY KEY, senderId INT NOT NULL REFERENCES users(id), receiverId INT NOT NULL REFERENCES users(id))"))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> AddFriendRequest(FriendRequestModel friendRequest)
        {
            await using (var cmd = dataSource.CreateCommand("INSERT INTO friendrequests (senderId, receiverId) VALUES (@senderId, @receiverId)"))
            {
                cmd.Parameters.AddWithValue("senderId", friendRequest.SenderId);
                cmd.Parameters.AddWithValue("receiverId", friendRequest.ReceiverId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> RemoveFriendRequest(FriendRequestModel friendRequest)
        {
            await using (var cmd = dataSource.CreateCommand("DELETE FROM friendrequests WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", friendRequest.Id);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<FriendRequestModel>> GetAllFriendRequests()
        {
            List<FriendRequestModel> friendRequests = [];

            await using (var cmd = dataSource.CreateCommand("SELECT id, senderId, receiverId FROM friendrequests"))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    friendRequests.Add(new FriendRequestModel
                    {
                        Id = reader.GetInt32(0),
                        SenderId = reader.GetInt32(1),
                        ReceiverId = reader.GetInt32(2),
                    });
                }
            }
            return friendRequests;
        }

        public async Task<List<FriendRequestModel>> GetUsersFriendRequests(int userId)
        {
            List<FriendRequestModel> friendRequests = [];

            await using (var cmd = dataSource.CreateCommand("SELECT id, senderId, receiverId FROM friendrequests WHERE receiverId = @id"))
            {
                cmd.Parameters.AddWithValue("id", userId);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    friendRequests.Add(new FriendRequestModel
                    {
                        Id = reader.GetInt32(0),
                        SenderId = reader.GetInt32(1),
                        ReceiverId = reader.GetInt32(2),
                    });
                }
            }
            return friendRequests;
        }

        public async Task<FriendRequestModel?> GetFriendRequest(int senderId, int receiverId)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, senderId, receiverId FROM friendrequests WHERE senderId = @senderId AND receiverId = @receiverId"))
            {
                cmd.Parameters.AddWithValue("senderId", senderId);
                cmd.Parameters.AddWithValue("receiverId", receiverId);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    return new FriendRequestModel
                    {
                        Id = reader.GetInt32(0),
                        SenderId = reader.GetInt32(1),
                        ReceiverId = reader.GetInt32(2),
                    };
                }
            }
            return null;
        }
        #endregion

        #region friends
        private async Task<bool> CheckIfFriendsTableExists()
        {
            await using (var cmd = dataSource.CreateCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'friends')"))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        private async Task CreateFriendsTable()
        {
            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS friends (id SERIAL PRIMARY KEY, user1Id INT NOT NULL REFERENCES users(id), user2Id INT NOT NULL REFERENCES users(id))"))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> AddFriend(FriendModel friendModel)
        {
            await using (var cmd = dataSource.CreateCommand("INSERT INTO friends (user1Id, user2Id) VALUES (@user1Id, @user2Id)"))
            {
                cmd.Parameters.AddWithValue("user1Id", friendModel.User1Id);
                cmd.Parameters.AddWithValue("user2Id", friendModel.User2Id);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> RemoveFriend(FriendModel friendModel)
        {
            await using (var cmd = dataSource.CreateCommand("DELETE FROM friends WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", friendModel.Id);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<FriendModel?> GetFriendByUserIds(int user1Id, int user2Id)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, user1Id, user2Id FROM friends WHERE user1Id = @user1Id AND user2Id = @user2Id"))
            {
                cmd.Parameters.AddWithValue("user1Id", user1Id);
                cmd.Parameters.AddWithValue("user2Id", user2Id);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    return new FriendModel
                    {
                        Id = reader.GetInt32(0),
                        User1Id = reader.GetInt32(1),
                        User2Id = reader.GetInt32(2)
                    };
                }
            }

            return null;
        }

        public async Task<FriendModel?> GetFriendById(int id)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, user1Id, user2Id FROM friends WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", id);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    return new FriendModel
                    {
                        Id = reader.GetInt32(0),
                        User1Id = reader.GetInt32(1),
                        User2Id = reader.GetInt32(2)
                    };
                }
            }

            return null;
        }

        public async Task<List<FriendModel>> FindUsersFriends(int userId)
        {
            List<FriendModel> friends = [];

            await using (var cmd = dataSource.CreateCommand("SELECT id, user1Id, user2Id FROM friends WHERE user1Id = @id"))
            {
                cmd.Parameters.AddWithValue("id", userId);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    friends.Add(new FriendModel
                    {
                        Id = reader.GetInt32(0),
                        User1Id = reader.GetInt32(1),
                        User2Id = reader.GetInt32(2)
                    });
                }
            }

            await using (var cmd = dataSource.CreateCommand("SELECT id, user1Id, user2Id FROM friends WHERE user2Id = @id"))
            {
                cmd.Parameters.AddWithValue("id", userId);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    friends.Add(new FriendModel
                    {
                        Id = reader.GetInt32(0),
                        User1Id = reader.GetInt32(1),
                        User2Id = reader.GetInt32(2)
                    });
                }
            }

            return friends;
        }
        #endregion

        #region conversation

        private async Task<bool> CheckIfConversationTableExists(Guid conversationName)
        {
            await using (var cmd = dataSource.CreateCommand($"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = '{conversationName}')"))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        private async Task CreateConversationTable(Guid conversationName)
        {
            await using (var cmd = dataSource.CreateCommand($"CREATE TABLE IF NOT EXISTS \"{conversationName}\" (id SERIAL PRIMARY KEY, message TEXT, file TEXT, reply TEXT, senderid INT REFERENCES users(id), sent TIMESTAMP)"))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task CheckAndCreateConversation(Guid conversationName)
        {
            if (!await CheckIfConversationTableExists(conversationName))
                await CreateConversationTable(conversationName);
        }

        public async Task<bool> SendMessage(ConversationModel message, Guid conversationName)
        {
            await CheckAndCreateConversation(conversationName);

            await using (var cmd = dataSource.CreateCommand($"INSERT INTO \"{conversationName}\" (message, file, reply, senderid, sent) VALUES (@message, @file, @reply, @senderid, @sent)"))
            {
                cmd.Parameters.AddWithValue("message", message.Message ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("file", message.File ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("reply", message.Reply ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("senderid", message.SenderId);
                cmd.Parameters.AddWithValue("sent", message.Sent);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> UpdateMessage(ConversationModel message, Guid conversationName)
        {
            await CheckAndCreateConversation(conversationName);

            await using (var cmd = dataSource.CreateCommand($"UPDATE \"{conversationName}\" SET message = @message, file = @file, reply = @reply WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", message.Id);
                cmd.Parameters.AddWithValue("message", message.Message ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("file", message.File ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("reply", message.Reply ?? (object)DBNull.Value);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteMessage(int messageId, Guid conversationName)
        {
            await CheckAndCreateConversation(conversationName);

            await using (var cmd = dataSource.CreateCommand($"DELETE FROM \"{conversationName}\" WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", messageId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<ConversationModel>> GetConversationMessages(Guid conversationName)
        {
            await CheckAndCreateConversation(conversationName);
            List<ConversationModel> messages = [];

            await using (var cmd = dataSource.CreateCommand($"SELECT id, message, file, reply, senderid, sent FROM \"{conversationName}\" ORDER BY sent ASC"))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    messages.Add(new ConversationModel
                    {
                        Id = reader.GetInt32(0),
                        Message = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        File = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
                        SenderId = reader.GetInt32(4),
                        Sent = reader.GetDateTime(5)
                    });
                }
            }
            return messages;
        }

        public async Task<ConversationModel?> GetMessage(int messageId, Guid conversationName)
        {
            await CheckAndCreateConversation(conversationName);

            await using (var cmd = dataSource.CreateCommand($"SELECT id, message, file, reply, senderid, sent FROM \"{conversationName}\" WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", messageId);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ConversationModel
                    {
                        Id = reader.GetInt32(0),
                        Message = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        File = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
                        SenderId = reader.GetInt32(4),
                        Sent = reader.GetDateTime(5)
                    };
                }
            }
            return null;
        }

        public async Task<bool> CreateConversation(Guid conversationName)
        {
            await using (var cmd = dataSource.CreateCommand($"CREATE TABLE IF NOT EXISTS \"{conversationName}\" (id SERIAL PRIMARY KEY, message TEXT, file TEXT, reply TEXT, senderid INT REFERENCES users(id), sent TIMESTAMP)"))
            {
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> DeleteConversation(Guid conversationName)
        {
            await using (var cmd = dataSource.CreateCommand($"DROP TABLE IF EXISTS \"{conversationName}\""))
            {
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<List<ConversationModel>> GetLatestMessage(Guid conversationName, DateTime latestMessageDateTime)
        {
            await using (var cmd = dataSource.CreateCommand($"SELECT id, message, file, reply, senderid, sent FROM \"{conversationName}\" WHERE sent > @latestMessageDateTime"))
            {
                var messages = new List<ConversationModel>();
                cmd.Parameters.AddWithValue("latestMessageDateTime", latestMessageDateTime);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    messages.Add(new ConversationModel
                    {
                        Id = reader.GetInt32(0),
                        Message = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        File = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
                        SenderId = reader.GetInt32(4),
                        Sent = reader.GetDateTime(5)
                    });
                }
                return messages;
            }
        }

        public async Task<bool> IsThisLatestMessage(Guid conversationName, int id)
        {
            await using (var cmd = dataSource.CreateCommand($"SELECT id, message, file, reply, senderid, sent FROM \"{conversationName}\" ORDER BY id DESC LIMIT 1"))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var model = new ConversationModel
                    {
                        Id = reader.GetInt32(0),
                        Message = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        File = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
                        SenderId = reader.GetInt32(4),
                        Sent = reader.GetDateTime(5)
                    };

                    if (model.Id < id)
                        return false;
                    else
                        return true;
                }

                return true;
            }
        }
        #endregion

        #region usersInChatJunc

        private async Task<bool> CheckIfUsersInChatJuncTableExists()
        {
            await using (var cmd = dataSource.CreateCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'users_in_chat_junc')"))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        private async Task CreateUsersInChatJuncTable()
        {
            await using (var cmd = dataSource.CreateCommand("CREATE TABLE IF NOT EXISTS users_in_chat_junc (id SERIAL PRIMARY KEY, userids INT[] NOT NULL, conversationname uuid NOT NULL, conversationdisplayname TEXT NOT NULL)"))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> AddUsersInChatJunc(UsersInChatJuncModel chatJunc)
        {
            await using (var cmd = dataSource.CreateCommand("INSERT INTO users_in_chat_junc (userids, conversationname, conversationdisplayname) VALUES (@userids, @conversationname, @conversationdisplayname)"))
            {
                cmd.Parameters.AddWithValue("userids", chatJunc.UserIds.ToArray());
                cmd.Parameters.AddWithValue("conversationname", chatJunc.ConversationName);
                cmd.Parameters.AddWithValue("conversationdisplayname", chatJunc.ConversationDisplayName);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> UpdateUsersInChatJunc(UsersInChatJuncModel chatJunc)
        {
            await using (var cmd = dataSource.CreateCommand("UPDATE users_in_chat_junc SET userids = @userids, conversationdisplayname = @conversationdisplayname WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", chatJunc.Id);
                cmd.Parameters.AddWithValue("userids", chatJunc.UserIds.ToArray());
                cmd.Parameters.AddWithValue("conversationdisplayname", chatJunc.ConversationDisplayName);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteUsersInChatJunc(int id)
        {
            await using (var cmd = dataSource.CreateCommand("DELETE FROM users_in_chat_junc WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", id);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<UsersInChatJuncModel>> GetAllUsersInChatJunc()
        {
            List<UsersInChatJuncModel> chatJuncs = [];

            await using (var cmd = dataSource.CreateCommand("SELECT id, userids, conversationname, conversationdisplayname FROM users_in_chat_junc"))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    chatJuncs.Add(new UsersInChatJuncModel
                    {
                        Id = reader.GetInt32(0),
                        UserIds = ((int[])reader.GetValue(1)).ToList(),
                        ConversationName = reader.GetGuid(2),
                        ConversationDisplayName = reader.GetString(3)
                    });
                }
            }
            return chatJuncs;
        }

        public async Task<UsersInChatJuncModel?> GetUsersInChatJunc(int id)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, userids, conversationname, conversationdisplayname FROM users_in_chat_junc WHERE id = @id"))
            {
                cmd.Parameters.AddWithValue("id", id);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UsersInChatJuncModel
                    {
                        Id = reader.GetInt32(0),
                        UserIds = ((int[])reader.GetValue(1)).ToList(),
                        ConversationName = reader.GetGuid(2),
                        ConversationDisplayName = reader.GetString(3)
                    };
                }
            }
            return null;
        }

        public async Task<UsersInChatJuncModel?> GetUsersInChatJuncByName(Guid conversationName)
        {
            await using (var cmd = dataSource.CreateCommand("SELECT id, userids, conversationname, conversationdisplayname FROM users_in_chat_junc WHERE conversationname = @conversationname"))
            {
                cmd.Parameters.AddWithValue("conversationname", conversationName);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UsersInChatJuncModel
                    {
                        Id = reader.GetInt32(0),
                        UserIds = ((int[])reader.GetValue(1)).ToList(),
                        ConversationName = reader.GetGuid(2),
                        ConversationDisplayName = reader.GetString(3)
                    };
                }
            }
            return null;
        }

        public async Task<List<UsersInChatJuncModel>> GetUserChats(int userId)
        {
            List<UsersInChatJuncModel> chatJuncs = [];

            await using (var cmd = dataSource.CreateCommand("SELECT id, userids, conversationname, conversationdisplayname FROM users_in_chat_junc WHERE @userId = ANY(userids)"))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    chatJuncs.Add(new UsersInChatJuncModel
                    {
                        Id = reader.GetInt32(0),
                        UserIds = ((int[])reader.GetValue(1)).ToList(),
                        ConversationName = reader.GetGuid(2),
                        ConversationDisplayName = reader.GetString(3)
                    });
                }
            }
            return chatJuncs;
        }
        #endregion 

        public void Dispose()
        {
            dataSource?.Dispose();
        }
    }
}
