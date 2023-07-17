using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

// public class ProjectTaskController : Controller
// {
//     private readonly IProjectTaskService _projectTaskService;
//
//     public ProjectTaskController(IProjectTaskService projectTaskService)
//     {
//         _projectTaskService = projectTaskService;
//     }
//
//     [HttpGet]
//     public async Task<IActionResult> Index()
//     {
//         List<ProjectTask> projects = await _projectTaskService.GetAll();
//         return View(projects);
//     }
//     
//     
// }