using Core.Enums;
using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IStakeHolderService : IGenericService<AppUser>
{
    Task UpdateProjectByTask(ProjectTask task);

    Task DeleteProjectAsync(string projectName);

    Task DeleteStakeHolder(AppUser stakeHolder);

    Task<List<Project>> GetProjectsByStakeHolder(AppUser stakeHolder);

    Task DeleteCurrentTask(ProjectTask task);

    Task<bool> ProjectIsAlreadyExistAsync(string projectName);

    Task<DateTime> UpdateDueDateInProjectAsync(string[] date);
    
    Task<AppUser> GetTesterByNameAsync(string name);
    
    Task AddTaskToProjectAsync(Project project, List<ProjectTask> tasks);

    Task<Project> GetProjectByNameAsync(string projectName);

    Task<Priority> GetPriorityAsync(int choice, Priority priority);

    Task<DateTime> CreateDueDateForTaskAsync(Project project, string[] date);

    Task<bool> ProjectTaskIsAlreadyExistAsync(string taskName);

    Task<AppUser> GetStakeHolderByProject(Project project);
}