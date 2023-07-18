using Microsoft.AspNetCore.Http;

namespace Core.Models;

public class TaskFile : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; } 
}