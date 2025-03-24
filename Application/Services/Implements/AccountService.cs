using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Proyecto_web_api.Application.DTOs.AccountDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAccountRepository _accountRepository;

        public AccountService(UserManager<User> userManager, IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="changePasswordDTO">Contraseña actual y nueva contraseña</param>
        /// <returns>Mensaje de exito o de error</returns>
        public async Task<string> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(changePasswordDTO.UserId.ToString()) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            if(changePasswordDTO.OldPassword == changePasswordDTO.NewPassword) throw new Exception("La contraseña nueva no puede ser igual a la actual.");
            var comparePassword = await _userManager.CheckPasswordAsync(user,changePasswordDTO.OldPassword);
            if(!comparePassword) throw new Exception("La contraseña actual no corresponde.");
            await _userManager.ChangePasswordAsync(user,changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            return "Contraseña cambiada con éxito.";
        }
    }
}