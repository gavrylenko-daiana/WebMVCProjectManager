using System.ComponentModel.DataAnnotations;

namespace UI.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Username or Email Address")]
    public string UsernameOrEmailAddress { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}