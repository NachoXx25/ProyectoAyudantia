using System.ComponentModel.DataAnnotations;

namespace Proyecto_web_api.Application.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El email es requerido.")]
        [RegularExpression (@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El Correo electrónico no es válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage ="La contraseña es requerida.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica y contener al menos una mayúscula.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")] 
        [MaxLength(20, ErrorMessage = "La contraseña debe tener como máximo 20 caracteres")]
        public required string Password { get; set; }

        [Required(ErrorMessage ="La confirmación de la contraseña es requerida.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+$", ErrorMessage = "La confirmación de la contraseña debe ser alfanumérica y contener al menos una mayúscula.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El NickName es requerido.")]
        [MinLength(2, ErrorMessage = "El NickName debe tener mínimo 2 caracteres.")]
        [MaxLength(10, ErrorMessage = "El NickName debe tener máximo 10 caracteres.")]
        public required string NickName { get; set; }
    }
}