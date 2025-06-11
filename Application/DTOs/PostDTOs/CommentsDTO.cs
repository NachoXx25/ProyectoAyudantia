namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class CommentsDTO
    {
        public string? UserNickname { get; set; }
        public required string Content { get; set; }
        public string? UserProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}