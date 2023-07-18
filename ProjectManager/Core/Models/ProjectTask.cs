using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Core.Models;

public class ProjectTask : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; } = null!;
    public DateTime DueDates { get; set; }
    public Priority Priority { get; set; }
    public Progress Progress { get; set; }
    public virtual List<UserTask> AssignedUsers { get; set; } = new List<UserTask>();
    public Guid ProjectId { get; set; }
    
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    public virtual List<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
}
