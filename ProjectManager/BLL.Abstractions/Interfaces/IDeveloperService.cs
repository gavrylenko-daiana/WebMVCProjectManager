using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IDeveloperService : IGenericService<AppUser>
{
    Task UpdateProjectByTask(ProjectTask task);

    Task UpdateProgressToWaitTester(ProjectTask task);

    Task<Project> GetProjectByNameAsync(string projectName);

    Task TakeTaskByDeveloper(ProjectTask task, AppUser developer, Project project);

    Task SendMailToUserAsync(string email, string message);

    Task<List<ProjectTask>> GetDeveloperTasks(AppUser developer);

    Task<List<ProjectTask>> GetTasksAnotherDeveloperAsync(AppUser developer);

    Task DeleteDeveloperFromTasks(AppUser developer);

    Task<AppUser> GetDeveloperFromTask(ProjectTask task);
}