using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace UI.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Email Address is required")]
    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Confirm Password is required")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password do not match")]
    public string ConfirmPassword { get; set; }
    
    public UserRole Role { get; set; }
}