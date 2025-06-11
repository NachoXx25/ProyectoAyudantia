using Proyecto_web_api.Application.DTOs.AccountDTOs;
using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Infrastructure.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Crea un perfil del usuario.
        /// </summary>
        /// <param name="userName">NickName del usuario</param>
        /// <param name="id">Id del usuario</param>
        Task CreateProfile(string userName, int id);

        /// <summary>
        /// Edita el perfil del usuario
        /// </summary>
        /// <param name="profile">Atributos del perfil</param>
        /// <returns>Mensaje de exito o error</returns>
        Task<string> EditProfile(ProfileDTO profile);

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        Task<List<UserDTO>> GetAllUsers();
        /// <summary>
        /// Obtiene el perfil del usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="userIdRequest">Id del usuario que solicita el perfil</param>
        /// <returns>Perfil del usuario</returns>
        Task<Object> GetUserProfile(int userId, int? userIdRequest);

        /// <summary>
        /// Obtiene el perfil del usuario logueado
        /// summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>Perfil del usuario</returns>
        Task<UserProfile?> GetUserProfileById(int userId);
    }
}