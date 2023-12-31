using System.ComponentModel.DataAnnotations;
using Core.Enums;
using Core.Models;

namespace UI.ViewModels;

public class CreateProjectViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name of project")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    [Display(Name = "Description")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "Deadline is required")]
    [Display(Name = "Deadline")]
    [DataType(DataType.Date)]
    public DateTime DueDates { get; set; }
}