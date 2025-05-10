using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.PostDTOs
{
    public class CreateReactionDTO
    {
        [JsonIgnore]
        public int UserId { get; set; }
        public int PostId { get; set; }
        [Required(ErrorMessage = "El campo reacción es requerido.")]
        public string Reaction { get; set; } = string.Empty;
    }
}