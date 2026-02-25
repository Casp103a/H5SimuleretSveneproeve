using DrocsidApi.Models;
using LogicLibrary.Dtos;

namespace DrocsidApi.Managers
{
    public interface IFriendManager
    {

    }

    public class FriendManager : IFriendManager
    {
        private readonly IDatabaseManager databaseManager;

        public FriendManager(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public async Task<bool> AddFriend(FriendDto friend)
        {
            var model = new FriendModel()
            {
                User1Id = friend.User1Id,
                User2Id = friend.User2Id,
            };

            var result = await databaseManager.AddFriend(model);

            return result;
        }

        public async Task<bool> RemoveFriend(FriendDto friend) 
        {
            bool result;
            var model = await databaseManager.GetFriendById(friend.Id);

            if (model != null)
            {
                result = await databaseManager.RemoveFriend(model);
                return result;
            }

            model = await databaseManager.GetFriendByUserIds(friend.User1Id, friend.User2Id);

            if (model != null) 
            {
                result = await databaseManager.RemoveFriend(model);
                return result;
            }

            model = await databaseManager.GetFriendByUserIds(friend.User2Id, friend.User1Id);
            
            if (model != null)
            {
                result = await databaseManager.RemoveFriend(model);
                return result;
            }

            return false;
        }
    }
}
