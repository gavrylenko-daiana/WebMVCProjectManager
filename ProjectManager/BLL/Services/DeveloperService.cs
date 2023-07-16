using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class DeveloperService : GenericService<User>, IDeveloperService
{
    private readonly IProjectService _projectService;
    private readonly IProjectTaskService _projectTaskService;
    private readonly IUserProjectService _userProjectService;
    private readonly IUserTaskService _userTask;

    public DeveloperService(IRepository<User> repository, IProjectService projectService,
        IProjectTaskService projectTaskService, IUserProjectService userProjectService, IUserTaskService userTask) : base(repository)
    {
        _projectService = projectService;
        _projectTaskService = projectTaskService;
        _userProjectService = userProjectService;
        _userTask = userTask;
    }

    public async Task<User> GetDeveloperByUsernameOrEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        try
        {
            User stakeHolder = await GetByPredicate(u =>
                u.Role == UserRole.Developer && (u.Username == input || u.Email == input));

            if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

            return stakeHolder;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<User>> GetAllDeveloper()
    {
        try
        {
            var developer = (await GetAll()).Where(u => u.Role == UserRole.Developer);

            return developer;
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

    public async Task UpdateProgressToWaitTester(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        try
        {
            task.Progress = Progress.WaitingTester;
            await _projectTaskService.Update(task.Id, task);
            var project = await GetProjectByTaskAsync(task);
            project.Tasks.First(t => t.Id == task.Id).Progress = Progress.WaitingTester;
            await _projectService.Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Project> GetProjectByNameAsync(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));

        try
        {
            var project = await _projectService.GetProjectByName(projectName);

            return project;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task SendMailToUserAsync(string email, string message)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

        try
        {
            await _projectService.SendMailToUser(email, message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task TakeTaskByDeveloper(ProjectTask task, User developer, Project project)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        try
        {
            var userTaskDeveloper = new UserTask
                { UserId = developer.Id, User = developer, ProjectTaskId = task.Id, ProjectTask = task };
            await _userTask.Add(userTaskDeveloper);
            task.Progress = Progress.InProgress;

            if (!await _userProjectService.IsUserInProject(developer.Id, project.Id))
            {
                var userProjectDeveloper = new UserProject
                {
                    UserId = developer.Id,
                    ProjectId = project.Id,
                    User = developer,
                    Project = project
                };
                await _userProjectService.Add(userProjectDeveloper);
            }

            await _projectTaskService.Update(task.Id, task);
            await _projectService.Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<ProjectTask>> GetDeveloperTasks(User developer)
    {
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        try
        {
            var tasks = await _projectTaskService.GetTasksByDeveloper(developer);

            return tasks;
        }
        catch
        {
            throw new Exception($"Task list is empty.");
        }
    }

    public async Task<List<ProjectTask>> GetTasksAnotherDeveloperAsync(User developer)
    {
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        try
        {
            var tasks = await _projectTaskService.GetTasksAnotherDeveloper(developer);

            return tasks;
        }
        catch
        {
            throw new Exception($"Task list is empty.");
        }
    }

    public async Task DeleteDeveloperFromTasks(User developer)
    {
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        try
        {
            var tasks = await GetDeveloperTasks(developer);
            await _projectTaskService.DeleteDeveloperFromTasksAsync(tasks);
            await Delete(developer.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<User> GetDeveloperFromTask(ProjectTask task)
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
}