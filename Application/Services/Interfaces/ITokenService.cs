using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Método que crea un token JWT para un usuario.
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="days">Duración del token en días</param>
        /// <returns>Toekn JWT</returns>
        Task<string> CreateToken(User user, int days);
    }
}