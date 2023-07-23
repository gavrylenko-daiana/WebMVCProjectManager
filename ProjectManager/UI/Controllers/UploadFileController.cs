using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class UploadFileController : Controller
{
    private readonly IUploadedFileService _uploadedFileService;
    private readonly IProjectTaskService _projectTaskService;
    private readonly IProjectService _projectService;

    public UploadFileController(IUploadedFileService uploadedFileService, IProjectTaskService projectTaskService, IProjectService projectService)
    {
        _uploadedFileService = uploadedFileService;
        _projectTaskService = projectTaskService;
        _projectService = projectService;
    }

    [HttpGet]
    public IActionResult Create(Guid id, Guid projectId)
    {
        var viewModel = new CreateFileProjectTaskViewModel
        {
            ProjectTaskId = id,
            ProjectId = projectId
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid id, CreateFileProjectTaskViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        var projectTask = await _projectTaskService.GetById(id);

        if (projectTask == null) return View("Error");

        var project = await _projectService.GetById(viewModel.ProjectId);

        if (project == null) return View("Error");

        var uploadResult = await _uploadedFileService.AddFileAsync(viewModel.UploadFile);

        var taskFile = new TaskFile()
        {
            FileName = viewModel.UploadFile.FileName,
            FilePath = uploadResult.Url.ToString(),
            ProjectTaskId = projectTask.Id,
            ProjectTask = projectTask
        };

        projectTask.UploadedFiles.Add(taskFile);

        try
        {
            await _uploadedFileService.AddUploadFileAsync(taskFile);
            await _projectService.UpdateOneTaskAsync(projectTask, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return RedirectToAction("Detail", "ProjectTask");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var uploadedFile = await _uploadedFileService.GetByIdUploadFileAsync(id);

        if (uploadedFile == null) return View("Error");

        return View(uploadedFile);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteFile(Guid id)
    {
        var fileToDelete = await _uploadedFileService.GetByIdUploadFileAsync(id);

        if (fileToDelete == null) return NotFound();
        
        var projectTask = fileToDelete.ProjectTask;

        await _uploadedFileService.DeleteFileAsync(fileToDelete.FilePath);
        await _uploadedFileService.DeleteUploadFileAsync(id);

        projectTask.UploadedFiles.Remove(fileToDelete);
        await _projectTaskService.Update(projectTask.Id, projectTask);

        return RedirectToAction("Index", "Project");
    }
}