using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.AccountDTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string NickName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } 
    }
}