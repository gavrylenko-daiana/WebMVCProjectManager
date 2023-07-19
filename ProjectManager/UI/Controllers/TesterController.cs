using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class TesterController : Controller
{
    private readonly ITesterService _testerService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IProjectTaskService _projectTaskService;

    public TesterController(ITesterService testerService, UserManager<AppUser> userManager, IProjectTaskService projectTaskService)
    {
        _testerService = testerService;
        _userManager = userManager;
        _projectTaskService = projectTaskService;
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
        //mail
        return RedirectToAction("Index", "Tester");
    }

    [HttpGet]
    public async Task<IActionResult> ReturnTask(Guid id)
    {
        var task = await _projectTaskService.GetById(id);
        await _testerService.ReturnTaskInProgress(task);
        //mail
        return RedirectToAction("Index", "Tester");
    }
}