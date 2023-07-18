using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectService : IGenericService<Project>
{
    Task<bool> ProjectIsAlreadyExist(string userInput);

    Task<Project> GetProjectByName(string projectName);

    Task<List<Project>> GetProjectsByStakeHolder(AppUser stakeHolder);

    Task<Project> GetProjectByTask(ProjectTask task);

    Task UpdateProject(Project project);

    Task<List<ProjectTask>> GetCompletedTask(Project project);

    Task UpdateToCompletedProject(Project project);

    Task UpdateToWaitingTask(Project project);

    Task SendMailToUser(string email, string messageEmail);

    Task<DateTime> UpdateDueDateInProject(string[] date);

    Task UpdateDueDateInTask(ProjectTask task, string[] date);

    Task DeleteProject(string projectName);

    Task DeleteProjectsWithSteakHolderAsync(AppUser stakeHolder);

    Task DeleteCurrentTaskAsync(ProjectTask task);

    Task CreateProjectTestWithoutStakeHolderAsync(Project projectVM);
    
    Task CreateProject(AppUser stakeHolder, Project project);

    Task AddTaskToProject(Project project, List<ProjectTask> tasks);

    Task DeleteTesterFromProjectsAsync(AppUser tester);
    
    Task UpdateOneTaskAsync(ProjectTask task, Project project);

    Task UpdateTask(ProjectTask task, List<ProjectTask> modifierTasks, Project project, ProjectTask newTask);
    
    Task<AppUser> GetStakeHolderByProject(Project project);

    Task<AppUser> GetTesterFromProject(Project project);
}
