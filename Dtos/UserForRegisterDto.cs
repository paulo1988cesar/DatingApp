using System;
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
        
        [Required]
        public string  Gender { get; set; }
        
        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string City { get; set; }
        
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}