namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class ReactionDTO
    {
        public int TotalReactions { get; set; }
        public string? UserNickName { get; set; }
        public string? UserProfilePicture { get; set; }
        public required string ReactionType { get; set; }

    }
}