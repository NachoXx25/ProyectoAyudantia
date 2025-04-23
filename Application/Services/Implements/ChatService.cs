using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Application.DTOs.ChatDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        private readonly UserManager<User> _userManager;

        private readonly IAccountRepository _accountRepository;

        public ChatService(IChatRepository chatRepository, UserManager<User> userManager, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _chatRepository = chatRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtiene los chats de un usuario.
        /// </summary>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>Una colección de chats del usuario.</returns>
        public async Task<IEnumerable<ChatDTO>> GetChats(int UserId)
        {
            var user = _userManager.Users.AsNoTracking().FirstOrDefault(u => u.Id == UserId) ?? throw new Exception("Usuario no encontrado.");
            var chats = await _chatRepository.GetUserChatsWithProfiles(UserId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
            return chats.Select(c => new ChatDTO
            {
                Id = c.ChatId,
                RepliedNickName = c.UserName,
                RepliedProfilePicture = c.ProfilePicture,
                LastMessage = c.LastMessageContent,
                LastMessageAt = c.LastMessageTime != null ? TimeZoneInfo.ConvertTime(c.LastMessageTime.Value, timeZone) : null
            }).ToList();
        }

        /// <summary>
        /// Obtiene los mensajes de un chat.
        /// </summary>
        /// <param name="ChatId">El ID del chat.</param>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        public Task<InfoChatDTO> GetMessagesByChat(int ChatId, int UserId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Envía un mensaje.
        /// </summary>
        /// <param name="Message">El DTO con la información del mensaje a enviar.</param>
        public Task SendMessage(SendMessageDTO Message)
        {
            throw new NotImplementedException();
        }
    }
}