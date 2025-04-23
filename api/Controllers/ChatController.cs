using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_web_api.Application.Services.Interfaces;

namespace Proyecto_web_api.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Obtiene los chats de un usuario.
        /// </summary>
        /// <returns>Una colección de chats del usuario.</returns>
        [HttpGet("GetChatsByUserId")]
        [Authorize]
        public async Task<IActionResult> GetChats()
        {
            try
            {
                var UserId = int.Parse(User.FindFirst("UserId")?.Value ?? throw new Exception("No se encontró el ID del usuario."));
                var chats = await _chatService.GetChats(UserId);
                if (chats == null || !chats.Any())
                {
                    return NotFound(new { error = "No se encontraron chats para el usuario." });
                }
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los mensajes de un chat.
        /// </summary>
        /// <param name="chatId">El ID del chat.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        [HttpGet("GetMessagesByChat/{chatId}")]
        [Authorize]
        public async Task<IActionResult> GetMessagesByChat(int chatId)
        {
            try
            {
                var UserId = int.Parse(User.FindFirst("UserId")?.Value ?? throw new Exception("No se encontró el ID del usuario."));
                var messages = await _chatService.GetMessagesByChat(chatId, UserId);
                if (messages == null)
                {
                    return NotFound(new { error = "No se encontraron mensajes para el chat." });
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}