using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class UserProject : BaseEntity
{
    public string? UserId { get; set; }
    public Guid ProjectId { get; set; }
    public virtual AppUser User { get; set; }
    public virtual Project Project { get; set; }
}