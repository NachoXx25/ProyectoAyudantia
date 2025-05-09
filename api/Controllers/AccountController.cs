using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_web_api.Application.DTOs.AccountDTOs;
using Proyecto_web_api.Application.Services.Interfaces;

namespace Proyecto_web_api.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //Solo acciones con jwt sin importar rol
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>Perfil del usuario</returns>
        [HttpGet("GetUserProfile/{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            int? userIdClaim = int.TryParse(User.FindFirst("Id")?.Value, out int id) ? id : null;
            try
            {
                var userProfile = await _accountService.GetUserProfile(userId, userIdClaim);
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _accountService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario logueado
        /// </summary>
        /// <returns>Perfil del usuario</returns>
        [HttpGet("GetOwnProfile")]
        [Authorize]
        public async Task<IActionResult> GetOwnProfile()
        {
            int? userIdClaim = int.TryParse(User.FindFirst("Id")?.Value, out int id) ? id : null;
            if(userIdClaim == null) return Unauthorized(new { error = "Token inválido o expirado" });
            try
            {
                var userProfile = await _accountService.GetOwnProfile(userIdClaim.Value);
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="changePasswordDTO">Contraseña actual y nueva contraseña</param>
        /// <returns>Mensaje de exito o de error</returns>
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO){
            try{
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { error = "Token inválido o expirado" });
                }
                changePasswordDTO.UserId = userId;
                var result = await _accountService.ChangePassword(changePasswordDTO);
                return Ok(new { result });
            }
            catch(Exception ex)
            {
                return BadRequest( new { error = ex.Message});
            }
        }

        /// <summary>
        /// Edita el perfil del usuario
        /// </summary>
        /// <param name="profile">Atributos del perfil</param>
        /// <returns>Mensaje de exito o error</returns>
        [HttpPut("EditProfile")]
        public async Task<IActionResult> EditProfile([FromForm] ProfileDTO profile){
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try{
                var userIdClaim = User.FindFirst("Id")?.Value;
                if(string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { error = "Token inválido o expirado" });
                }
                profile.UserdId = userId;
                var result = await _accountService.EditProfile(profile);
                return Ok(new { result });
            }catch(Exception ex)
            {
                return BadRequest( new { error = ex.Message});
            }
        }
    }
}