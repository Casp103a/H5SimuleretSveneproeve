using DrocsidApi.Managers;
using LogicLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DrocsidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IConversationManager conversationManager;
        private readonly IUsersInChatJuncManager usersInChatJuncManager;

        public ChatController(IConversationManager conversationManager, IUsersInChatJuncManager usersInChatJuncManager)
        {
            this.conversationManager = conversationManager;
            this.usersInChatJuncManager = usersInChatJuncManager;
        }

        #region Chat Management

        [HttpPost("createChat")]
        public async Task<IActionResult> CreateChat([FromBody] UsersInChatJuncDto chatJunc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (chatJunc == null)
                return BadRequest("Chat data is required");

            var result = await usersInChatJuncManager.CreateChatAsync(chatJunc);

            if (result == null)
                return BadRequest("Invalid chat data");

            if (result == false)
                return Conflict("Chat already exists");

            await conversationManager.CreateConversationAsync(chatJunc.ConversationName);

            return Ok();
        }

        [HttpPut("updateChat")]
        public async Task<IActionResult> UpdateChat([FromBody] UsersInChatJuncDto chatJunc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (chatJunc == null || chatJunc.Id <= 0)
                return BadRequest("Valid chat data with ID is required");

            var result = await usersInChatJuncManager.UpdateChatAsync(chatJunc);

            if (result == null)
                return BadRequest("Invalid chat data");

            if (result == false)
                return NotFound("Chat not found");

            return Ok();
        }

        [HttpDelete("deleteChat")]
        public async Task<IActionResult> DeleteChat([FromQuery] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest("Valid chat ID is required");

            var chat = await usersInChatJuncManager.GetChatAsync(id);
            if (chat == null)
                return NotFound("Chat not found");

            var result = await usersInChatJuncManager.DeleteChatAsync(id);

            if (result == false)
                return NotFound("Chat not found");

            await conversationManager.DeleteConversationAsync(chat.ConversationName);

            return Ok();
        }

        [HttpGet("getAllChats")]
        public async Task<IActionResult> GetAllChats()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var chats = await usersInChatJuncManager.GetAllChatsAsync();

            if (chats == null)
                return StatusCode(500, "Error retrieving chats");

            return Ok(chats);
        }

        [HttpGet("getChat")]
        public async Task<IActionResult> GetChat([FromQuery] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest("Valid chat ID is required");

            var chat = await usersInChatJuncManager.GetChatAsync(id);

            if (chat == null)
                return NotFound("Chat not found");

            return Ok(chat);
        }

        [HttpGet("getChatByName")]
        public async Task<IActionResult> GetChatByName([FromBody] UsersInChatJuncDto usersInChatJuncDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usersInChatJuncDto == null || usersInChatJuncDto.ConversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var chat = await usersInChatJuncManager.GetChatByNameAsync(usersInChatJuncDto.ConversationName);

            if (chat == null)
                return NotFound("Chat not found");

            return Ok(chat);
        }

        [HttpGet("getUserChats")]
        public async Task<IActionResult> GetUserChats([FromQuery] int userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userId <= 0)
                return BadRequest("Valid user ID is required");

            var chats = await usersInChatJuncManager.GetUserChatsAsync(userId);

            if (chats == null)
                return StatusCode(500, "Error retrieving user chats");

            return Ok(chats);
        }

        #endregion

        #region Participant Management

        [HttpPost("addUserToChat")]
        public async Task<IActionResult> AddUserToChat([FromBody] SendRemoveUserFromChatDto sendRemove)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (sendRemove == null || sendRemove.Junc == null || sendRemove.Junc.ConversationName == Guid.Empty || sendRemove.UserId <= 0)
                return BadRequest("Valid conversation name and user ID are required");

            var result = await usersInChatJuncManager.AddUserToChatAsync(sendRemove.UserId, sendRemove.Junc.ConversationName);

            if (result == null)
                return BadRequest("Invalid request data");

            if (result == false)
                return BadRequest("User already in chat or chat not found");

            return Ok();
        }

        [HttpDelete("removeUserFromChat")]
        public async Task<IActionResult> RemoveUserFromChat([FromBody] SendRemoveUserFromChatDto sendRemove)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (sendRemove == null || sendRemove.Junc == null || sendRemove.Junc.ConversationName == Guid.Empty || sendRemove.UserId <= 0)
                return BadRequest("Valid conversation name and user ID are required");

            var result = await usersInChatJuncManager.RemoveUserFromChatAsync(sendRemove.UserId, sendRemove.Junc.ConversationName);

            if (result == null)
                return BadRequest("Invalid request data");

            if (result == false)
                return BadRequest("User not in chat or chat not found");

            return Ok();
        }

        [HttpGet("getChatParticipants")]
        public async Task<IActionResult> GetChatParticipants([FromBody] UsersInChatJuncDto usersJuncDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usersJuncDto == null || usersJuncDto.ConversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var participants = await usersInChatJuncManager.GetChatParticipantsAsync(usersJuncDto.ConversationName);

            if (participants == null)
                return NotFound("Chat not found");

            return Ok(participants);
        }

        #endregion

        #region Message Management

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] UpdateMessageDto actionMessageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (actionMessageDto == null || actionMessageDto.UsersInChatJunc == null || actionMessageDto.UsersInChatJunc.ConversationName == Guid.Empty || actionMessageDto.Conversation == null)
                return BadRequest("Valid conversation name and message are required");

            var result = await conversationManager.SendMessageAsync(actionMessageDto.Conversation, actionMessageDto.UsersInChatJunc.ConversationName);

            if (result == null)
                return BadRequest("Invalid message data");

            if (result == false)
                return StatusCode(500, "Failed to send message");

            return Ok();
        }

        [HttpPut("updateMessage")]
        public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageDto actionMessageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (actionMessageDto == null || actionMessageDto.UsersInChatJunc == null || actionMessageDto.UsersInChatJunc.ConversationName == Guid.Empty || actionMessageDto.Conversation == null || actionMessageDto.Conversation.Id <= 0)
                return BadRequest("Valid conversation name and message with ID are required");

            var result = await conversationManager.UpdateMessageAsync(actionMessageDto.Conversation, actionMessageDto.UsersInChatJunc.ConversationName);

            if (result == null)
                return BadRequest("Invalid message data");

            if (result == false)
                return BadRequest("Message not found or unauthorized");

            return Ok();
        }

        [HttpDelete("deleteMessage")]
        public async Task<IActionResult> DeleteMessage([FromBody] UsersInChatJuncDto usersJuncDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usersJuncDto == null || usersJuncDto.ConversationName == Guid.Empty || usersJuncDto.Id <= 0)
                return BadRequest("Valid conversation name and message ID are required");

            var result = await conversationManager.DeleteMessageAsync(usersJuncDto.Id, usersJuncDto.ConversationName);

            if (result == null)
                return BadRequest("Invalid request data");

            if (result == false)
                return NotFound("Message not found");

            return Ok();
        }

        [HttpGet("getConversationMessages")]
        public async Task<IActionResult> GetConversationMessages([FromQuery] int conversationId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var conversationDto = await usersInChatJuncManager.GetChatAsync(conversationId);

            if (conversationDto == null || conversationDto.ConversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            List<ConversationDto>? messages;

            messages = await conversationManager.GetConversationMessagesAsync(conversationDto.ConversationName);

            if (messages == null)
                return StatusCode(500, "Error retrieving messages");

            return Ok(messages);
        }

        [HttpGet("getMessage")]
        public async Task<IActionResult> GetMessage([FromBody] UsersInChatJuncDto usersJuncDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usersJuncDto == null || usersJuncDto.Id <= 0 || usersJuncDto.ConversationName == Guid.Empty)
                return BadRequest("Valid conversation name and message ID are required");

            var message = await conversationManager.GetMessageAsync(usersJuncDto.Id, usersJuncDto.ConversationName);

            if (message == null)
                return NotFound("Message not found");

            return Ok(message);
        }

        [HttpGet("getLatestMessage")]
        public async Task<IActionResult> GetLatestMessage([FromQuery] int id, [FromQuery] Guid Conversation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest("Valid conversation name is required");
            var lastChat = await conversationManager.GetMessageAsync(id, Conversation);

            if (lastChat == null)
                return BadRequest("Invalid chat");

            var latestMessage = await conversationManager.GetLatestMessagesAsync(Conversation, lastChat.Sent);

            if (latestMessage == null)
                return NotFound("No new messages found");

            return Ok(latestMessage);
        }

        [HttpGet("getIsThisLatestMessage")]
        public async Task<IActionResult> GetIsThisLatestMessage([FromQuery] int id, [FromQuery] Guid conversationName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest("Valid conversation name is required");

            var isLatest = await conversationManager.IsThisLatestMessageAsync(conversationName, id);

            return Ok(isLatest);
        }

        #endregion

        #region Conversation Management

        [HttpPost("createConversation")]
        public async Task<IActionResult> CreateConversation([FromBody] UsersInChatJuncDto conversationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (conversationDto == null || conversationDto.ConversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var result = await conversationManager.CreateConversationAsync(conversationDto.ConversationName);

            if (result == null)
                return BadRequest("Invalid conversation name");

            if (result == false)
                return StatusCode(500, "Failed to create conversation");

            return Ok();
        }

        [HttpDelete("deleteConversation")]
        public async Task<IActionResult> DeleteConversation([FromBody] UsersInChatJuncDto conversationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (conversationDto == null || conversationDto.ConversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var result = await conversationManager.DeleteConversationAsync(conversationDto.ConversationName);

            if (result == null)
                return BadRequest("Invalid conversation name");

            return Ok();
        }

        #endregion
    }
}
