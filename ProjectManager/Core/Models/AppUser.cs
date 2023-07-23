using Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Core.Models;

public class AppUser : IdentityUser
{
    public UserRole Role { get; set; }
    public virtual List<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public virtual List<UserTask> AssignedTasks { get; set; } = new List<UserTask>();
}
