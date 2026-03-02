using LogicLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;
using DrocsidApi.Managers;

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
            if (chatJunc == null)
                return BadRequest("Chat data is required");

            var result = await usersInChatJuncManager.CreateChatAsync(chatJunc);
            
            if (result == null)
                return BadRequest("Invalid chat data");
            
            if (result == false)
                return Conflict("Chat already exists");

            // Create the conversation table
            await conversationManager.CreateConversationAsync(chatJunc.ConversationName);
            
            return Ok();
        }

        [HttpPut("updateChat")]
        public async Task<IActionResult> UpdateChat([FromBody] UsersInChatJuncDto chatJunc)
        {
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
        public async Task<IActionResult> DeleteChat(int id)
        {
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
            var chats = await usersInChatJuncManager.GetAllChatsAsync();
            
            if (chats == null)
                return StatusCode(500, "Error retrieving chats");

            return Ok(chats);
        }

        [HttpGet("getChat")]
        public async Task<IActionResult> GetChat(int id)
        {
            if (id <= 0)
                return BadRequest("Valid chat ID is required");

            var chat = await usersInChatJuncManager.GetChatAsync(id);
            
            if (chat == null)
                return NotFound("Chat not found");

            return Ok(chat);
        }

        [HttpGet("getChatByName")]
        public async Task<IActionResult> GetChatByName(Guid conversationName)
        {
            if (conversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var chat = await usersInChatJuncManager.GetChatByNameAsync(conversationName);
            
            if (chat == null)
                return NotFound("Chat not found");

            return Ok(chat);
        }

        [HttpGet("getUserChats")]
        public async Task<IActionResult> GetUserChats(int userId)
        {
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
        public async Task<IActionResult> AddUserToChat(Guid conversationName, int userId)
        {
            if (conversationName == Guid.Empty || userId <= 0)
                return BadRequest("Valid conversation name and user ID are required");

            var result = await usersInChatJuncManager.AddUserToChatAsync(userId, conversationName);
            
            if (result == null)
                return BadRequest("Invalid request data");
            
            if (result == false)
                return BadRequest("User already in chat or chat not found");

            return Ok();
        }

        [HttpDelete("removeUserFromChat")]
        public async Task<IActionResult> RemoveUserFromChat(Guid conversationName, int userId)
        {
            if (conversationName == Guid.Empty || userId <= 0)
                return BadRequest("Valid conversation name and user ID are required");

            var result = await usersInChatJuncManager.RemoveUserFromChatAsync(userId, conversationName);
            
            if (result == null)
                return BadRequest("Invalid request data");
            
            if (result == false)
                return BadRequest("User not in chat or chat not found");

            return Ok();
        }

        [HttpGet("getChatParticipants")]
        public async Task<IActionResult> GetChatParticipants(Guid conversationName)
        {
            if (conversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var participants = await usersInChatJuncManager.GetChatParticipantsAsync(conversationName);
            
            if (participants == null)
                return NotFound("Chat not found");

            return Ok(participants);
        }

        #endregion

        #region Message Management

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(Guid conversationName, [FromBody] ConversationDto message)
        {
            if (conversationName == Guid.Empty || message == null)
                return BadRequest("Valid conversation name and message are required");

            var result = await conversationManager.SendMessageAsync(message, conversationName);
            
            if (result == null)
                return BadRequest("Invalid message data");
            
            if (result == false)
                return StatusCode(500, "Failed to send message");

            return Ok();
        }

        [HttpPut("updateMessage")]
        public async Task<IActionResult> UpdateMessage(Guid conversationName, [FromBody] ConversationDto message)
        {
            if (conversationName == Guid.Empty || message == null || message.Id <= 0)
                return BadRequest("Valid conversation name and message with ID are required");

            var result = await conversationManager.UpdateMessageAsync(message, conversationName);
            
            if (result == null)
                return BadRequest("Invalid message data");
            
            if (result == false)
                return BadRequest("Message not found or unauthorized");

            return Ok();
        }

        [HttpDelete("deleteMessage")]
        public async Task<IActionResult> DeleteMessage(Guid conversationName, int messageId)
        {
            if (conversationName == Guid.Empty || messageId <= 0)
                return BadRequest("Valid conversation name and message ID are required");

            var result = await conversationManager.DeleteMessageAsync(messageId, conversationName);
            
            if (result == null)
                return BadRequest("Invalid request data");
            
            if (result == false)
                return NotFound("Message not found");

            return Ok();
        }

        [HttpGet("getConversationMessages")]
        public async Task<IActionResult> GetConversationMessages(Guid conversationName)
        {
            if (conversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            List<ConversationDto>? messages;

            messages = await conversationManager.GetConversationMessagesAsync(conversationName);

            if (messages == null)
                return StatusCode(500, "Error retrieving messages");

            return Ok(messages);
        }

        [HttpGet("getMessage")]
        public async Task<IActionResult> GetMessage(Guid conversationName, int messageId)
        {
            if (conversationName == Guid.Empty || messageId <= 0)
                return BadRequest("Valid conversation name and message ID are required");

            var message = await conversationManager.GetMessageAsync(messageId, conversationName);
            
            if (message == null)
                return NotFound("Message not found");

            return Ok(message);
        }

        #endregion

        #region Conversation Management

        [HttpPost("createConversation")]
        public async Task<IActionResult> CreateConversation(Guid conversationName)
        {
            if (conversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var result = await conversationManager.CreateConversationAsync(conversationName);
            
            if (result == null)
                return BadRequest("Invalid conversation name");
            
            if (result == false)
                return StatusCode(500, "Failed to create conversation");

            return Ok();
        }

        [HttpDelete("deleteConversation")]
        public async Task<IActionResult> DeleteConversation(Guid conversationName)
        {
            if (conversationName == Guid.Empty)
                return BadRequest("Valid conversation name is required");

            var result = await conversationManager.DeleteConversationAsync(conversationName);
            
            if (result == null)
                return BadRequest("Invalid conversation name");

            return Ok();
        }

        #endregion
    }
}
