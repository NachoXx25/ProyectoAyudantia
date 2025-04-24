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
        private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
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
            var senderChats = await _context.Chats.AsNoTracking().Include( c => c.Messages).Where( c => c.SenderId == UserId)
                                                  .Join(_context.UserProfiles,
                                                        chat => chat.RepliedId,
                                                        profile => profile.UserId,
                                                        (chat, profile) => new ChatWithProfileDTO
                                                        {
                                                            ChatId = chat.Id,
                                                            UserName = profile.NickName,
                                                            ProfilePicture = profile.IsProfilePicturePublic ? profile.ProfilePicture : null,
                                                            LastMessageContent = chat.Messages.Any() ? chat.Messages.OrderByDescending(m => m.SentAt).First().Content : null,
                                                            LastMessageTime = chat.Messages.Any() ? chat.Messages.OrderByDescending(m => m.SentAt).First().SentAt : null
                                                        })
                                                  .ToListAsync();

            //Obtenemos los chats donde el usuario es el receptor (entonces el remitente es el otro usuario)
            var repliedChats  = await _context.Chats.AsNoTracking().Include( c => c.Messages).Where( c => c.RepliedId == UserId)
                                                  .Join(_context.UserProfiles,
                                                        chat => chat.SenderId,
                                                        profile => profile.UserId,
                                                        (chat, profile) => new ChatWithProfileDTO
                                                        {
                                                            ChatId = chat.Id,
                                                            UserName = profile.NickName,
                                                            ProfilePicture = profile.IsProfilePicturePublic ? profile.ProfilePicture : null,
                                                            LastMessageContent = chat.Messages.Any() ? chat.Messages.OrderByDescending(m => m.SentAt).First().Content : null,
                                                            LastMessageTime = chat.Messages.Any() ? chat.Messages.OrderByDescending(m => m.SentAt).First().SentAt : null,
                                                        })
                                                  .ToListAsync();
            return senderChats.Concat(repliedChats).ToList();
        }

        /// <summary>
        /// Obtiene los mensajes de un chat.
        /// </summary>
        /// <param name="ChatId">El ID del chat.</param>
        /// <param name="UserId">El ID del usuario.</param>
        /// <returns>La información del chat con los mensajes.</returns>
        public async Task<InfoChatDTO> GetMessagesByChat(int ChatId, int UserId)
        {
            Chat chat = await _context.Chats.Include(c => c.Messages).AsNoTracking().FirstOrDefaultAsync(c => c.Id == ChatId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            if(chat.RepliedId != UserId && chat.SenderId != UserId) throw new Exception("No tienes permiso para ver este chat.");
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

        /// <summary>
        /// Verifica si un chat existe entre dos usuarios, y lo crea si no existe.
        /// </summary>
        /// <param name="repliedId">El ID del usuario al que se responde.</param>
        /// <param name="requestId">El ID del usuario que envía la request.</param>
        /// <returns>El DTO con la información del chat.</returns>
        public async Task<InfoInChatDTO> CreateOrGetChat(int repliedId, int requestId)
        {
            var chat = await _context.Chats.AsNoTracking().Include(c => c.Messages).FirstOrDefaultAsync(c => (c.RepliedId == repliedId && c.SenderId == requestId) || (c.RepliedId == requestId && c.SenderId == repliedId));

            if(chat != null)
            {
                if(chat.SenderId == requestId)
                {
                    var userProfile = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(up => up.UserId == chat.RepliedId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
                    return new InfoInChatDTO
                    {
                        ChatId = chat.Id,
                        RepliedNickName = userProfile.NickName,
                        RepliedProfilePicture = userProfile.IsProfilePicturePublic ? userProfile.ProfilePicture : null,
                        Messages = chat.Messages.Select(m => new MessageInChatDTO
                        {
                            Content = m.Content,
                            Date = TimeZoneInfo.ConvertTime(m.SentAt, timeZone),
                            SenderId = m.SenderId,
                            RepliedTo = m.RepliedId,
                        }).ToList()
                    };
                }
                else
                {
                    var userProfile = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(up => up.UserId == chat.SenderId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
                    return new InfoInChatDTO
                    {
                        ChatId = chat.Id,
                        RepliedNickName = userProfile.NickName,
                        RepliedProfilePicture = userProfile.IsProfilePicturePublic ? userProfile.ProfilePicture : null,
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
            else {
                var newChat = new Chat
                {
                    SenderId = requestId,
                    RepliedId = repliedId,
                    Messages = new List<Message>()
                };
                await _context.Chats.AddAsync(newChat);
                await _context.SaveChangesAsync();
                var userProfile = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(up => up.UserId == newChat.RepliedId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
                return new InfoInChatDTO
                {
                    ChatId = newChat.Id,
                    RepliedNickName = userProfile.NickName,
                    RepliedProfilePicture = userProfile.IsProfilePicturePublic ? userProfile.ProfilePicture : null,
                    Messages = new List<MessageInChatDTO>()
                };
            }
        }
    }
}