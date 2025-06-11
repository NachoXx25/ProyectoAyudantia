using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_web_api.Domain.Models
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; } = null!;

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public required string Content { get; set; }    

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}