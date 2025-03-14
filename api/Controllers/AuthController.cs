using Microsoft.AspNetCore.Mvc;
using Proyecto_web_api.Application.DTOs.AuthDTOs;
using Proyecto_web_api.Application.Services.Interfaces;

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
                return BadRequest( new {ex.Message});
            }
        }
    }
}