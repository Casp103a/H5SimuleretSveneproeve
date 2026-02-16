using DrocsidApi.Models;
using Npgsql;

namespace DrocsidApi.Managers
{
    internal interface IDatabaseManager
    {
        
    }

    internal class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=NogetJegKanHuske;Database=Drocsid"
        private async Task SetupDatabase()
        {
            if (!await CheckIfUserTableExists())
            {
                await CreateUserTable();
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

        public async Task AddUser(UserModel user)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password, isActive) VALUES (@username, @password, @isActive)", conn))
            {
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("isActive", user.IsActive);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
