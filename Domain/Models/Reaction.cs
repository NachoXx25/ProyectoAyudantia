using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_web_api.Domain.Models
{
    public class Reaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PostId   { get; set; }
        public Post Post { get; set; } = null!;

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int ReactionTypeId { get; set; }
        public ReactionType ReactionType { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}