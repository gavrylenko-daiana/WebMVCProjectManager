using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface ITesterService : IGenericService<AppUser>
{
    Task<IEnumerable<AppUser>> GetAllTester();

    Task<AppUser> GetTesterByName(string testerName);

    Task<List<ProjectTask>> GetTesterTasksAsync(AppUser tester);

    Task AddCompletedTask(ProjectTask task);

    Task ReturnTaskInProgress(ProjectTask task);

    Task SendMailToUserAsync(string email, string message);

    Task<List<ProjectTask>> GetWaitTasksByTesterAsync(AppUser tester);

    Task UpdateProjectByTask(ProjectTask task);

    Task DeleteTesterAsync(AppUser tester);

    Task<AppUser> GetDeveloperFromTask(ProjectTask task);

    Task<AppUser> GetTesterFromTask(ProjectTask task);
}