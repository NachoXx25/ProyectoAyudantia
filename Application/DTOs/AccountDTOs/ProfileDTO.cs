namespace Proyecto_web_api.Application.DTOs.AccountDTOs
{
    public class ProfileDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public bool IsFirstNamePublic { get; set; }
        public string LastName { get; set; } = string.Empty;
        public bool IsLastNamePublic { get; set; }
        public string NickName { get; set; } = string.Empty;
        public bool IsNickNamePublic { get; set; }
        public string Bio { get; set; } = string.Empty;
        public bool IsBioPublic { get; set; }
        public string ProfilePicture { get; set; } = string.Empty;
        public bool IsProfilePicturePublic { get; set; }
    }
}