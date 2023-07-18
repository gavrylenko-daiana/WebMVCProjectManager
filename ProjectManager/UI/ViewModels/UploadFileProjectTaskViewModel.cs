namespace UI.ViewModels;

public class UploadFileProjectTaskViewModel
{
    public Guid ProjectTaskId { get; set; }
    public Guid ProjectId { get; set; }
    public IFormFile UploadFile { get; set; }
}