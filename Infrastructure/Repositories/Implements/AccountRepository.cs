using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Application.DTOs.AccountDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Data;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Infrastructure.Repositories.Implements
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;
        private readonly IFileService _fileService;
        public AccountRepository(DataContext context, IFileService fileRepository)
        {
            _fileService = fileRepository;
            _context = context;
        }
        
        /// <summary>
        /// Crea un perfil del usuario.
        /// </summary>
        /// <param name="currentName">NickName del usuario</param>
        /// <param name="id">Id del usuario</param>
        public async Task CreateProfile(string currentName, int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
            var currentProfile  = new UserProfile
            {
                UserId = id,
                User = user,
                NickName = currentName,
            };
            await _context.UserProfiles.AddAsync(currentProfile);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Edita el perfil del usuario
        /// </summary>
        /// <param name="profile">Atributos del perfil</param>
        /// <returns>Mensaje de exito o error</returns>
        public async Task<string> EditProfile(ProfileDTO profile)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try{
                var currentProfile = await _context.UserProfiles.FirstOrDefaultAsync( up => up.UserId == profile.UserdId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
                bool hasChanges = false;
                if(currentProfile.FirstName != profile.FirstName)
                {
                    currentProfile.FirstName = profile.FirstName;
                    hasChanges = true;
                }
                if(currentProfile.IsFirstNamePublic != profile.IsFirstNamePublic)
                {
                    if (string.IsNullOrWhiteSpace(profile.FirstName)) throw new Exception("El nombre no puede estar vacío.");
                    currentProfile.IsFirstNamePublic = profile.IsFirstNamePublic;
                    hasChanges = true;
                }
                if(currentProfile.LastName != profile.LastName)
                {
                    if(string.IsNullOrWhiteSpace(profile.LastName)) throw new Exception("El apellido no puede estar vacío."); 
                    currentProfile.LastName = profile.LastName;
                    hasChanges = true;
                }
                if(currentProfile.IsLastNamePublic != profile.IsLastNamePublic)
                {
                    currentProfile.IsLastNamePublic = profile.IsLastNamePublic;
                    hasChanges = true;
                }
                if(currentProfile.NickName != profile.NickName)
                {
                    if(string.IsNullOrWhiteSpace(profile.NickName)) throw new Exception("El Nickname no puede estar vacío.");
                    var user = await _context.Users.FindAsync(profile.UserdId) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde.");
                    user.UserName = profile.NickName;
                    user.NormalizedUserName = profile.NickName.ToUpper();
                    currentProfile.NickName = profile.NickName;
                    hasChanges = true;
                }
                if(currentProfile.Bio != profile.Bio)
                {
                    currentProfile.Bio = profile.Bio;
                    hasChanges = true;
                }
                if(currentProfile.IsBioPublic != profile.IsBioPublic)
                {
                    currentProfile.IsBioPublic = profile.IsBioPublic;
                    hasChanges = true;
                }
                if(profile.ProfilePicture != null)
                {
                    var file = await _fileService.AddFile(profile.ProfilePicture);
                    if(currentProfile.ProfilePicture != file.SecureUrl.AbsoluteUri)
                    {
                        currentProfile.ProfilePicture = file.SecureUrl.AbsoluteUri;
                        hasChanges = true;
                    }
                }
                if(currentProfile.IsProfilePicturePublic != profile.IsProfilePicturePublic)
                {
                    currentProfile.IsProfilePicturePublic = profile.IsProfilePicturePublic;
                    hasChanges = true;
                }
                if(!hasChanges) throw new Exception("Debe realizar al menos una modificación.");
                currentProfile.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "Perfil editado correctamente";
            }catch{
                await transaction.RollbackAsync();
                throw; 
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="userIdRequest">Id del usuario que solicita el perfil</param>
        /// <returns>Perfil del usuario</returns>
        public async Task<Object> GetUserProfile(int userId, int? userIdRequest)
        {
            var userProfile = await _context.UserProfiles.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(up => up.UserId == userId) ?? throw new Exception("El perfil no existe.");
            bool isOwnProfile = userIdRequest.HasValue && userIdRequest.Value == userId;
            Object userProfileDTO;
            if(isOwnProfile)
            {
                userProfileDTO = new UserProfileDTO
                {
                    NickName = userProfile.NickName,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    Bio = userProfile.Bio ,
                    ProfilePicture = userProfile.ProfilePicture ,
                    IsFirstNamePublic = userProfile.IsFirstNamePublic,
                    IsLastNamePublic = userProfile.IsLastNamePublic,
                    IsBioPublic = userProfile.IsBioPublic,
                    IsProfilePicturePublic = userProfile.IsProfilePicturePublic
                };
            }
            else{
                userProfileDTO = new AnotherUserProfileDTO
                {
                    UserId = userProfile.UserId,
                    NickName = userProfile.NickName,
                    FirstName = userProfile.IsFirstNamePublic ? userProfile.FirstName : null,
                    LastName = userProfile.IsLastNamePublic ? userProfile.LastName : null,
                    Bio = userProfile.IsBioPublic ? userProfile.Bio : null,
                    ProfilePicture = userProfile.IsProfilePicturePublic ? userProfile.ProfilePicture : null
                };
            }
            return userProfileDTO;
        }
    }
}