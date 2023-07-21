using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class DeveloperController : Controller
{
    private readonly IDeveloperService _developerService;
    private readonly IProjectTaskService _projectTaskService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;

    public DeveloperController(IDeveloperService developerService, IProjectService projectService, UserManager<AppUser> userManager, IProjectTaskService projectTaskService, IUserService userService)
    {
        _developerService = developerService;
        _projectService = projectService;
        _userManager = userManager;
        _projectTaskService = projectTaskService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentDeveloper = await _userManager.GetUserAsync(User);
        var tasks = await _developerService.GetTasksAnotherDeveloperAsync(currentDeveloper);

        return View(tasks);
    }
    
    [HttpGet]
    public async Task<IActionResult> TakeTaskByDeveloper(Guid id)
    {
        var task = await _projectTaskService.GetById(id);
        var project = await _projectService.GetById(task.ProjectId);
        var currentDeveloper = await _userManager.GetUserAsync(User);

        await _developerService.TakeTaskByDeveloper(task, currentDeveloper, project);
        await _userService.SendMessageEmailUserAsync(currentDeveloper.Email, $"{currentDeveloper.UserName},\nyou have taken the task - '{task.Name}'.\nThe deadline is {task.DueDates}.");

        return RedirectToAction("Index", "ProjectTask");
    }

    [HttpGet]
    public async Task<IActionResult> SendToCheck(Guid id)
    {
        var task = await _projectTaskService.GetById(id);
        await _developerService.UpdateProgressToWaitTester(task);
        
        var developer = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Developer)?.User;
        
        if (developer != null)
            await _userService.SendMessageEmailUserAsync(developer.Email, $"{developer.UserName},\nYour task - '{task.Name}' has been sent to the tester for review by another developer.\nPlease wait for a response.");
        
        var tester = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Tester)?.User;

        if (tester != null)
            await _userService.SendMessageEmailUserAsync(tester.Email,
                $"{tester.UserName},\nA new task - '{task.Name} is waiting for you to check.");
        
        return RedirectToAction("Index", "Developer");
    }

    [HttpGet]
    public async Task<IActionResult> AssignedTasks()
    {
        var currentDeveloper = await _userManager.GetUserAsync(User);
        var tasks = await _developerService.GetDeveloperTasks(currentDeveloper);

        return View(tasks);
    }
}