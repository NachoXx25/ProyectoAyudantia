using Microsoft.AspNetCore.Identity;
using Proyecto_web_api.Application.DTOs.AuthDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;

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
        public Task<object> Login(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

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