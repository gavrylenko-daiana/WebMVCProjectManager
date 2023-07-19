using System.ComponentModel.DataAnnotations;

namespace UI.ViewModels;

public class CreateFileProjectTaskViewModel
{
    public Guid ProjectTaskId { get; set; }
    public Guid ProjectId { get; set; }
    
    [Required(ErrorMessage = "Uploaded file is required")]
    [Display(Name = "Upload File")]
    public IFormFile UploadFile { get; set; }
}