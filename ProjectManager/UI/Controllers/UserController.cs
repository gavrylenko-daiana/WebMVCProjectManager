using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITesterService _testerService;
    private readonly IDeveloperService _developerService;
    private readonly IStakeHolderService _stakeHolderService;

    public UserController(UserManager<AppUser> userManager, ITesterService testerService,
        IDeveloperService developerService, IStakeHolderService stakeHolderService, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _testerService = testerService;
        _developerService = developerService;
        _stakeHolderService = stakeHolderService;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var userViewModel = new UserViewModel()
        {
            Id = currentUser.Id,
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            Role = currentUser.Role,
            UserProjects = currentUser.UserProjects,
            AssignedTasks = currentUser.AssignedTasks
        };

        return View(userViewModel);
    }
    
    [HttpGet]
    public async Task<IActionResult> Detail(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            TempData["Error"] = "Entered incorrect data. Please try again.";
            return RedirectToAction("Index", "Project");
        }

        var detailUser = new AppUser()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            UserProjects = user.UserProjects,
            AssignedTasks = user.AssignedTasks
        };

        return View(detailUser);
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
            UserName = user.UserName,
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

        user.UserName = editUserViewModel.UserName;
        user.Email = editUserViewModel.Email;
        
        await _userManager.UpdateAsync(user);
        
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

        return RedirectToAction("Index", "User", new { user.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return View("Error");

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return View("Error");

        if (user.Role == UserRole.StakeHolder) await _stakeHolderService.DeleteStakeHolder(user);
        else if (user.Role == UserRole.Developer) await _developerService.DeleteDeveloperFromTasks(user);
        else if (user.Role == UserRole.Tester) await _testerService.DeleteTesterAsync(user);
        
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login", "Account");
    }
}