using LogicLibrary.Dtos;
using DrocsidApi.Models;

namespace DrocsidApi.Managers
{
    public interface IUserManager
    {
        Task<bool?> CreateAsync(UserDto user);
    }

    internal class UserManager : IUserManager
    {
        private readonly IDatabaseManager _databaseManager;

        public UserManager(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<bool?> CreateAsync(UserDto userDto)
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
    }
}
