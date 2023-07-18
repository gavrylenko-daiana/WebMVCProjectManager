using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class UploadedFile : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    
    public Guid ProjectTaskId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual ProjectTask ProjectTask { get; set; }
}