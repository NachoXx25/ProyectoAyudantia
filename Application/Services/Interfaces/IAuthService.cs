using Proyecto_web_api.Application.DTOs.AuthDTOs;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object> Login(LoginDTO loginDTO);
        Task<object> Register(RegisterDTO registerDTO);
    }
}