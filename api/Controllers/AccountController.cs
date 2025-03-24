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
    }
}