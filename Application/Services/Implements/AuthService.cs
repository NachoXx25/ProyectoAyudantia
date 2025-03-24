using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Proyecto_web_api.Application.DTOs.AuthDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Serilog;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ITokenService _tokenService;
        private static readonly Dictionary<string, string> ErrorTranslations = new ()
        {
            {"DuplicateUserName", "El nombre de usuario ya está en uso"},
            {"DuplicateEmail", "El correo electrónico ya está registrado"},
            {"InvalidUserName", "El nombre de usuario contiene caracteres inválidos"}
        };

        public AuthService (UserManager<User> userManager, RoleManager<Role> roleManager, ITokenService tokenService) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Función que permite a un usuario Loguearse
        /// </summary>
        /// <param name="loginDTO">Credenciales del usuario.</param>
        /// <returns>Token JWT en caso de éxito o error en caso de fracaso.</returns>
        public async Task<object> Login(LoginDTO loginDTO)
        {
            //Buscamos al usuario por su email
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            
            if(user == null) throw new Exception("Usuario o contraseña incorrectos.");
            var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());
            
            //La cuenta está bloqueada?
            if(await _userManager.IsLockedOutAsync(user))
            {  
                //Sacamos la fecha de cuando termina.
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                //Le decimos cuando puede volver a intentar.
                throw new Exception($"Cuenta bloqueada. Intente nuevamente en {lockoutEnd?.Subtract(DateTime.UtcNow).Minutes} minutos");
            }
            //Si no está bloqueada vamos a verificar si las contraseñas coinciden (comparación hasheada).
            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            //Las contraseñas no coinciden?
            if(!result)
            {
                //Se obtiene el número de intentos fallidos hasta el momento
                var failedAttemps = await _userManager.GetAccessFailedCountAsync(user);
                var leftAttemps = 5 - failedAttemps;

                //Aqui se incrementa el contador de accesos fallidos automáticamente.
                await _userManager.AccessFailedAsync(user);

                //Si supera el límite de intentos fallidos se le bloquea la cuenta.
                if( leftAttemps <= 1)
                {
                    throw new Exception($"Su cuenta a sido bloqueada por reiterados intentos fallidos, intente en 5 minutos");
                }
                else {
                    //Si todavía le quedan intentos, le damos un error de advertencia.
                    throw new Exception($"Usuario o contraseña incorrectos. Intentos restantes : {leftAttemps - 1}");
                }
            }
            //Se resetea el contador del usuario
            await _userManager.ResetAccessFailedCountAsync(user);
            //Se verifica si quiere recordar su sesión o no (cambia el tiempo del token)
            if(loginDTO.RememberMe)
            {
                return await _tokenService.CreateToken(user,5);
            }
            else{
                return await _tokenService.CreateToken(user,1);
            }
        }

        /// <summary>
        /// Método que registra un usuario en el sistema.
        /// </summary>
        /// <param name="registerDTO">Credenciales del usuario.</param>
        /// <returns>Token JWT en caso de éxito o error en caso de fracaso.</returns>
        public async Task<object> Register(RegisterDTO registerDTO)
        {
            var role = await _roleManager.FindByNameAsync("Free") ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            var user = new User {
                UserName = registerDTO.NickName,
                Email = registerDTO.Email,
                Role = role,
                RoleId = role.Id
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if(result.Succeeded) 
            {
                await _userManager.AddToRoleAsync(user, "Free");
                //todo:Llamado a crear perfil
                return await _tokenService.CreateToken(user,1);
            }
            return new AuthErrorDTO 
            { 
                Errors = result.Errors.Select(e => TranslateError(e)).ToList() 
            };
        }


        public static string TranslateError(IdentityError error)
        {
            return ErrorTranslations.TryGetValue(error.Code, out string? translation) ? translation : error.Description;
        }
    }
}