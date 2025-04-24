using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class SendMessageDTO
    {
        [Required(ErrorMessage = "El id del chat es requerido.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El id del chat solo puede contener números.")]
        public required string ChatId { get; set; }
        
        [Required(ErrorMessage = "El contenido del mensaje es requerido.")]
        public required string Content { get; set; }

        [JsonIgnore]
        public string? SenderId { get; set; }

        [JsonIgnore]
        public string? RepliedTo { get; set; }
    }
}