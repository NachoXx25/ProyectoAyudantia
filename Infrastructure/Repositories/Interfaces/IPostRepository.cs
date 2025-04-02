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
    }
}