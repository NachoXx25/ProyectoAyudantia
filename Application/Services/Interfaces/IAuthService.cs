using Proyecto_web_api.Application.DTOs.AuthDTOs;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IAuthService
    {

        /// <summary>
        /// Función que permite a un usuario Loguearse
        /// </summary>
        /// <param name="loginDTO">Credenciales del usuario.</param>
        /// <returns>Token JWT en caso de éxito o error en caso de fracaso.</returns>
        Task<object> Login(LoginDTO loginDTO);

        /// <summary>
        /// Método que registra un usuario en el sistema.
        /// </summary>
        /// <param name="registerDTO">Credenciales del usuario.</param>
        /// <returns>Token JWT en caso de éxito o error en caso de fracaso.</returns>
        Task<object> Register(RegisterDTO registerDTO);
    }
}