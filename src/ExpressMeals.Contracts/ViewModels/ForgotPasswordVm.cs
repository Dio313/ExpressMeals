using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels
{
    public class ForgotPasswordVm
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
