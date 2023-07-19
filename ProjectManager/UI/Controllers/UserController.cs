using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public UserController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        
        var userViewModel = new UserViewModel()
        {
            Id = currentUser.Id,
            Username = currentUser.UserName,
            Email = currentUser.Email,
            Role = currentUser.Role,
            UserProjects = currentUser.UserProjects,
            AssignedTasks = currentUser.AssignedTasks
        };
        
        return View(userViewModel);
    }
    
    
}