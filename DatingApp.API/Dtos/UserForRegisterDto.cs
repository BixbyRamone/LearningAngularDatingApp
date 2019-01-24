using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 15 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters")]
        public string Password { get; set; }
    }
}