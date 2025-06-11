namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class UpdatePostDTO
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string? Content { get; set; }
        public List<IFormFile> Files { get; set; } = [];
    }
}