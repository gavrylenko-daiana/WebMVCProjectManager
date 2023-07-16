using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class DeveloperConsoleManager : ConsoleManager<IDeveloperService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userConsoleManager;
    private readonly ProjectConsoleManager _projectManager;
    private readonly ProjectTaskConsoleManager _projectTaskManager;

    public DeveloperConsoleManager(IDeveloperService service, UserConsoleManager userConsoleManager,
        ProjectConsoleManager projectManager, ProjectTaskConsoleManager projectTaskManager) : base(service)
    {
        _userConsoleManager = userConsoleManager;
        _projectManager = projectManager;
        _projectTaskManager = projectTaskManager;
    }

    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", DisplayDeveloperAsync },
            { "2", UpdateDeveloperAsync },
            { "3", AssignTasksToDeveloperAsync },
            { "4", SendToSubmitByTesterAsync },
            { "5", AddFileToTask },
            { "6", DeleteDeveloperAsync }
        };

        while (true)
        {
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display information about you");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Select tasks");
            Console.WriteLine("4. Submit a task for review");
            Console.WriteLine("5. Add file to task");
            Console.WriteLine("6. Delete your account");
            Console.WriteLine("7. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "6")
            {
                await actions[input](user);
                break;
            }

            if (input == "7") break;
            if (actions.ContainsKey(input)) await actions[input](user);
            else Console.WriteLine("Invalid operation number.");
        }
    }

    private async Task AssignTasksToDeveloperAsync(User developer)
    {
        try
        {
            await _projectManager.DisplayAllProjectsAsync();
            Console.WriteLine("\nWrite the name of the project from which you want to take tasks.");
            var projectName = Console.ReadLine()!;
            var project = await Service.GetProjectByNameAsync(projectName);
            var tasks = project.Tasks;

            if (tasks.Count != 0)
            {
                await _projectTaskManager.DisplayAllTaskByProjectPlanned(tasks);

                foreach (var task in tasks)
                {
                    if (task.Progress == Progress.Planned)
                    {
                        Console.WriteLine(
                            $"Can {developer.Username} take task {task.Name}?\nPlease, write '1' - yes or '2' - no");
                        var choice = int.Parse(Console.ReadLine()!);

                        if (choice == 1)
                        {
                            await Service.TakeTaskByDeveloper(task, developer, project);
                            await _projectManager.UpdateAsync(project.Id, project);
                            await Service.SendMailToUserAsync(developer.Email, "The task has been changed from Planned to InProgress");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Task list is empty!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Assign tasks to Developer failed");
        }
    }

    private async Task SendToSubmitByTesterAsync(User developer)
    {
        try
        {
            var tasks = await Service.GetTasksAnotherDeveloperAsync(developer);

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    if (task.Progress == Progress.InProgress)
                    {
                        await _projectTaskManager.DisplayTaskAsync(task);
                        Console.WriteLine($"Are you wanna send to submit this task?\n1 - Yes, 2 - No");
                        var option = int.Parse(Console.ReadLine()!);

                        if (option == 1)
                        {
                            await Service.UpdateProgressToWaitTester(task);
                            await Service.SendMailToUserAsync(developer.Email,
                                $"The task - {task.Name} has been changed from InProgress to WaitingTester");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private async Task DisplayDeveloperAsync(User developer)
    {
        try
        {
            Console.WriteLine($"\nUsername: {developer.Username}");
            Console.WriteLine($"Email: {developer.Email}");

            var tasks = await Service.GetDeveloperTasks(developer);

            if (tasks.Any())
            {
                Console.WriteLine($"Your current task(s): ");
                
                foreach (var task in tasks)
                {
                    Console.WriteLine(task.Name);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private async Task UpdateDeveloperAsync(User developer)
    {
        try
        {
            await _userConsoleManager.UpdateUserAsync(developer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task AddFileToTask(User developer)
    {
        try
        {
            var task = await _userConsoleManager.AddFileToTaskAsync();
            await Service.UpdateProjectByTask(task);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DeleteDeveloperAsync(User developer)
    {
        try
        {
            Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
            int choice = int.Parse(Console.ReadLine()!);

            if (choice == 1)
            {
                await Service.DeleteDeveloperFromTasks(developer);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}