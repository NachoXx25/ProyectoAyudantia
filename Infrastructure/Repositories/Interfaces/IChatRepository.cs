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
    }
}