using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proyecto_web_api.Application.DTOs.AccountDTOs
{
    public class ChangePasswordDTO
    {
        [JsonIgnore]
        public int UserId { get; set; }
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        public required string OldPassword { get; set; }
        [Required(ErrorMessage ="La contraseña es requerida.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica y contener al menos una mayúscula.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")] 
        [MaxLength(20, ErrorMessage = "La contraseña debe tener como máximo 20 caracteres")]
        public required string NewPassword { get; set; }
        [Required(ErrorMessage ="La confirmación de la contraseña es requerida.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+$", ErrorMessage = "La confirmación de la contraseña debe ser alfanumérica y contener al menos una mayúscula.")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }
    }
}