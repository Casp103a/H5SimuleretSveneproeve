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

        public FriendController(IFriendManager friendManager, IFriendRequestManager friendRequestManager, IUserManager userManager)
        {
            this.friendManager = friendManager;
            this.friendRequestManager = friendRequestManager;
            this.userManager = userManager;
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

            var dto = new FriendRequestDto(sender.Id, sender.Username!, receiver.Id, receiver.Username!);
            var result = await friendRequestManager.CreateFriendRequest(dto);

            if (result)
                return Ok();
            else
                return BadRequest();
        }
    }
}
