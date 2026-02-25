using LogicLibrary.Dtos;
using DrocsidApi.Models;

namespace DrocsidApi.Managers
{
    public interface IUserManager
    {
        Task<bool?> CreateAsync(LoginRegisterUserDto user);
        Task<bool?> UpdateAsync(UserModel user);
        Task<bool?> DeleteAsync(UserDto user);
        Task<List<UserModel>> GetAllUsers();
        Task<UserModel?> GetUser(int id);
        Task<UserModel?> GetUser(string username);
        Task<bool> CheckPasswordSignInAsync(LoginRegisterUserDto login, string password);
    }

    internal class UserManager : IUserManager
    {
        private readonly IDatabaseManager databaseManager;

        public UserManager(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public async Task<bool?> CreateAsync(LoginRegisterUserDto userDto)
        {
            try
            {
                if (userDto == null || string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
                {
                    return null;
                }

                var doesUserExist = await databaseManager.GetUser(userDto.Username);
                
                if(doesUserExist != null)
                {
                    return false;
                }

                var userModel = new UserModel
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    IsActive = true
                };

                await databaseManager.AddUser(userModel);

                return true;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool?> UpdateAsync(UserModel? user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                {
                    return null;
                }
                
                await databaseManager.UpdateUser(user);
                return true;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(UserDto user)
        {
            try
            {
                await databaseManager.DeleteUser(user.Id);
                return true;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            try
            {
                return await databaseManager.GetAllUsers();
            }
            catch (Exception)
            {
                return new List<UserModel>();
            }
        }

        public async Task<bool> CheckPasswordSignInAsync(LoginRegisterUserDto login, string password)
        {
            try
            {
                var user = await databaseManager.GetUser(login.Username!);
                if (user == null) return false;
                return user.Password == password;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserModel?> GetUser(int id)
        {
            try
            {
                var user = await databaseManager.GetUser(id);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserModel?> GetUser(string username)
        {
            try
            {
                var user = await databaseManager.GetUser(username);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
