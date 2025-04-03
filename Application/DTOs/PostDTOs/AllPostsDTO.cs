namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class AllPostsDTO
    {
        public int PostId { get; set; }
        public string? AuthorNickName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PostFileDTO>? Files { get; set; }
        public List<ReactionSummaryDTO> Reactions { get; set; } = [];
        public int TotalReactions => Reactions.Sum(r => r.Count);
    }
}