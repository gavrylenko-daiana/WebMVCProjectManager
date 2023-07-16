using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class TesterService : GenericService<AppUser>, ITesterService
{
    private readonly IProjectService _projectService;
    private readonly IProjectTaskService _projectTaskService;

    public TesterService(IRepository<AppUser> repository, IProjectService projectService, IProjectTaskService projectTaskService) : base(repository)
    {
        _projectService = projectService;
        _projectTaskService = projectTaskService;
    }

    public async Task<IEnumerable<AppUser>> GetAllTester()
    {
        try
        {
            var testers = (await GetAll()).Where(u => u.Role == UserRole.Tester);

            return testers;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<AppUser> GetTesterByName(string testerName)
    {
        if (string.IsNullOrWhiteSpace(testerName)) throw new Exception(nameof(testerName));
        
        try
        {
            var tester = await GetByPredicate(u => u.Username == testerName && u.Role == UserRole.Tester);

            return tester;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    private async Task<Project> GetProjectByTaskAsync(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
        {
            var project = await _projectService.GetProjectByTask(task);

            return project;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateProjectByTask(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
        {
            var project = await _projectService.GetProjectByTask(task);
            await _projectService.UpdateProject(project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<List<ProjectTask>> GetTesterTasksAsync(AppUser tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        
        try
        {
            var tasks = await _projectTaskService.GetWaitTasksByTester(tester);

            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task AddCompletedTask(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
        {
            task.Progress = Progress.CompletedTask;
            await _projectTaskService.Update(task.Id, task);
            var project = await GetProjectByTaskAsync(task);
            project.CountDoneTasks += 1;
            project.Tasks.First(t => t.Id == task.Id).Progress = task.Progress;
            await _projectService.Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task ReturnTaskInProgress(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
        {
            task.Progress = Progress.InProgress;
            var project = await GetProjectByTaskAsync(task);
            project.Tasks.First(t => t.Id == task.Id).Progress = task.Progress;
            await _projectTaskService.Update(task.Id, task);
            await _projectService.Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task SendMailToUserAsync(string email, string message)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (message == null) throw new ArgumentNullException(nameof(message));

        try
        {
            await _projectService.SendMailToUser(email, message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task DeleteTesterAsync(AppUser tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        
        try
        {
            await _projectService.DeleteTesterFromProjectsAsync(tester);
            await _projectTaskService.DeleteTesterFromTasksAsync(tester);
            await Delete(tester.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<List<ProjectTask>> GetWaitTasksByTesterAsync(AppUser tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        
        try
        {
            var tasks = await _projectTaskService.GetWaitTasksByTester(tester);

            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public Task<AppUser> GetDeveloperFromTask(ProjectTask task)
    {
        try
        {
            var userTask = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Developer);
            var developer = userTask?.User;

            return Task.FromResult(developer!);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public Task<AppUser> GetTesterFromTask(ProjectTask task)
    {
        try
        {
            var userTask = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Tester);
            var tester = userTask?.User;

            return Task.FromResult(tester!);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
