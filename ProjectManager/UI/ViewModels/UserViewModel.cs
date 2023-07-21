using System.ComponentModel.DataAnnotations;
using Core.Enums;
using Core.Models;

namespace UI.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    
    [Required(ErrorMessage = "UserName is required")]
    [Display(Name = "UserName")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Email Address is required")]
    [Display(Name = "Email Address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Role is required")]
    [Display(Name = "Role")]
    public UserRole Role { get; set; }
    public virtual List<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public virtual List<UserTask> AssignedTasks { get; set; } = new List<UserTask>();
}