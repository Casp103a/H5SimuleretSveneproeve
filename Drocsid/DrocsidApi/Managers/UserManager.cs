using DrocsidApi.Models;

namespace DrocsidApi.Managers
{
    public interface IUserManager
    {
        Task<UserModel> CreateAsync(UserModel user, string password);
    }

    internal class UserManager : IUserManager
    {
        public async Task<UserModel?> CreateAsync(UserModel userModel, string password)
        {
            try
            {
                var user = new UserModel
                {
                    Username = userModel.Username,
                    Password = password,
                    IsActive = true
                };

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
