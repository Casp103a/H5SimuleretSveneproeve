using DrocsidApi.Managers;
using LogicLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DrocsidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly IFriendManager friendManager;
        private readonly IFriendRequestManager friendRequestManager;
        private readonly IUserManager userManager;
        private readonly IUsersInChatJuncManager usersInChatJuncManager;

        public FriendController(IFriendManager friendManager, IFriendRequestManager friendRequestManager, IUserManager userManager, IUsersInChatJuncManager usersInChatJuncManager)
        {
            this.friendManager = friendManager;
            this.friendRequestManager = friendRequestManager;
            this.userManager = userManager;
            this.usersInChatJuncManager = usersInChatJuncManager;
        }

        [HttpGet("GetUserFriendRequest")]
        public async Task<IActionResult> GetUserFriendRequest([FromQuery] int userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = new List<FriendRequestDto>();

            try
            {
                result = await friendRequestManager.GetAUsersFriendRequest(userId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

        [HttpPost("Accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] FriendRequestDto friendRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var friendDto = new FriendDto()
            {
                User1Id = friendRequest.SenderId,
                User2Id = friendRequest.ReceiverId,
            };

            var result = await friendManager.AddFriend(friendDto);

            if (result)
            {
                result = await friendRequestManager.RemoveFriendRequest(friendRequest);
                
                if (!result)
                    return Ok("Failed while trying to clean up friend request");

                var junc = new UsersInChatJuncDto()
                {
                    ConversationDisplayName = "chat",
                    ConversationName = Guid.NewGuid(),
                    UserIds = [friendDto.User1Id, friendDto.User2Id]
                };

                result = await usersInChatJuncManager.CreateChatAsync(junc);

                if (!result)
                    return Ok("Failed to create chat");

                return Ok("Success while adding friend and cleaning");
            }
            else
                return BadRequest();
        }

        [HttpPost("Reject")]
        public async Task<IActionResult> RejectFriendRequest([FromBody] FriendRequestDto friendRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await friendRequestManager.RemoveFriendRequest(friendRequest);

            if (result)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateFriendRequest([FromBody] AddFriendRequestDto addFriendRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sender = await userManager.GetUser(addFriendRequest.SenderId);
            var receiver = await userManager.GetUser(addFriendRequest.ReceiverUsername!);

            if (sender == null || receiver == null)
                return NotFound("Failed finding users");

            if (sender.Id == receiver.Id)
                return BadRequest("You can't sent a friend request to yourself.");

            var checkIfAlreadySentFriendRequest = await friendRequestManager.FriendRequestIsAlreadySent(sender.Id, receiver.Id);
            
            if (checkIfAlreadySentFriendRequest)
            {
                return Ok("Friend request is already sent");
            }

            var checkIfAlreadyFriends = await friendManager.IsAlreadyFriends(sender.Id, receiver.Id);
            
            if (checkIfAlreadyFriends)
            {
                return Ok("Already friends");
            }

            var dto = new FriendRequestDto(sender.Id, sender.Username!, receiver.Id, receiver.Username!);
            var result = await friendRequestManager.CreateFriendRequest(dto);

            if (result)
                return Ok();
            else
                return BadRequest();
        }
    }
}
