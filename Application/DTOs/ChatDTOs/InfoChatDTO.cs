namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class InfoChatDTO
    {
        public int ChatId { get; set; }

        public List<MessageInChatDTO> Messages { get; set; } = new List<MessageInChatDTO>();
    }
}