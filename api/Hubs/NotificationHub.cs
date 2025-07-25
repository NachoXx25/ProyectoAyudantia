using Microsoft.AspNetCore.SignalR;
using Proyecto_web_api.Application.Services.Interfaces;
using Serilog;

namespace Proyecto_web_api.api.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly Dictionary<string, HashSet<string>> _groupConnections = new();
        private static readonly object _lock = new();
        private readonly IChatService _chatService;
        public NotificationHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Método llamado cuando un cliente se conecta.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Log.Information($"Client Connected - ConnectionId: {Context.ConnectionId}, Time: {DateTime.Now}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Método llamado cuando un cliente se desconecta.
        /// </summary>
        /// <param name="exception">La excepción que causó la desconexión, si existe.</param>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                Log.Error($"Client Disconnected with error - ConnectionId: {Context.ConnectionId}, Error: {exception.Message}");
            }
            else
            {
                Log.Information($"Client Disconnected normally - ConnectionId: {Context.ConnectionId}");
            }

            // Removemos la conexión del grupo al que pertenece
            lock (_lock)
            {
                foreach (var group in _groupConnections)
                {
                    group.Value.Remove(Context.ConnectionId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Método para unirse a un chat.
        /// </summary>
        /// <param name="chatId">El ID del chat.</param>
        public async Task JoinChat(string chatId)
        {
            try
            {
                Log.Information($"Attempting to join chat - ConnectionId: {Context.ConnectionId}, ChatId: {chatId}");

                lock (_lock)
                {
                    if (!_groupConnections.ContainsKey(chatId))
                    {
                        _groupConnections[chatId] = new HashSet<string>();
                    }
                    _groupConnections[chatId].Add(Context.ConnectionId);
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
                var ConnectionCount = GetConnectionCount(chatId);
                Log.Information($"Successfully joined chat - ChatId: {chatId}, Active Connections: {ConnectionCount}");
            }
            catch (Exception ex)
            {
               Log.Error($"Error joining chat - ConnectionId: {Context.ConnectionId}, ChatId: {chatId}, Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método para unirse a un post.
        /// </summary>
        /// <param name="postId">El ID del post.</param>
        public async Task JoinPost(string postId)
        {
           try
           {
                Log.Information($"Attempting to join post - ConnectionId: {Context.ConnectionId}, PostId: {postId}");
    
                lock (_lock)
                {
                    if (!_groupConnections.ContainsKey(postId))
                    {
                        _groupConnections[postId] = new HashSet<string>();
                    }
                    _groupConnections[postId].Add(Context.ConnectionId);
                }
    
                await Groups.AddToGroupAsync(Context.ConnectionId, postId);
                Log.Information($"Successfully joined post - PostId: {postId}, Active Connections: {GetConnectionCount(postId)}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error joining post - ConnectionId: {Context.ConnectionId}, PostId: {postId}, Error: {ex.Message}");
               throw;
            }
        }

        /// <summary>
        /// Método para salir de un post.
        /// </summary>
        /// <param name="postId">El ID del post.</param>
        public async Task LeavePost(string postId)
        {
            try
            {
                Log.Information($"Attempting to leave post - ConnectionId: {Context.ConnectionId}, PostId: {postId}");

                lock (_lock)
                {
                    if (_groupConnections.ContainsKey(postId))
                    {
                        _groupConnections[postId].Remove(Context.ConnectionId);
                    }
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId);
            }
            catch (Exception ex)
            {
                Log.Error($"Error leaving post - ConnectionId: {Context.ConnectionId}, PostId: {postId}, Error: {ex.Message}");
                throw;
            }
        }   

        /// <summary>
        /// Método para unirse a múltiples posts a la vez.
        /// </summary>
        /// <param name="postIds">Lista de IDs de posts.</param>
        public async Task JoinAllPosts(List<string> postIds)
        {
            try
            {
                Log.Information($"Attempting to join multiple posts - ConnectionId: {Context.ConnectionId}, Count: {postIds.Count}");
                
                foreach (var postId in postIds)
                {
                    await JoinPost(postId);
                }
                Log.Information($"Successfully joined all posts - ConnectionId: {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error joining multiple posts - ConnectionId: {Context.ConnectionId}, Error: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Método para indicar que el usuario ha cerrado la interfaz de un chat pero sigue recibiendo notificaciones.
        /// </summary>
        /// <param name="chatId">El ID del chat.</param>
        public void CloseChat(string chatId)
        {
            try
            {
                Log.Information($"User closed chat UI - ConnectionId: {Context.ConnectionId}, ChatId: {chatId}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error closing chat - ConnectionId: {Context.ConnectionId}, ChatId: {chatId}, Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método para unirse a múltiples chats a la vez.
        /// </summary>
        /// <param name="chatIds">Lista de IDs de chats.</param>
        public async Task JoinAllChats(List<string> chatIds)
        {
            try
            {
                Log.Information($"Attempting to join multiple chats - ConnectionId: {Context.ConnectionId}, Count: {chatIds.Count}");
                
                foreach (var chatId in chatIds)
                {
                    await JoinChat(chatId);
                }
                
                Log.Information($"Successfully joined all chats - ConnectionId: {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error joining multiple chats - ConnectionId: {Context.ConnectionId}, Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método para unirse a un grupo de usuario.
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        public async Task JoinUserGroup(string userId)
        {
            try
            {
                var userGroup = $"User_{userId}";
                Log.Information($"Attempting to join user group - ConnectionId: {Context.ConnectionId}, UserId: {userId}");

                lock (_lock)
                {
                    if (!_groupConnections.ContainsKey(userGroup))
                    {
                        _groupConnections[userGroup] = new HashSet<string>();
                    }
                    _groupConnections[userGroup].Add(Context.ConnectionId);
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);

                var ConnectionCount = GetConnectionCount(userGroup);
                Log.Information($"Successfully joined user group - UserId: {userId}, Active Connections: {ConnectionCount}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error joining user group - ConnectionId: {Context.ConnectionId}, UserId: {userId}, Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método para salir de un chat.
        /// </summary>
        /// <param name="chatId">El ID del chat.</param>
        public async Task LeaveChat(string chatId)
        {
            try
            {
                Log.Information($"Attempting to leave chat - ConnectionId: {Context.ConnectionId}, ChatId: {chatId}");

                lock (_lock)
                {
                    if (_groupConnections.ContainsKey(chatId))
                    {
                        _groupConnections[chatId].Remove(Context.ConnectionId);
                    }
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
                Log.Information($"Successfully left chat - ChatId: {chatId}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error leaving chat - ConnectionId: {Context.ConnectionId}, ChatId: {chatId}, Error: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Obtiene el número de conexiones en un grupo.
        /// </summary>
        /// <param name="groupName">El nombre del grupo.</param>
        /// <returns>El número de conexiones en el grupo.</returns>
        private int GetConnectionCount(string groupName)
        {
            lock (_lock)
            {
                return _groupConnections.TryGetValue(groupName, out var connections) ? connections.Count : 0;
            }
        }
    }
}