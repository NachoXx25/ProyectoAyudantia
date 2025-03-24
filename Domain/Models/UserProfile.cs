using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_web_api.Domain.Models
{
    public class UserProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public string FirstName { get; set; } = string.Empty;

        public bool IsFirstNamePublic { get; set; } = true;

        public string LastName { get; set; } = string.Empty;

        public bool IsLastNamePublic { get; set; } = true;

        public string Bio { get; set; } = string.Empty;

        public bool IsBioPublic { get; set; } = true;

        public string NickName { get; set; } = string.Empty;

        public string ProfilePicture { get; set; } = string.Empty;
        public bool IsProfilePicturoPublic { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}