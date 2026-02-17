using DrocsidApi.Models;
using LogicLibrary.Dtos;
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
        Task<List<RefreshTokens>> GetAllRefreshTokens();
        Task<bool> RemoveRefreshToken(Guid refreshTokenId);
        Task<bool> AddRefreshToken(RefreshTokens refreshToken);
        Task<bool> UpdateRefreshToken(Guid TokenId, bool HasBeenUsed);
    }

    internal class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=NogetJegKanHuske;Database=Drocsid";

        public DatabaseManager()
        {
            SetupDatabase().Wait();
        }

        private async Task SetupDatabase()
        {
            if (!await CheckIfUserTableExists())
            {
                await CreateUserTable();
            }

            if (!await CheckIfRefreshTokenTableExists())
            {
                await CreateRefreshTokenTable();
            }

        }

        private async Task<bool> CheckIfUserTableExists()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'users')", conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        private async Task CreateUserTable()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS users (id SERIAL PRIMARY KEY, username TEXT NOT NULL, password TEXT NOT NULL, isActive BOOLEAN NOT NULL)", conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> AddUser(UserModel user)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password, isActive) VALUES (@username, @password, @isActive)", conn))
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
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("UPDATE users SET username = @username, password = @password, isActive = @isActive WHERE id = @id", conn))
            {
                cmd.Parameters.AddWithValue("id", user.Id);
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("isActive", user.IsActive);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("DELETE FROM users WHERE id = @id", conn))
            {
                cmd.Parameters.AddWithValue("id", userId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            List<UserModel> users = new List<UserModel>();
            await using (var cmd = new NpgsqlCommand("SELECT id, username, password, isActive FROM users", conn))
            {
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        IsActive = reader.GetBoolean(3)
                    });
                }
            }
            return users;
        }

        public async Task<UserModel?> GetUser(string username)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("SELECT id, username, password, isActive FROM users WHERE username = @username", conn))
            {
                cmd.Parameters.AddWithValue("username", username);
                var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        IsActive = reader.GetBoolean(3)
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task CreateRefreshTokenTable()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS refreshtokens (id SERIAL PRIMARY KEY, userId TEXT NOT NULL, token TEXT NOT NULL, expires TIMESTAMP NOT NULL, hasBeenUsed BOOLEAN NOT NULL)", conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> CheckIfRefreshTokenTableExists()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'refreshtokens')", conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                return result != null && (bool)result;
            }
        }

        public async Task<List<RefreshTokens>> GetAllRefreshTokens()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            List<RefreshTokens> refreshTokens = new List<RefreshTokens>();
            await using (var cmd = new NpgsqlCommand("SELECT id, userId, token, expires, hasBeenUsed FROM refreshtokens", conn))
            {
                var reader = await cmd.ExecuteReaderAsync();
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
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("DELETE FROM refreshtokens WHERE id = @id", conn))
            {
                cmd.Parameters.AddWithValue("id", refreshTokenId);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> AddRefreshToken(RefreshTokens refreshToken)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("INSERT INTO refreshtokens (id, userId, token, expires, hasBeenUsed) VALUES (@id, @userId, @token, @expires, @hasBeenUsed)", conn))
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
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using (var cmd = new NpgsqlCommand("UPDATE refreshtokens SET hasBeenUsed = @hasBeenUsed WHERE id = @id", conn))
            {
                cmd.Parameters.AddWithValue("id", TokenId);
                cmd.Parameters.AddWithValue("hasBeenUsed", HasBeenUsed);
                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }
}
