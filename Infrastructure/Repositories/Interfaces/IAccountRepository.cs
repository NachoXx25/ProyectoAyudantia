using Proyecto_web_api.Application.DTOs.AccountDTOs;

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
    }
}