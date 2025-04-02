using Proyecto_web_api.Application.DTOs.PostDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IFileService _fileService;
        public PostService(IPostRepository postRepository, IFileService fileService)
        {
            _fileService = fileService;
            _postRepository = postRepository;
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
                            FileUrl = result.SecureUrl.AbsoluteUri
                        });
                    }
                }
                return await _postRepository.createPostDTO(post);
            }
            catch (Exception ex)
            {
                return $"Error al crear el post: {ex.Message}";
            }
        }
    }
}