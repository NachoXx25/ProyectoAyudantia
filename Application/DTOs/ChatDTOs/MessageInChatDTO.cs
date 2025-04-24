namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class MessageInChatDTO
    {
        public required string Content { get; set; }

        public int SenderId { get; set; }   

        public int RepliedTo { get; set; }

        public DateTime Date { get; set; }
    }
}