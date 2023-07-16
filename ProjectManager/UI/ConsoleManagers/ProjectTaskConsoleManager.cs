using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectTaskConsoleManager : ConsoleManager<IProjectTaskService, ProjectTask>, IConsoleManager<ProjectTask>
{
    public ProjectTaskConsoleManager(IProjectTaskService service) : base(service)
    {
    }

    public async Task DisplayTaskAsync(ProjectTask task)
    {
        try
        {
            var taskDeveloperEmail = await Service.GetDeveloperFromTask(task);
            var taskTesterEmail = await Service.GetTesterFromTask(task);
            Console.WriteLine($"\nName: {task.Name}");

            if (!string.IsNullOrWhiteSpace(task.Description))
                Console.WriteLine($"Description: {task.Description}");

            if (taskDeveloperEmail != null)
                Console.WriteLine($"Developer performing task: {taskDeveloperEmail.Username}");

            Console.WriteLine($"Tester: {taskTesterEmail.Username}");
            Console.WriteLine($"Priority: {task.Priority}");
            Console.WriteLine($"DueDates: {task.DueDates.Date}");
            Console.WriteLine($"Status: {task.Progress}\n");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task DisplayAllTaskByProject(List<ProjectTask> tasks)
    {
        try
        {
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    await DisplayTaskAsync(task);
                }
            }
            else
            {
                Console.WriteLine("Tasks list is empty");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    public async Task DisplayAllTaskByProjectPlanned(List<ProjectTask> tasks)
    {
        try
        {
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    if (task.Progress == Progress.Planned)
                        await DisplayTaskAsync(task);
                }
            }
            else
            {
                Console.WriteLine("Tasks list is empty");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task DisplayAllTasks()
    {
        try
        {
            var tasks = await GetAllAsync();
            var projectTasks = tasks.ToList();

            if (projectTasks.Any())
            {
                foreach (var task in projectTasks)
                {
                    await DisplayTaskAsync(task);
                }
            }
            else
            {
                throw new Exception("Tasks list is empty");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}