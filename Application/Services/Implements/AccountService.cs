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

        /// <summary>
        /// Edita el perfil del usuario
        /// </summary>
        /// <param name="profileDTO">Atributos del perfil</param>
        /// <returns>Mensaje de exito o error</returns>
        public async Task<string> EditProfile(ProfileDTO profileDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(profileDTO.UserdId.ToString()) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
                var result = await _accountRepository.EditProfile(profileDTO);
                return result;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="userIdRequest">Id del usuario que solicita el perfil</param>
        /// <returns>Perfil del usuario</returns>
        public Task<Object> GetUserProfile(int userId, int? userIdRequest)
        {
            var userProfile = _accountRepository.GetUserProfile(userId, userIdRequest);
            if (userProfile == null) throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            return userProfile;
        }
    }
}