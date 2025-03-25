using System.ComponentModel.DataAnnotations;

namespace Proyecto_web_api.Application.DTOs.AccountDTOs
{
    public class ProfileDTO
    {
        public int UserdId { get; set; }
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$", ErrorMessage = "El Nombre solo puede contener carácteres del abecedario español.")]
        [MinLength(2, ErrorMessage ="El nombre debe tener mínimo 2 letras.")]
        [MaxLength(20, ErrorMessage ="El nombre debe tener máximo 20 letras.")]
        public string FirstName { get; set; } = string.Empty;
        public bool IsFirstNamePublic { get; set; }
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$", ErrorMessage = "El Apellido solo puede contener carácteres del abecedario español.")]
        [MinLength(2, ErrorMessage ="El apellido debe tener mínimo 2 letras.")]
        [MaxLength(20, ErrorMessage ="El apellido debe tener máximo 20 letras.")]
        public string LastName { get; set; } = string.Empty;
        public bool IsLastNamePublic { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_\-\.]+$", ErrorMessage = "El nickname no es válido.")]
        [MinLength(2, ErrorMessage ="El nickname debe tener mínimo 2 letras.")]
        [MaxLength(20, ErrorMessage ="El nickname debe tener máximo 20 letras.")]
        public string NickName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool IsBioPublic { get; set; }
        public string ProfilePicture { get; set; } = string.Empty;
        public bool IsProfilePicturePublic { get; set; }
    }
}