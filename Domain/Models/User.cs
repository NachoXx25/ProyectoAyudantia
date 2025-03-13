using Microsoft.AspNetCore.Identity;

namespace Proyecto_web_api.Domain.Models
{
    public class User : IdentityUser<int>
    {
        public User (){
            SecurityStamp = Guid.NewGuid().ToString();
        }
        public ICollection<Post> Posts { get; set; } = [];
        public ICollection<Reaction> Reactions { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}