using Core.Enums;
using Core.Models;

namespace UI.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public virtual List<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public virtual List<UserTask> AssignedTasks { get; set; } = new List<UserTask>();
}