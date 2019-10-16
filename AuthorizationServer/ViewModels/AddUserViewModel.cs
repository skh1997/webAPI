using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
    }
}
