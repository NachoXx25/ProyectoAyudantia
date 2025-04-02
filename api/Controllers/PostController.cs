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
        [HttpGet("GetOwnPosts")]
        [Authorize]
        public async Task<IActionResult> GetOwnPosts([FromQuery] int page, int pageSize)
        {
            if (page <= 0) return BadRequest("El número de página no puede ser menor o igual a cero.");
            if(!int.TryParse(page.ToString(), out page)) return BadRequest("El número de página no es válido.");
            if (pageSize <= 0) return BadRequest("El tamaño de la página no puede ser menor o igual a cero.");
            if(!int.TryParse(pageSize.ToString(), out pageSize)) return BadRequest("El tamaño de la página no es válido.");
            string? userId = User.FindFirst("Id")?.Value;
            int.TryParse(userId, out int Id);
            var result = await _postService.GetOwnPosts(Id, page, pageSize);
            if(result.Posts.Count() == 0)
            {
                return NotFound("¡Todavía no hay publicaciones!");
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
                return Ok(result);
            }catch(Exception ex)
            {
                return BadRequest("Error al crear el post: " + ex.Message);
            }
        }
    }
}