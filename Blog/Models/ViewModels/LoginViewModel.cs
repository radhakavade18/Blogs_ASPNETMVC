using System.ComponentModel.DataAnnotations;

namespace Blog.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Password has to be atleast 6 character")]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
