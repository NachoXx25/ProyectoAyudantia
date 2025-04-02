namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class OwnPostsDTO
    {
        public int PostId { get; set; }
        public required string Content { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PostFileDTO>? Files { get; set; }
        public List<ReactionSummaryDTO> Reactions { get; set; } = [];
        public int TotalReactions => Reactions.Sum(r => r.Count);
    }
}