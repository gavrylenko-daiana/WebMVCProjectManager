namespace Core.Models;

public class UserTask : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid ProjectTaskId { get; set; }
    public virtual ProjectTask ProjectTask { get; set; }
}