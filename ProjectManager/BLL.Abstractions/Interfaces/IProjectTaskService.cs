using Core.Enums;
using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectTaskService : IGenericService<ProjectTask>
{
    // Task<List<ProjectTask>> GetTasksByProject(Project project);

    // Task<ProjectTask> GetTaskAfterCreating();
    Task AddFileToDirectory(string sourceFilePath, ProjectTask projectTask);
    
    Task<List<ProjectTask>> GetTasksByDeveloper(AppUser developer);

    Task<List<ProjectTask>> GetTasksAnotherDeveloper(AppUser developer);

    Task<List<ProjectTask>> GetTasksByTester(AppUser tester);
    
    Task<List<ProjectTask>> GetWaitTasksByTester(AppUser tester);
    
    Task<List<ProjectTask>> GetApproveTasks(Project project);

    Task<Priority> GetPriority(int choice, Priority priority);

    Task DeleteTesterFromTasksAsync(AppUser tester);

    Task<ProjectTask> GetTaskByName(string taskName);

    Task<bool> ProjectTaskIsAlreadyExist(string userInput);

    Task UpdateDueDateInTaskAsync(ProjectTask task, string[] date);

    Task DeleteTasksWithProject(Project project);

    Task DeleteTask(ProjectTask task);

    Task DeleteDeveloperFromTasksAsync(List<ProjectTask> tasks);

    Task<DateTime> CreateDueDateForTask(Project project, string[] date);
    
    Task<ProjectTask> CreateTaskWithoutTesterAndStakeHolderTestAsync(ProjectTask projectTask);

    Task<ProjectTask> CreateTaskAsync(string taskName, string taskDescription, DateTime term,
        Priority priority, AppUser tester, AppUser stakeHolder, Project project);

    Task<AppUser> GetDeveloperFromTask(ProjectTask task);

    Task<AppUser> GetTesterFromTask(ProjectTask task);
}