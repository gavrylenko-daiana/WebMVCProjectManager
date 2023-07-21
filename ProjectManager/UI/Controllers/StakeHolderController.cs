using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class StakeHolderController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IProjectService _projectService;
    private readonly ITesterService _testerService;
    private readonly IProjectTaskService _projectTaskService;
    private readonly IUserService _userService;

    public StakeHolderController(IProjectService projectService, UserManager<AppUser> userManager, ITesterService testerService, IProjectTaskService projectTaskService, IUserService userService)
    {
        _projectService = projectService;
        _userManager = userManager;
        _testerService = testerService;
        _projectTaskService = projectTaskService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> CompletedProject()
    {
        var user = await _userManager.GetUserAsync(User);
        var projects = await _projectService.GetProjectsByStakeHolder(user);
        var projectCompleted = new List<Project>();

        foreach (var project in projects)
        {
            var tasks = await _projectService.GetCompletedTask(project);

            if (project.CountDoneTasks == project.Tasks.Count && tasks.Any() &&
                project.Progress != Progress.CompletedProject)
            {
                projectCompleted.Add(project);
            }
        }

        return View(projectCompleted);
    }

    [HttpGet]
    public async Task<IActionResult> Approve(Guid id)
    {
        var project = await _projectService.GetById(id);
        await _projectService.UpdateToCompletedProject(project);

        return RedirectToAction("CompletedProject", "StakeHolder");
    }

    [HttpGet]
    public async Task<IActionResult> ReturnTask(Guid id)
    {
        var task = await _projectTaskService.GetById(id);
        await _testerService.ReturnTaskInProgress(task);
        var stakeHolder = await _userManager.GetUserAsync(User);

        if (stakeHolder != null)
        {
            var developer = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Developer)?.User;

            if (developer != null)
                await _userService.SendMessageEmailUserAsync(developer.Email,
                    $"{developer.UserName},\nThe stakeHolder - '{stakeHolder.UserName}' returned the task for which you were responsible - '{task.Name}'.");
        }
        
        return RedirectToAction("CompletedProject", "StakeHolder");
    }
}