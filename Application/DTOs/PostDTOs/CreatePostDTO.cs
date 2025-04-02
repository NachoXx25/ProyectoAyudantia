namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class CreatePostDTO
    {
        public int UserId { get; set; }
        public string? Content { get; set; }
        public List<IFormFile> Files { get; set; } = [];
    }
}