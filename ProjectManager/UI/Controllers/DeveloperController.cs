using BLL.Abstractions.Interfaces;
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

    public DeveloperController(IDeveloperService developerService, IProjectService projectService, UserManager<AppUser> userManager, IProjectTaskService projectTaskService)
    {
        _developerService = developerService;
        _projectService = projectService;
        _userManager = userManager;
        _projectTaskService = projectTaskService;
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
        // Send to mail

        return RedirectToAction("Index", "ProjectTask");
    }

    [HttpGet]
    public async Task<IActionResult> SendToCheck(Guid id)
    {
        var task = await _projectTaskService.GetById(id);
        await _developerService.UpdateProgressToWaitTester(task);
        // Send to mail
        
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