using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}
