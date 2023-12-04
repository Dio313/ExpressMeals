using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels;

public class ChangePasswordVm
{
	[Required]
	public string OldPassword { get; set; } 
	[Required]
	public string NewPassword { get; set; }
}
