using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.api.Hubs;
using Proyecto_web_api.Application.DTOs.ChatDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;
using Serilog;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public ChatService(IChatRepository chatRepository, UserManager<User> userManager, IHubContext<NotificationHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Verifica si un chat existe entre dos usuarios, y lo crea si no existe.
        /// </summary>
        /// <param name="repliedId">El ID del usuario al que se responde.</param>
        /// <param name="requestId">El ID del usuario que envía el mensaje.</param>
        /// <returns>El DTO con la información del chat.</returns>
        public async Task<InfoInChatDTO> CreateOrGetChat(int repliedId, int requestId)
        {
            if(repliedId == requestId) throw new Exception("No puedes crear un chat contigo mismo.");
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == requestId) ?? throw new Exception("Usuario remitente no encontrado.");
            var userReplied = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == repliedId) ?? throw new Exception("Usuario destinatario no encontrado.");
            try{
                var chatInfo = await _chatRepository.CreateOrGetChat(repliedId, requestId);
                //Como el chat es nuevo y no tiene mensajes, se envía la notificación a ambos usuarios
                if (!chatInfo.Messages.Any())
                {
                    await _hubContext.Clients.User(repliedId.ToString())
                        .SendAsync("NewChatCreated", new { 
                            ChatId = chatInfo.ChatId.ToString(),
                            UserName = chatInfo.RepliedNickName,
                            RepliedId = repliedId,
                            SenderId = requestId
                        });
                        
                    await _hubContext.Clients.User(requestId.ToString())
                        .SendAsync("NewChatCreated", new {
                            ChatId = chatInfo.ChatId.ToString(),
                            UserName = chatInfo.RepliedNickName,
                            RepliedId = repliedId,
                            SenderId = requestId
                        });
                    Log.Information("Nuevo chat {ChatId} creado entre usuarios {SenderId} y {RepliedId}",
                        chatInfo.ChatId, requestId, repliedId);
                }
                //En este caso en particular, el backend se encarga de avisar que se creó un chat, pero es responsabilidad del frontend añadir a los usuarios al chat usando el joinChat para que reciban notificaciones.
                return chatInfo;
            }catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message);
            }
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
        public async Task<InfoChatDTO> GetMessagesByChat(int ChatId, int UserId)
        {
            try
            {
                return await _chatRepository.GetMessagesByChat(ChatId, UserId);
            }
            catch (Exception ex)
            {
                Log.Error("Error al obtener los mensajes del chat: {Error}", ex.Message);
                throw new Exception("Error al obtener los mensajes del chat: " + ex.Message);
            }
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