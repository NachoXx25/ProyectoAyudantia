using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class InfoInChatDTO
    {
        public int ChatId { get; set; }
        public string RepliedNickName { get; set; } = string.Empty;
        public string? RepliedProfilePicture { get; set; } 
        public List<MessageInChatDTO> Messages { get; set; } = new List<MessageInChatDTO>();
    }
}