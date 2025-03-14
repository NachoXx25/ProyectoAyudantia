using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user, int days);
    }
}