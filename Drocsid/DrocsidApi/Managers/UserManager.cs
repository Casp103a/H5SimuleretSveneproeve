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
        Task<bool> CheckPasswordSignInAsync(LoginRegisterUserDto login, string password);
    }

    internal class UserManager : IUserManager
    {
        private readonly IDatabaseManager _databaseManager;

        public UserManager(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<bool?> CreateAsync(LoginRegisterUserDto userDto)
        {
            try
            {
                if (userDto == null || string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
                {
                    return null;
                }

                var userModel = new UserModel
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    IsActive = true
                };

                await _databaseManager.AddUser(userModel);

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
                
                await _databaseManager.UpdateUser(user);
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
                await _databaseManager.DeleteUser(user.Id);
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
                return await _databaseManager.GetAllUsers();
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
                var user = await _databaseManager.GetUser(login.Username);
                if (user == null) return false;
                return user.Password == password;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
