using Proyecto_web_api.Application.DTOs.AccountDTOs;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="changePasswordDTO">Contraseña actual y nueva contraseña</param>
        /// <returns>Mensaje de exito o de error</returns>
        Task<string> ChangePassword(ChangePasswordDTO changePasswordDTO);
    }
}