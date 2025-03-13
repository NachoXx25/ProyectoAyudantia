using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_web_api.Domain.Models
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ChatId { get; set; }

        public Chat Chat { get; set; } = null!;

        public int SenderId { get; set; }

        public User Sender { get; set; } = null!;

        public int RepliedId { get; set; }

        public User Replied { get; set; }  = null!;

        public required string Content { get; set; }

        public DateTime SentAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
    }
}