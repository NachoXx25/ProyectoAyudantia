using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Application.DTOs.PostDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Data;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;
using Serilog;

namespace Proyecto_web_api.Infrastructure.Repositories.Implements
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;
        private readonly IFileService _fileService;
        public PostRepository(DataContext context, IFileService fileService)
        {
            _fileService = fileService;
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
                CreatedAt = TimeZoneInfo.ConvertTime(p.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                UpdatedAt = TimeZoneInfo.ConvertTime(p.UpdatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                IsArchived = p.IsArchived,
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
            Log.Information("Se obtuvieron {PostCount} posts para el usuario {UserId}", posts.Count(), userId);
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
                    Log.Information("Post creado con éxito: {PostId}", post.Id);
                    return "Post creado con éxito.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error al crear el post: {Message}", ex.Message);
                    return $"Error al crear el post: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Elimina un post (soft delete)
        /// </summary>
        /// <param name="postId">Id del post a eliminar</param>
        /// <param name="UserId">Id del usuario que elimina el post</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> ArchiveOrUnarchivePost(int postId, int UserId)
        {
            var post = await _context.Posts.FindAsync(postId) ?? throw new Exception("La publicación especificada no existe.");
            if(post.AuthorId != UserId) throw new Exception("No puedes eliminar esta publicación.");
            post.IsArchived = !post.IsArchived;
            await _context.SaveChangesAsync();
            Log.Information("Post archivado/desarchivado con éxito: {PostId}", post.Id);
            if(post.IsArchived) return "Publicación archivada con éxito.";
            return "Publicación Desarchivada con éxito.";
        }

        /// <summary>
        /// Actualiza un post
        /// </summary>
        /// <param name="postDTO">DTO del post a actualizar</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> UpdatePost(UpdatePostDTO postDTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if(string.IsNullOrWhiteSpace(postDTO.Content) && postDTO.Files.Count() == 0) throw new Exception("No se puede actualizar la publicación sin contenido o archivos.");
                    var post = await _context.Posts.FindAsync(postDTO.PostId) ?? throw new Exception("La publicación especificada no existe.");
                    if (post.AuthorId != postDTO.UserId) throw new Exception("No puedes editar esta publicación.");
                    if (postDTO.Files != null){
                        var existingFiles = await _context.PostFiles.Where(pf => pf.PostId == post.Id).ToListAsync();
                        foreach (var existingFile in existingFiles)
                        {
                            _context.PostFiles.Remove(existingFile);
                            await _fileService.DeleteFile(existingFile.PublicId);
                        }
                        Log.Information("Archivos eliminados con éxito para el post: {PostId}", post.Id);
                        foreach (var file in postDTO.Files)
                        {
                            var fileUrl = await _fileService.AddFile(file);
                            post.Files.Add(new PostFile
                            {
                                FileUrl = fileUrl.SecureUrl.AbsoluteUri,
                                PublicId = fileUrl.PublicId,
                                PostId = post.Id
                            });
                        }
                        Log.Information("Archivos actualizados con éxito para el post: {PostId}", post.Id);
                        await _context.SaveChangesAsync();
                    }
                    if(!string.IsNullOrWhiteSpace(postDTO.Content)){
                        post.Content = postDTO.Content;
                    }
                    post.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return "Post actualizado con éxito.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error al actualizar el post: {Message}", ex.Message);
                    return ex.Message;
                }
            }
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
            if(userId == 0){
                var visitorPosts = await _context.Posts.Where( p => p.IsArchived == false).OrderByDescending( p => p.CreatedAt).Include( p => p.Author).Include( p => p.Files).Include( p => p.Reactions).ThenInclude( r => r.ReactionType).Take(10).ToListAsync();
                return (visitorPosts.Select(post => new AllPostsDTO {
                PostId = post.Id,
                Content = post.Content,
                AuthorNickName = post.Author.UserName,
                CreatedAt = TimeZoneInfo.ConvertTime(post.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                UpdatedAt = TimeZoneInfo.ConvertTime(post.UpdatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                Files = post.Files.Select(f => new PostFileDTO {
                    FileId = f.Id,
                    UrlFile = f.FileUrl
                }).ToList(),
                Reactions = post.Reactions.GroupBy( re => re.ReactionTypeId).Select( group => new ReactionSummaryDTO {
                    Name = group.First().ReactionType.Name,
                    Count = group.Count(),
                    UserReacted = group.Any( g => g.UserId == userId)
                }).ToList()
                }).ToList(), null);
            }
            var posts = await _context.Posts.Where( p => p.IsArchived == false).OrderByDescending( p => p.CreatedAt).Include( p => p.Author).Include( p => p.Files).Include( p => p.Reactions).ThenInclude( r => r.ReactionType).Skip((page-1) * pageSize).Take(pageSize).ToListAsync();
            var totalCount = _context.Posts.Count();
            return (posts.Select(post => new AllPostsDTO {
                PostId = post.Id,
                Content = post.Content,
                AuthorNickName = post.Author.UserName,
                CreatedAt = TimeZoneInfo.ConvertTime(post.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                UpdatedAt = TimeZoneInfo.ConvertTime(post.UpdatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                Files = post.Files.Select(f => new PostFileDTO {
                    FileId = f.Id,
                    UrlFile = f.FileUrl
                }).ToList(),
                Reactions = post.Reactions.GroupBy( re => re.ReactionTypeId).Select( group => new ReactionSummaryDTO {
                    Name = group.First().ReactionType.Name,
                    Count = group.Count(),
                    UserReacted = group.Any( g => g.UserId == userId)
                }).ToList()
            }).ToList(), totalCount);
        }

        /// <summary>
        /// Obtiene los comentarios de un post
        /// </summary>
        /// <param name="PostId">Id del post</param>
        /// <returns>Lista de comentarios del post</returns>
        public async Task<IEnumerable<CommentsDTO>> GetCommentsByPostId(int PostId)
        {
            Log.Information("Obteniendo los comentarios del post {postId}", PostId);
           var comments = await _context.Comments
                .Where(c => c.PostId == PostId)
                .OrderByDescending(c => c.CreatedAt)
                .Join(
                    _context.UserProfiles,
                    comment => comment.UserId,
                    profile => profile.UserId,
                    (comment, profile) => new { Comment = comment, Profile = profile }
                )
                .Join(
                    _context.Users,
                    joined => joined.Comment.UserId,
                    user => user.Id,
                    (joined, user) => new CommentsDTO
                    {
                        UserNickname = user.UserName,
                        UserProfilePicture = joined.Profile.IsProfilePicturePublic ? joined.Profile.ProfilePicture : null,
                        Content = joined.Comment.Content,
                        CreatedAt = TimeZoneInfo.ConvertTime(joined.Comment.CreatedAt, 
                                    TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                    }
                )
                .ToListAsync();
            return comments;
        }

        /// <summary>
        /// Obtiene las reacciones de un post
        /// </summary>
        /// <param name="postId">Id del post</param>
        /// <returns>Lista de reacciones del post</returns>
        public async Task<IEnumerable<ReactionDTO>> GetReactionsByPostId(int postId)
        {
            if(!await _context.Posts.AnyAsync( p => p.Id == postId)) throw new Exception("La publicación especificada no existe.");
            Log.Information("Obteniendo las reacciones del post {postId}", postId);
            var reactions = await _context.Reactions
            .Where( r => r.PostId == postId)
            .OrderByDescending( r => r.CreatedAt)
            .Join(
                _context.UserProfiles,
                reactions => reactions.UserId,
                userProfile => userProfile.UserId,
                ( reactions, UserProfile ) => new { Reactions = reactions, UserProfile = UserProfile}
            )
            .Join(
                _context.Users,
                joined => joined.Reactions.UserId,
                user => user.Id,
                (joined, user) => new ReactionDTO{
                    UserNickName = user.UserName,
                    UserProfilePicture = joined.UserProfile.IsProfilePicturePublic ? joined.UserProfile.ProfilePicture : null,
                    ReactionType = joined.Reactions.ReactionType.Name
                }
            ).ToListAsync();
            return reactions;
        }
    }
}