using DrocsidApi.Models;
using LogicLibrary.Dtos;

namespace DrocsidApi.Managers
{
    public interface IFriendRequestManager
    {
        Task<bool> CreateFriendRequest(FriendRequestDto friendRequest);
        Task<bool> RemoveFriendRequest(FriendRequestDto friendRequest);
        Task<List<FriendRequestDto>> GetAllFriendRequests();
        Task<List<FriendRequestDto>> GetAUsersFriendRequest(int userId);
        Task<bool> FriendRequestIsAlreadySent(int senderId, int receiverId);
    }

    public class FriendRequestManger : IFriendRequestManager
    {
        private readonly IDatabaseManager databaseManager;

        public FriendRequestManger(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public Task<bool> CreateFriendRequest(FriendRequestDto friendRequest)
        {
            var friendModel = new FriendRequestModel()
            {
                SenderId = friendRequest.SenderId,
                SenderUsername = friendRequest.SenderUsername,
                ReceiverId = friendRequest.ReceiverId,
                ReceiverUsername = friendRequest.ReceiverUsername
            };

            return databaseManager.AddFriendRequest(friendModel);
        }

        public async Task<bool> RemoveFriendRequest(FriendRequestDto friendRequest)
        {
            var friendModel = new FriendRequestModel()
            {
                Id = friendRequest.Id,
                SenderId = friendRequest.SenderId,
                SenderUsername = friendRequest.SenderUsername,
                ReceiverId = friendRequest.ReceiverId,
                ReceiverUsername = friendRequest.ReceiverUsername
            };

            return await databaseManager.RemoveFriendRequest(friendModel);
        }

        public async Task<List<FriendRequestDto>> GetAllFriendRequests()
        {
            var friendRequests = await databaseManager.GetAllFriendRequests();
            List<FriendRequestDto> result = [];

            foreach (var friendRequest in friendRequests)
            {
                var dto = new FriendRequestDto(friendRequest.Id, friendRequest.SenderId, friendRequest.SenderUsername, friendRequest.ReceiverId, friendRequest.ReceiverUsername);
                result.Add(dto);
            }

            return result;
        }

        public async Task<List<FriendRequestDto>> GetAUsersFriendRequest(int userId)
        {
            var friendRequests = await databaseManager.GetUsersFriendRequests(userId);
            List<FriendRequestDto> result = [];

            foreach (var friendRequest in friendRequests)
            {
                var dto = new FriendRequestDto(friendRequest.Id, friendRequest.SenderId, friendRequest.SenderUsername, friendRequest.ReceiverId, friendRequest.ReceiverUsername);
                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> FriendRequestIsAlreadySent(int senderId, int receiverId)
        {
            var friendRequest = await databaseManager.GetFriendRequest(senderId, receiverId);

            if (friendRequest != null)
                return true;
            else 
                return false;
        }
    }
}
