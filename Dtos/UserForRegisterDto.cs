using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [StringLength(8, MinimumLength= 4, ErrorMessage="You must specify password between 4 and 8 charactes.")]
        public string password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50, ErrorMessage="You must specify the email.")]
        public string Email { get; set; }
    }
}