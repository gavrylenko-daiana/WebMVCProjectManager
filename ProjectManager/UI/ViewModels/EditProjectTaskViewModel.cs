using System.ComponentModel.DataAnnotations;
using Core.Enums;
using Core.Models;

namespace UI.ViewModels;

public class EditProjectTaskViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name of project")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    [Display(Name = "Description")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "Deadline is required")]
    [Display(Name = "Deadline:")]
    public DateTime DueDates { get; set; }
    
    [Required(ErrorMessage = "Priority is required")]
    [Display(Name = "Priority")]
    
    public Priority Priority { get; set; }
    
    public Guid ProjectId { get; set; }
    public List<TaskFile> UploadedFiles { get; set; } = new List<TaskFile>();
    public List<IFormFile> NewFiles { get; set; } = new List<IFormFile>();
}