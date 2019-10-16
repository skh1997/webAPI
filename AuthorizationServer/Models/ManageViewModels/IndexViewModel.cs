using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public string Alias { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
