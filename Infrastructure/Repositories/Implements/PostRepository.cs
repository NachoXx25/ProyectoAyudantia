using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Application.DTOs.PostDTOs;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Data;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Infrastructure.Repositories.Implements
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;
        public PostRepository(DataContext context)
        {
            _context = context;
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
            var totalCount = await _context.Posts.CountAsync(p => p.AuthorId == userId);
            var posts = await _context.Posts.Where( p => p.AuthorId == userId).Include( p => p.Files).Include( p => p.Reactions).ThenInclude( r => r.ReactionType).OrderByDescending( p => p.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(p => new OwnPostsDTO
            {
                PostId = p.Id,
                Content = p.Content ?? "",
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Files = p.Files.Select(f => new PostFileDTO{
                    FileId = f.Id,
                    UrlFile = f.FileUrl,
                }).ToList(),
                Reactions = p.Reactions.GroupBy( r => r.ReactionTypeId).Select(group => new ReactionSummaryDTO
                {
                    Name = group.First().ReactionType.Name,
                    Count = group.Count(),
                    UserReacted = group.Any(r => r.UserId == userId)
                }).ToList()
            }).ToListAsync();
            return (posts, totalCount);
        }

        /// <summary>
        /// Crea un nuevo post
        /// </summary>
        /// <param name="post">Post a crear</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> createPostDTO(Post post)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Posts.AddAsync(post);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return "Post creado con éxito.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return $"Error al crear el post: {ex.Message}";
                }
            }
        }
    }
}