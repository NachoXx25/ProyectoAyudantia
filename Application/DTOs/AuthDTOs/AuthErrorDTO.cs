using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.AuthDTOs
{
    public class AuthErrorDTO
    {
        public List<string> Errors { get; set; } = new();
    }
}