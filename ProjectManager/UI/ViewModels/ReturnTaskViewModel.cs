using System.ComponentModel.DataAnnotations;

namespace UI.ViewModels;

public class ReturnTaskViewModel
{
    [StringLength(200)]
    [Display(Name = "Message")]
    public string Note { get; set; }
}