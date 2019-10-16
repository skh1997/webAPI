using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords not match.")]
        public string ConfirmPassword { get; set; }
    }
}
