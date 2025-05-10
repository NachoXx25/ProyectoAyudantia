using Microsoft.AspNetCore.SignalR;
using Proyecto_web_api.api.Hubs;
using Proyecto_web_api.Application.DTOs.PostDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;
using Serilog;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IFileService _fileService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public PostService(IPostRepository postRepository, IFileService fileService, IHubContext<NotificationHub> hubContext)
        {
            _fileService = fileService;
            _postRepository = postRepository;
            _hubContext = hubContext;
        }
            
        /// <summary>
        /// Obtiene todos los posts de un usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="page">Número de página para paginación.</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        public async Task<(IEnumerable<OwnPostsDTO> Posts, int totalCount)> GetOwnPosts(int userId, int page, int pageSize)
        {
            return await _postRepository.GetOwnPosts(userId, page, pageSize);
        }

        /// <summary>
        /// Crea un nuevo post
        /// </summary>
        /// <param name="postDTO">DTO del post a crear</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> createPostDTO(CreatePostDTO postDTO)
        {
            try {
                var post = new Post {
                    Content = postDTO.Content ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AuthorId = postDTO.UserId,
                    Files = new List<PostFile>()
                };

                if (postDTO.Files != null && postDTO.Files.Any())
                {
                    foreach (var file in postDTO.Files)
                    {
                        var result = await _fileService.AddFile(file);
                        post.Files.Add(new PostFile {
                            FileUrl = result.SecureUrl.AbsoluteUri,
                            PublicId = result.PublicId,
                        });
                    }
                }
                return await _postRepository.createPostDTO(post);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear el post: {Message}", ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// Archiva o desarchiva un post
        /// </summary>
        /// <param name="postId">Id del post a eliminar</param>
        /// <param name="UserId">Id del usuario que elimina el post</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> ArchiveOrUnarchivePost(int postId, int UserId)
        {
            try
            {
                return await _postRepository.ArchiveOrUnarchivePost(postId, UserId); 
            }catch(Exception ex)
            {   
                Log.Error("Ha ocurrido un error en la acción {Message}", ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// Actualiza un post
        /// </summary>
        /// <param name="postDTO">DTO del post a actualizar</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> UpdatePost(UpdatePostDTO postDTO)
        {
            return await _postRepository.UpdatePost(postDTO);
        }

        /// <summary>
        /// Obtiene todos los posts de la aplicación
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="page">Número de página para paginación.</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        public async Task<(IEnumerable<AllPostsDTO> Posts, int? totalCount)> GetAllPosts(int userId, int page, int pageSize)
        {
            return await _postRepository.GetAllPosts(userId, page, pageSize);
        }

        /// <summary>
        /// Obtiene los comentarios de un post
        /// </summary>
        /// <param name="PostId">Id del post</param>
        /// <returns>Lista de comentarios del post</returns>
        public async Task<(IEnumerable<CommentsDTO>, int TotalComments)> GetCommentsByPostId(int PostId)
        {
            try
            {
                var comments = await _postRepository.GetCommentsByPostId(PostId);
                return (comments, comments.Count());
            }catch(Exception ex)
            {
                Log.Error("Ha ocurrido un error mientras se obtenian los mensajes {Message}", ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene las reacciones de un post
        /// </summary>
        /// <param name="postId">Id del post</param>
        /// <returns>Lista de reacciones del post</returns>
        public async Task<(IEnumerable<ReactionDTO>, int TotalReactions)> GetReactionsByPostId(int postId)
        {
            try
            {
                var reactions = await _postRepository.GetReactionsByPostId(postId);
                return (reactions, reactions.Count());
            }catch(Exception ex)
            {
                Log.Error("Ha ocurrido un error mientras se obtenian las reacciones {Message}", ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los ids de los posts de un usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>Lista de ids de los posts del usuario</returns>
        public async Task<List<int>> GetAllPostIdsByUserId(int userId)
        {
            try
            {
                return await _postRepository.GetAllPostIdsByUserId(userId);
            }catch (Exception ex)
            {
                Log.Error("Ha ocurrido un error mientras se obtenian los ids de los posts {Message}", ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Agrega un comentario a un post
        /// </summary>
        /// <param name="commentDTO">DTO del comentario a agregar</param>
        public async Task CommentPost(CommentDTO commentDTO)
        {
            try{
                var commentSignal = await _postRepository.CommentPost(commentDTO);
                await _hubContext.Clients.Group(commentSignal.PostId.ToString()).SendAsync("NewComment", commentSignal);
                await _hubContext.Clients.Group($"User_{commentSignal.AuthorPostId}").SendAsync("ReceiveComment", commentSignal);
                Log.Information("Se ha agregado un nuevo comentario al post {PostId}", commentSignal.PostId);
            }catch(Exception ex)
            {
                Log.Error("Ha ocurrido un error mientras se agregaba el comentario {Message}", ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}