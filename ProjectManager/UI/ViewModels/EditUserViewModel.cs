using System.ComponentModel.DataAnnotations;

namespace UI.ViewModels;

public class EditUserViewModel
{
    [Required(ErrorMessage = "UserName is required")]
    [Display(Name = "UserName of task")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Email Address is required")]
    [Display(Name = "Email Address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}