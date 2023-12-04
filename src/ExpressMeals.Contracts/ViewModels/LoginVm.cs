using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } 
        [Required(ErrorMessage = "Password name is required")]
        public string Password { get; set; }
    }
}