using Proyecto_web_api.Application.DTOs.ChatDTOs;

namespace Proyecto_web_api.Infrastructure.Repositories.Interfaces
{
    public interface IChatRepository
    {
        
        /// <summary>
        /// Obtiene los chats de un usuario con sus perfiles.
        /// </summary>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>Una colección de chats del usuario con los perfiles de los otros usuarios.</returns>
        public Task<IEnumerable<ChatWithProfileDTO>> GetUserChatsWithProfiles(int UserId);

        /// <summary>
        /// Obtiene los mensajes de un chat.
        /// </summary>
        /// <param name="ChatId">El ID del chat.</param>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        public Task<InfoChatDTO> GetMessagesByChat(int ChatId, int UserId);

        /// <summary>
        /// Verifica si un chat existe entre dos usuarios, y lo crea si no existe.
        /// </summary>
        /// <param name="repliedId">El ID del usuario al que se responde.</param>
        /// <param name="requestId">El ID del usuario que envía la request.</param>
        /// <returns>El DTO con la información del chat.</returns>
        public Task<InfoInChatDTO> CreateOrGetChat(int repliedId, int requestId);
    }
}