using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserService _userService;

    public UserController(UserManager<AppUser> userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
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

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return View("Error");
        }

        var editUserViewModel = new EditUserViewModel()
        {
            Username = user.UserName,
            Email = user.Email
        };
        
        return View(editUserViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel editUserViewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit profile");
            
            return View("Edit", editUserViewModel);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return View("Error");
        }

        user.UserName = editUserViewModel.Username;
        user.Email = editUserViewModel.Email;

        await _userManager.UpdateAsync(user);
        await _userService.UpdateIdentity(user.Id, user);

        return RedirectToAction("Index", "User", new { user.Id });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditPassword()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return View("Error");
        }

        var editUserPasswordViewModel = new EditUserPasswordViewModel();
        
        return View(editUserPasswordViewModel);
    }

    [HttpPost]
    [ActionName("EditPassword")]
    public async Task<IActionResult> EditUserPassword(EditUserPasswordViewModel editUserPasswordViewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("editUserPasswordViewModel", "Failed to edit profile");
            
            return View("EditPassword", editUserPasswordViewModel);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return View("Error");
        }
        
        var checkPassword = await _userManager.CheckPasswordAsync(user, editUserPasswordViewModel.CurrentPassword);

        if (checkPassword)
        {
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, editUserPasswordViewModel.NewPassword);
        }

        await _userManager.UpdateAsync(user);
        await _userService.UpdateIdentity(user.Id, user);

        return RedirectToAction("Index", "User", new { user.Id });
    }
}