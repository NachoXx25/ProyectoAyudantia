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
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        Task<InfoInChatDTO> GetMessagesByChat(int ChatId, int UserId);

        /// <summary>
        /// Verifica si un chat existe entre dos usuarios, y lo crea si no existe.
        /// </summary>
        /// <param name="repliedId">El ID del usuario al que se responde.</param>
        /// <param name="SenderId">El ID del usuario que envía el mensaje.</param>
        /// <returns>El DTO con la información del chat.</returns>
        Task<InfoInChatDTO> CreateOrGetChat(int repliedId, int SenderId);

        /// <summary>
        /// Envía un mensaje.
        /// </summary>
        /// <param name="Message">El DTO con la información del mensaje a enviar.</param>
        Task SendMessage(SendMessageDTO Message);
    }
}