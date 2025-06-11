using Microsoft.AspNetCore.Identity;

namespace Proyecto_web_api.Domain.Models
{
    public class User : IdentityUser<int>
    {
        public User (){
            SecurityStamp = Guid.NewGuid().ToString();
        }
        public string? StripeCustomerId { get; set; }
        public int RoleId { get; set; }

        public required Role Role { get; set; }
        public ICollection<Post> Posts { get; set; } = [];
        public ICollection<Reaction> Reactions { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}