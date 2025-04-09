using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Proyecto_web_api.Application.DTOs.PostDTOs;
using Proyecto_web_api.Application.Services.Interfaces;

namespace Proyecto_web_api.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Obtiene todos los posts de un usuario
        /// </summary>
        /// <param name="page">Número de página para paginación.</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        [HttpGet("GetAllPosts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPosts([FromQuery] int page, int pageSize)
        {
            if (page <= 0) return BadRequest("El número de página no puede ser menor o igual a cero.");
            if (pageSize <= 0) return BadRequest("El tamaño de la página no puede ser menor o igual a cero.");  
            string? userId = User.FindFirst("Id")?.Value;
            int.TryParse(userId, out int Id);
            var result = await _postService.GetAllPosts(Id, page, pageSize);
            if(result.Posts.Count() == 0)
            {
                return NotFound("¡Todavía no hay publicaciones!");
            }
            else{
                return Ok(new {
                    result.Posts,
                    result.totalCount
                });
            }
        }

        /// <summary>
        /// Obtiene todos los posts de un usuario
        /// </summary>
        /// <param name="page">Número de página para paginación.</param>
        /// <param name="pageSize">Número de elementos por página.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        [HttpGet("GetOwnPosts")]
        [Authorize]
        public async Task<IActionResult> GetOwnPosts([FromQuery] int page, int pageSize)
        {
            if (page <= 0) return BadRequest("El número de página no puede ser menor o igual a cero.");
            if (pageSize <= 0) return BadRequest("El tamaño de la página no puede ser menor o igual a cero.");
            string? userId = User.FindFirst("Id")?.Value;
            int.TryParse(userId, out int Id);
            var result = await _postService.GetOwnPosts(Id, page, pageSize);
            if(result.Posts.Count() == 0)
            {
                return NotFound(new { result = "¡Todavía no hay publicaciones!" });
            }
            else
            {
                return Ok(new
                {
                    result.Posts,
                    TotalCount = result.totalCount
                });
            }
        }

        /// <summary>
        /// Obtiene los comentarios de un post
        /// </summary>
        /// <param name="postId">Id del post</param>
        /// <returns>Lista de comentarios del post</returns>
        [HttpGet("GetCommentsByPostId/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsByPostId(int postId)
        {
            try
            {
                if (postId <= 0) return BadRequest("El id del post no puede ser menor o igual a cero.");
                var result = await _postService.GetCommentsByPostId(postId);
                if(result.Item1.Count() == 0)
                {
                    return NotFound(new { result = "¡Todavía no hay comentarios!"});
                }
                else
                {
                    return Ok(new { result });
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene las reacciones de un post
        /// </summary>
        /// <param name="postId">Id del post</param>
        /// <returns>Lista de reacciones del post</returns>
        [HttpGet("GetReactionsByPostId/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReactionsByPostId(int postId)
        {
            try
            {
                if (postId <= 0) return BadRequest("El id del post no puede ser menor o igual a cero.");
                var result = await _postService.GetReactionsByPostId(postId);
                if(result.Item1.Count() == 0)
                {
                    return NotFound(new { result = "¡Todavía no hay reacciones!"});
                }
                else
                {
                    return Ok(new { result });
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los posts de un usuario
        /// </summary>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        [HttpPost("CreatePost")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDTO postDTO)
        {
            if (postDTO == null) return BadRequest("El post no puede ser nulo.");
            if(!string.IsNullOrWhiteSpace(postDTO.Content) && postDTO.Content.Length > 500) return BadRequest("El contenido del post no puede exceder los 500 caracteres.");
            if (postDTO.Files == null || postDTO.Files.Count() > 3) return BadRequest("El post debe tener máximo 3 archivos.");
            if(string.IsNullOrWhiteSpace(postDTO.Content) && postDTO.Files.Count() == 0) return BadRequest("El post debe tener al menos un archivo o contenido.");
            try
            {
                string? userId = User.FindFirst("Id")?.Value;
                int.TryParse(userId, out int Id);
                postDTO.UserId = Id;
                var result = await _postService.createPostDTO(postDTO);
                return Ok(new {result});
            }catch(Exception ex)
            {
                return BadRequest("Error al crear el post: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los posts de un usuario
        /// </summary>
        /// <param name="postId">Id del post a eliminar.</param>
        /// <returns>Post con la información del usuario y el conteo total.</returns>
        [HttpPatch("ArchiveOrUnarchivePost/{postId}")]
        [Authorize]
        public async Task<IActionResult> ArchiveOrUnarchivePost(int postId)
        {
            try
            {
                var UserIdClaim = User.FindFirst("Id")?.Value;
                if(UserIdClaim.IsNullOrEmpty() || !int.TryParse(UserIdClaim?.ToString(), out int userId)) throw new Exception("Algo falló en la autenticación del usuario."); 
                return Ok(new { result = await _postService.ArchiveOrUnarchivePost(postId, userId)});
            }catch(Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un post
        /// </summary>
        /// <param name="postDTO">DTO del post a actualizar</param>
        /// <returns>Mensaje de éxito o error.</returns>
        [HttpPut("UpdatePost")]
        [Authorize]
        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostDTO postDTO)
        {
            try
            {
                if(!string.IsNullOrWhiteSpace(postDTO.Content) && postDTO.Content.Length > 500) throw new Exception("El contenido del post no puede exceder los 500 caracteres.");
                string? userId = User.FindFirst("Id")?.Value;
                int.TryParse(userId, out int Id);
                postDTO.UserId = Id;
                var result = await _postService.UpdatePost(postDTO);
                return Ok(new {result});
            }catch(Exception ex)
            {
                return BadRequest(new {ex.Message});
            }
        }
    }
}