using Proyecto_web_api.Application.DTOs.ChatDTOs;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IChatService
    {
        /// <summary>
        /// Obtiene los chats de un usuario.
        /// </summary>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>Una colección de chats del usuario.</returns>
        Task<IEnumerable<ChatDTO>> GetChats(int UserId);

        /// <summary>
        /// Obtiene los mensajes de un chat.
        /// </summary>
        /// <param name="ChatId">El ID del chat.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        Task<InfoChatDTO> GetMessagesByChat(int ChatId);

        /// <summary>
        /// Envía un mensaje.
        /// </summary>
        /// <param name="Message">El DTO con la información del mensaje a enviar.</param>
        Task SendMessage(SendMessageDTO Message);
    }
}