using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class CommentDTO
    {
        [JsonIgnore]
        public int UserId { get; set; }

        [Required (ErrorMessage = "El campo comentario es requerido.")]
        public required string Comment { get; set; } 

        public int PostId { get; set; }
    }
}