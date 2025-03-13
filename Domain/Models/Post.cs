using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_web_api.Domain.Models
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public User Author { get; set; } = null!;

        public required string Content { get; set; }

        public ICollection<PostFile> Files { get; set; } = [];

        public ICollection<Reaction> Reactions { get; set; } = [];
        public ICollection<Comment> Comments  { get; set; } = [];

        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));

        public DateTime UpdatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
    }
}