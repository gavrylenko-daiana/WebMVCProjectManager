using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models;

public class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; } = null!;
    public int CountDoneTasks { get; set; } = 0;
    public DateTime DueDates { get; set; }
    public Progress Progress { get; set; }
    public virtual List<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public virtual List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
