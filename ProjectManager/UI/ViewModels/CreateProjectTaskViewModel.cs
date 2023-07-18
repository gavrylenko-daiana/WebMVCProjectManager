using System.ComponentModel.DataAnnotations;
using Core.Enums;
using Core.Models;
using Microsoft.CodeAnalysis;

namespace UI.ViewModels;

public class CreateProjectTaskViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name of task")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    [Display(Name = "Description")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "Deadline is required")]
    [Display(Name = "Deadline")]
    [DataType(DataType.Date)]
    public DateTime DueDates { get; set; }
    
    [Required(ErrorMessage = "Priority is required")]
    [Display(Name = "Priority")]
    public Priority Priority { get; set; }
    
    public virtual List<TaskFile> UploadedFiles { get; set; } = new List<TaskFile>();
    
    public Guid ProjectId { get; set; }
}