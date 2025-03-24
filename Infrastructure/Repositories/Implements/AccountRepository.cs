using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Application.DTOs.AccountDTOs;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Data;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Infrastructure.Repositories.Implements
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;
        public AccountRepository(DataContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Crea un perfil del usuario.
        /// </summary>
        /// <param name="userName">NickName del usuario</param>
        /// <param name="id">Id del usuario</param>
        public async Task CreateProfile(string userName, int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            var userProfile  = new UserProfile 
            {
                UserId = id,
                User = user,
                NickName = userName,
            };
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
        }
    }
}