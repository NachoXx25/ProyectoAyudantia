using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Proyecto_web_api.Application.DTOs.AuthDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Serilog;

namespace Proyecto_web_api.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
        {
            _authService = authService;
        }

        /// <summary>
        /// Función que permite a un usuario Loguearse
        /// </summary>
        /// <param name="login">Credenciales del usuario.</param>
        /// <returns>Token JWT en caso de éxito o error en caso de fracaso.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _authService.Login(login);
                if(result.GetType().Equals(typeof(string))) return Ok(new { Token = result });
                return BadRequest(result);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Método que registra un usuario en el sistema.
        /// </summary>
        /// <param name="register">Credenciales del usuario.</param>
        /// <returns>Token JWT en caso de éxito o error en caso de fracaso.</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _authService.Register(register);
                if(result.GetType().Equals(typeof(string))) return Ok(new { Token = result });
                return BadRequest (result);
            }
            catch(Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}