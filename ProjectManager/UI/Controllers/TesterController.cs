using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class TesterController : Controller
{
    private readonly ITesterService _testerService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IProjectTaskService _projectTaskService;
    private readonly IUserService _userService;

    public TesterController(ITesterService testerService, UserManager<AppUser> userManager, IProjectTaskService projectTaskService, IUserService userService)
    {
        _testerService = testerService;
        _userManager = userManager;
        _projectTaskService = projectTaskService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentTester = await _userManager.GetUserAsync(User);
        var tasks = await _testerService.GetTesterTasksAsync(currentTester);
        
        return View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> CheckedTask(Guid id)
    {
        var task = await _projectTaskService.GetById(id);
        await _testerService.AddCompletedTask(task);
        var tester = await _userManager.GetUserAsync(User);
        
        if (tester != null)
        {
            var developer = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Developer)?.User;

            if (developer != null)
                await _userService.SendMessageEmailUserAsync(developer.Email,
                    $"{developer.UserName},\nThe tester - '{tester.UserName}' successfully tested the task for which you were responsible - '{task.Name}'.");
        }

        return RedirectToAction("Index", "Tester");
    }

    [HttpGet]
    public async Task<IActionResult> ReturnTask(Guid id)
    {
        var returnTask = new ReturnTaskViewModel();

        return View(returnTask);
    }
    
    [HttpPost]
    public async Task<IActionResult> ReturnTask(Guid id, ReturnTaskViewModel returnTaskViewModel)
    {
        var task = await _projectTaskService.GetById(id);
        await _testerService.ReturnTaskInProgress(task);
        var tester = await _userManager.GetUserAsync(User);
        
        if (tester != null)
        {
            var developer = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Developer)?.User;

            if (developer != null)
                await _userService.SendMessageEmailUserAsync(developer.Email,
                    $"{developer.UserName},\nThe tester - '{tester.UserName}' checked and returned the task for which you were responsible - '{task.Name}'.\nThe reason: '{returnTaskViewModel.Note}'");
        }
        
        return RedirectToAction("Index", "Tester");
    }
}