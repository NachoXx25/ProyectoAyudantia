namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class ReactionSummaryDTO
    {
        public int ReactionTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool UserReacted { get; set; }
    }
}