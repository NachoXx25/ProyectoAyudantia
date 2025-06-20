using Proyecto_web_api.Application.DTOs.PostDTOs;
using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Infrastructure.Repositories.Interfaces
{
    public interface IPostRepository
    {
        /// <summary>
        /// Obtiene todos los posts de un usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="page">Número de página para paginación.</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        Task<(IEnumerable<OwnPostsDTO> Posts, int totalCount)> GetOwnPosts(int userId, int page, int pageSize);

        /// <summary>
        /// Crea un nuevo post
        /// </summary>
        /// <param name="post">Post a crear</param>
        /// <returns>Mensaje de éxito o error.</returns>
        Task<string> createPostDTO(Post post);

        /// <summary>
        /// Archiva o desarchiva un post
        /// </summary>
        /// <param name="postId">Id del post a eliminar</param>
        /// <param name="UserId">Id del usuario que elimina el post</param>
        /// <returns>Mensaje de éxito o error.</returns> 
        Task<string> ArchiveOrUnarchivePost(int postId, int UserId);

        /// <summary>
        /// Actualiza un post
        /// </summary>
        /// <param name="postDTO">DTO del post a actualizar</param>
        /// <returns>Mensaje de éxito o error.</returns>
        Task<string> UpdatePost(UpdatePostDTO postDTO);

        /// <summary>
        /// Obtiene todos los posts de la aplicación
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="page">Número de página para paginación.</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        Task<(IEnumerable<AllPostsDTO> Posts, int? totalCount)> GetAllPosts(int userId, int page, int pageSize);

        /// <summary>
        /// Obtiene los comentarios de un post
        /// </summary>
        /// <param name="PostId">Id del post</param>
        /// <returns>Lista de comentarios del post</returns>
        Task<IEnumerable<CommentsDTO>> GetCommentsByPostId(int PostId);

        /// <summary>
        /// Obtiene las reacciones de un post
        /// </summary>
        /// <param name="postId">Id del post</param>
        /// <returns>Lista de reacciones del post</returns>
        Task<List<ReactionDTO>> GetReactionsByPostId(int postId);

        /// <summary>
        /// Obtiene todos los ids de los posts de un usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>Lista de ids de los posts del usuario</returns>
        Task<List<int>> GetAllPostIdsByUserId(int userId);

        /// <summary>
        /// Crea un nuevo comentario
        /// </summary>
        /// <param name="commentDTO">Comentario a crear</param>
        /// <returns>Comentario creado</returns>
        Task<CommentSignalDTO> CommentPost(CommentDTO commentDTO);

        /// <summary>
        /// Agrega una reacción a un post
        /// </summary>
        /// <param name="reactionDTO">DTO de la reacción a agregar</param>
        /// <returns>Reacción creada</returns>
        Task<ReactionSignalDTO> ReactToPost(CreateReactionDTO reactionDTO);
    }
}