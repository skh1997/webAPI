using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
