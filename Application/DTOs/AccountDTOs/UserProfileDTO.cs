namespace Proyecto_web_api.Application.DTOs.AccountDTOs
{
    public class UserProfileDTO
    {
        public string? FirstName { get; set; } 
        public bool IsFirstNamePublic { get; set; } 
        public string? LastName { get; set; } 
        public bool IsLastNamePublic { get; set; }
        public string? NickName { get; set; } 
        public string? Bio { get; set; } 
        public bool IsBioPublic { get; set; }
        public string? ProfilePicture { get; set; } 
        public bool IsProfilePicturePublic { get; set; }
    }
}