namespace Core.Models;

public class UserTask : BaseEntity
{
    public string? UserId { get; set; }
    public virtual AppUser User { get; set; }
    public Guid ProjectTaskId { get; set; }
    public virtual ProjectTask ProjectTask { get; set; }
}