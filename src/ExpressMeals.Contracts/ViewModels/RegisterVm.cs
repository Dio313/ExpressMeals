﻿using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels;

public class RegisterVm
{
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password is not matched")]
    public string ConfirmPassword { get; set; }
    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; }
}