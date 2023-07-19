using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class TaskFile : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    
    public Guid ProjectTaskId { get; set; }
    [ForeignKey("ProjectTaskId")]
    public virtual ProjectTask ProjectTask { get; set; }
}