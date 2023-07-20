using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using UI.ViewModels;

namespace UI.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly UserManager<AppUser> _userManager;

    public ProjectController(IProjectService projectService, UserManager<AppUser> userManager)
    {
        _projectService = projectService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<Project> projects = await _projectService.GetAll();
        return View(projects);
    }
    
    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var project = await _projectService.GetById(id);

        if (project == null)
        {
            TempData["Error"] = "Entered incorrect data. Please try again.";
            return RedirectToAction("Index", "Project");
        }

        var projectDetail = new Project()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CountDoneTasks = project.CountDoneTasks,
            DueDates = project.DueDates,
            Progress = project.Progress,
            UserProjects = project.UserProjects,
            Tasks = project.Tasks
        };

        return View(projectDetail);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectViewModel createProjectViewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Entered incorrect data. Please try again.";
            return View(createProjectViewModel);
        }

        if (!await _projectService.ProjectIsAlreadyExist(createProjectViewModel.Name))
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var project = new Project
            {
                Name = createProjectViewModel.Name,
                Description = createProjectViewModel.Description,
                DueDates = createProjectViewModel.DueDates,
            };

            await _projectService.CreateProject(currentUser, project);
            return RedirectToAction("Index", "Project");
        }
        
        TempData["Error"] = "Entered incorrect data. Please try again.";
        return View(createProjectViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var project = await _projectService.GetById(id);
    
        if (project == null) return View("Error");

        var projectViewModel = new EditProjectViewModel()
        {
            Name = project.Name,
            Description = project.Description,
            DueDates = project.DueDates
        };

        return View(projectViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, EditProjectViewModel projectViewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Entered incorrect data. Please try again.";
            return View(projectViewModel);
        }
        
        var project = new Project()
        {
            Id = id,
            Name = projectViewModel.Name,
            Description = projectViewModel.Description,
            DueDates = projectViewModel.DueDates
        };

        await _projectService.Update(project.Id, project);

        return RedirectToAction("Index", "Project");
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _projectService.GetById(id);
        
        if (project == null) return View("Error");
        
        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _projectService.GetById(id);

        if (project == null) return View("Error");

        await _projectService.DeleteProject(project.Name);
        
        return RedirectToAction("Index", "Project");
    }
}