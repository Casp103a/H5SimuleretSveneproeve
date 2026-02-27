using DrocsidApi.Models;
using LogicLibrary.Dtos;

namespace DrocsidApi.Managers
{
    public interface IFriendManager
    {
        Task<bool> AddFriend(FriendDto friend);
        Task<bool> RemoveFriend(FriendDto friend);
        Task<bool> IsAlreadyFriends(int user1Id, int user2Id);
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

        public async Task<bool> IsAlreadyFriends(int user1Id, int user2Id)
        {
            var friendship = await databaseManager.GetFriendByUserIds(user1Id, user2Id);

            if (friendship != null) 
            {
                return true;
            }

            friendship = await databaseManager.GetFriendByUserIds(user2Id, user1Id);

            if (friendship != null) 
            {
                return true;
            }

            return false;
        }
    }
}
