using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels;

public class UserVm
{
    public string Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
   public string RefreshToken { get; set; }
	
}
