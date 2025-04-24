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

        /// <summary>
        /// Edita el perfil del usuario
        /// </summary>
        /// <param name="profileDTO">Atributos del perfil</param>
        /// <returns>Mensaje de exito o error</returns>
        Task<string> EditProfile(ProfileDTO profileDTO);

        /// <summary>
        /// Obtiene el perfil del usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="userIdRequest">Id del usuario que solicita el perfil</param>
        /// <returns>Perfil del usuario</returns>
        Task<Object> GetUserProfile(int userId, int? userIdRequest);

        /// <summary>
        /// Obtiene el perfil del usuario logueado
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>Perfil del usuario</returns>
        Task<UserProfileDTO> GetOwnProfile(int userId);
    }
}