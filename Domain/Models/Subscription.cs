using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_web_api.Domain.Models
{
    public class Subscription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string StripeSubscriptionId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));

        public DateTime UpdatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
    }
}