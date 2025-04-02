using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotenv.net.Utilities;
using Microsoft.IdentityModel.Tokens;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Application.Services.Implements
{
    public class TokenService : ITokenService
    {

        /// <summary>
        /// Método que crea un token JWT para un usuario.
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="days">Duración del token en días</param>
        /// <returns>Token JWT</returns>
        public Task<string> CreateToken(User user, int days)
        {
            var Claims = new List<Claim>(){
                new Claim ("Id", user.Id.ToString()),
                new Claim ("Email", user.Email ?? throw new ArgumentNullException("El email es requerido")),
                new Claim ("NickName", user.UserName ?? throw new ArgumentNullException("El nickName es requerido")),
                new Claim (ClaimTypes.Role, user.Role.Name ?? throw new ArgumentNullException("No se ha mandado el rol"))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvReader.GetStringValue("JWT_SECRET")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.UtcNow.AddDays(days),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(jwt);
        }
    }
}