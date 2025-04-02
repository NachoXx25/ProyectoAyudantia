using Proyecto_web_api.Application.DTOs.PostDTOs;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IPostService
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
        /// <param name="postDTO">DTO del post a crear</param>
        /// <returns>Mensaje de éxito o error.</returns>
        Task<string> createPostDTO(CreatePostDTO postDTO);

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
    }
}