using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class CommentSignalDTO
    {
        public int UserId { get; set; }
        public string UserNickName { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int AuthorPostId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}