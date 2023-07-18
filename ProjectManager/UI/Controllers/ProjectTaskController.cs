using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class ProjectTaskController : Controller
{
    private readonly IProjectTaskService _projectTaskService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IProjectService _projectService;

    public ProjectTaskController(IProjectTaskService projectTaskService, UserManager<AppUser> userManager,
        IProjectService projectService)
    {
        _projectTaskService = projectTaskService;
        _userManager = userManager;
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<ProjectTask> projects = await _projectTaskService.GetAll();
        return View(projects);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var projectTask = await _projectTaskService.GetById(id);

        if (projectTask == null)
        {
            TempData["Error"] = "Entered incorrect username or email. Please try again.";
            return RedirectToAction("Index", "ProjectTask");
        }

        var projectTaskDetail = new ProjectTask()
        {
            Id = projectTask.Id,
            Name = projectTask.Name,
            Description = projectTask.Description,
            DueDates = projectTask.DueDates,
            Priority = projectTask.Priority,
            Progress = projectTask.Progress,
            AssignedUsers = projectTask.AssignedUsers,
            ProjectId = projectTask.ProjectId,
            UploadedFiles = projectTask.UploadedFiles
        };

        return View(projectTaskDetail);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid projectId)
    {
        var project = await _projectService.GetById(projectId);

        if (project == null) return RedirectToAction("Index", "Project");

        ViewData["DueDates"] = project.DueDates;
        ViewData["projectId"] = projectId;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateProjectTaskViewModel createProjectTaskViewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Entered incorrect data. Please try again.";
            return View(createProjectTaskViewModel);
        }

        if (!await _projectTaskService.ProjectTaskIsAlreadyExist(createProjectTaskViewModel.Name))
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var projectTask = new ProjectTask()
            {
                Name = createProjectTaskViewModel.Name,
                Description = createProjectTaskViewModel.Description,
                DueDates = createProjectTaskViewModel.DueDates,
                Priority = createProjectTaskViewModel.Priority,
                ProjectId = projectId
            };

            await _projectTaskService.CreateTaskWithoutTesterAndStakeHolderTestAsync(projectTask);

            return RedirectToAction("Index", "ProjectTask");
        }

        TempData["Error"] = "Entered incorrect data. Please try again.";
        return View(createProjectTaskViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, Guid projectId)
    {
        var project = await _projectService.GetById(projectId);

        if (project == null) return View("Error");

        var projectTask = await _projectTaskService.GetById(id);

        if (projectTask == null) return View("Error");

        var projectTaskViewModel = new EditProjectTaskViewModel()
        {
            Name = projectTask.Name,
            Description = projectTask.Description,
            DueDates = projectTask.DueDates,
            Priority = projectTask.Priority,
            UploadedFiles = projectTask.UploadedFiles
        };

        ViewData["DueDates"] = project.DueDates;
        ViewData["projectId"] = projectId;

        return View(projectTaskViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, EditProjectTaskViewModel projectTaskViewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Entered incorrect data. Please try again.";
            return View(projectTaskViewModel);
        }

        var existingProjectTask = await _projectTaskService.GetById(id);

        if (existingProjectTask == null)
        {
            TempData["Error"] = "Task not found.";
            return RedirectToAction("Index", "ProjectTask");
        }

        existingProjectTask.Name = projectTaskViewModel.Name;
        existingProjectTask.Description = projectTaskViewModel.Description;
        existingProjectTask.DueDates = projectTaskViewModel.DueDates;
        existingProjectTask.Priority = projectTaskViewModel.Priority;

        await _projectTaskService.Update(existingProjectTask.Id, existingProjectTask);
        
        var project = await _projectService.GetById(existingProjectTask.ProjectId);
        if (project != null)
        {
            await _projectService.UpdateOneTaskAsync(existingProjectTask, project);
        }
        
        return RedirectToAction("Index", "ProjectTask");
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var projectTask = await _projectTaskService.GetById(id);
        
        if (projectTask == null) return View("Error");
        
        return View(projectTask);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var projectTask = await _projectTaskService.GetById(id);
    
        if (projectTask == null) return View("Error");

        await _projectTaskService.DeleteTask(projectTask);
        
        return RedirectToAction("Index", "ProjectTask");
    }
}