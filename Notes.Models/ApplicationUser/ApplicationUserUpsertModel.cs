using System.ComponentModel.DataAnnotations;

namespace Notes.Models.ApplicationUser
{
    public class ApplicationUserUpsertModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Incorrect email format.")]

        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]

        [MinLength(3, ErrorMessage = "Username has to have at least 3 characters.")]
        public string Username { get; set; }
        [MinLength(6, ErrorMessage = "Password has to have at least 6 characters.")]

        [MaxLength(200)]
        public string Password { get; set; }
    }
}
