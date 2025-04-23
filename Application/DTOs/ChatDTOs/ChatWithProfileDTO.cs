namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class ChatWithProfileDTO
    {
        public int ChatId { get; set; }
        public required string UserName { get; set; } 
        public string? ProfilePicture { get; set; }
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }
}