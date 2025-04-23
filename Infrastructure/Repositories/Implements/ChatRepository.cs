using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proyecto_web_api.Application.DTOs.ChatDTOs;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Data;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Infrastructure.Repositories.Implements
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext _context;
        public ChatRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene los chats de un usuario con sus perfiles.
        /// </summary>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>Una colección de chats del usuario con los perfiles de los otros usuarios.</returns>
        public async Task<IEnumerable<ChatWithProfileDTO>> GetUserChatsWithProfiles(int UserId)
        {
            //Obtenemos los chats donde el usuario es el remitente (entonces el receptor es el otro usuario)
            var senderChats = await _context.Chats.Where( c => c.SenderId == UserId)
                                                  .Join(_context.UserProfiles,
                                                        chat => chat.RepliedId,
                                                        profile => profile.UserId,
                                                        (chat, profile) => new ChatWithProfileDTO
                                                        {
                                                            ChatId = chat.Id,
                                                            UserName = profile.NickName,
                                                            ProfilePicture = profile.IsProfilePicturePublic ? profile.ProfilePicture : null,
                                                            LastMessageContent = chat.Messages.Last().Content,
                                                            LastMessageTime = chat.Messages.Last().SentAt
                                                        })
                                                  .ToListAsync();

            //Obtenemos los chats donde el usuario es el receptor (entonces el remitente es el otro usuario)
            var repliedChats  = await _context.Chats.Where( c => c.RepliedId == UserId)
                                                  .Join(_context.UserProfiles,
                                                        chat => chat.SenderId,
                                                        profile => profile.UserId,
                                                        (chat, profile) => new ChatWithProfileDTO
                                                        {
                                                            ChatId = chat.Id,
                                                            UserName = profile.NickName,
                                                            ProfilePicture = profile.IsProfilePicturePublic ? profile.ProfilePicture : null,
                                                            LastMessageContent = chat.Messages.Last().Content,
                                                            LastMessageTime = chat.Messages.Last().SentAt
                                                        })
                                                  .ToListAsync();
            return senderChats.Concat(repliedChats).ToList();
        }

        /// <summary>
        /// Obtiene los mensajes de un chat.
        /// </summary>
        /// <param name="ChatId">El ID del chat.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        public async Task<InfoChatDTO> GetMessagesByChat(int ChatId)
        {
            Chat chat = await _context.Chats.Include(c => c.Messages).AsNoTracking().FirstOrDefaultAsync(c => c.Id == ChatId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
            return new InfoChatDTO
            {
                ChatId = chat.Id,
                Messages = chat.Messages.Select(m => new MessageInChatDTO
                {
                    Content = m.Content,
                    Date = TimeZoneInfo.ConvertTime(m.SentAt, timeZone),
                    SenderId = m.SenderId,
                    RepliedTo = m.RepliedId,
                }).ToList()
            };
        }
    }
}