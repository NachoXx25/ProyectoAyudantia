namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class ChatDTO
    {
        public int Id { get; set; }

        public required string RepliedNickName { get; set; } 
        
        public string? RepliedProfilePicture { get; set; }

        public string? LastMessage { get; set; } 

        public DateTime? LastMessageAt { get; set; }
    }
}