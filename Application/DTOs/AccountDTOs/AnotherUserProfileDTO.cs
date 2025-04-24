namespace Proyecto_web_api.Application.DTOs.AccountDTOs
{
    public class AnotherUserProfileDTO
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public string? NickName { get; set; } 
        public string? Bio { get; set; } 
        public string? ProfilePicture { get; set; } 
    }
}