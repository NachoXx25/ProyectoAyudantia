using System.ComponentModel.DataAnnotations;

namespace Proyecto_web_api.Application.DTOs.ChatDTOs
{
    public class SendMessageDTO
    {
        [Required(ErrorMessage = "El id del chat es requerido.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El id del chat solo puede contener números.")]
        public required string ChatId { get; set; }
        
        [Required(ErrorMessage = "El contenido del mensaje es requerido.")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "El id del remitente es requerido.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El id del remitente solo puede contener números.")]
        public required string SenderId { get; set; }

        [RegularExpression(@"^\d*$", ErrorMessage = "El id del destinatario solo puede contener números.")]
        public required string RepliedTo { get; set; }
    }
}