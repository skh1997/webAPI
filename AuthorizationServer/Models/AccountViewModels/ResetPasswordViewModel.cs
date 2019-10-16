using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
