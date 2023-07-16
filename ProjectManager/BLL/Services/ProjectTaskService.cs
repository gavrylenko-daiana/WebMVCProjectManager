using System.ComponentModel;
using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectTaskService : GenericService<ProjectTask>, IProjectTaskService
{
    private readonly ITaskFileService _taskFile;
    private readonly IUserTaskService _userTask;
    private readonly IUserProjectService _userProjectService;

    public ProjectTaskService(IRepository<ProjectTask> repository, ITaskFileService taskFile, IUserTaskService userTask,
        IUserProjectService userProjectService) : base(repository)
    {
        _taskFile = taskFile;
        _userTask = userTask;
        _userProjectService = userProjectService;
    }

    public async Task<bool> ProjectTaskIsAlreadyExist(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) throw new ArgumentNullException(nameof(userInput));

        try
        {
            var check = (await GetAll()).Any(p => p.Name == userInput);

            return check;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task AddFileToDirectory(string sourceFilePath, ProjectTask projectTask)
    {
        if (projectTask == null) throw new ArgumentNullException(nameof(projectTask));
        if (string.IsNullOrWhiteSpace(sourceFilePath)) throw new ArgumentNullException(nameof(sourceFilePath));

        try
        {
            const string pathToFolder = "DirectoryForUser";

            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }

            string fileName = Path.GetFileName(sourceFilePath);
            string destinationFilePath = Path.Combine(pathToFolder, fileName);

            if (File.Exists(destinationFilePath))
            {
                int count = 1;
                string fileNameOnly = Path.GetFileNameWithoutExtension(destinationFilePath);
                string extension = Path.GetExtension(destinationFilePath);
                string newFullPath = destinationFilePath;

                while (File.Exists(newFullPath))
                {
                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                    newFullPath = Path.Combine(pathToFolder, tempFileName + extension);
                }

                File.Copy(sourceFilePath, newFullPath);
                var taskFile = await _taskFile.AddNewFileAsync(newFullPath);
                await _taskFile.UpdateTaskFile(taskFile);
                projectTask.UploadedFiles.Add(taskFile);
                await Update(projectTask.Id, projectTask);
            }
            else
            {
                File.Copy(sourceFilePath, destinationFilePath);
                var taskFile = await _taskFile.AddNewFileAsync(destinationFilePath);
                await _taskFile.UpdateTaskFile(taskFile);
                projectTask.UploadedFiles.Add(taskFile);
                await Update(projectTask.Id, projectTask);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<ProjectTask>> GetTasksByDeveloper(User developer)
    {
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        try
        {
            var tasks = (await GetAll()).Where(t =>
                t.AssignedUsers.Any(ut => ut.User.Role == UserRole.Developer) &&
                t.AssignedUsers.Any(ut => ut.UserId == developer.Id && t.Progress == Progress.InProgress)).ToList();

            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<ProjectTask>> GetTasksAnotherDeveloper(User developer)
    {
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        try
        {
            var tasks = await GetAll();

            var tasksToReview = tasks.Where(t =>
                    t.AssignedUsers.Any(ut =>
                        ut.User != null && ut.User.Role == UserRole.Developer && !ut.UserId.Equals(developer.Id)) &&
                    t.Progress == Progress.InProgress)
                .ToList();

            return tasksToReview;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<ProjectTask>> GetTasksByTester(User tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));

        try
        {
            var tasks = (await GetAll()).Where(t =>
                t.AssignedUsers.Any(ut => ut.User.Role == UserRole.Tester) &&
                t.AssignedUsers.Any(ut => ut.UserId == tester.Id)).ToList();

            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<ProjectTask>> GetWaitTasksByTester(User tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));

        try
        {
            var tasks = (await GetAll()).Where(t =>
                t.AssignedUsers.Any(ut => ut.User.Role == UserRole.Tester) &&
                t.AssignedUsers.Any(ut => ut.UserId == tester.Id && t.Progress == Progress.WaitingTester)).ToList();

            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<List<ProjectTask>> GetApproveTasks(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        try
        {
            var approveTasks = project.Tasks.Where(t => t.Progress == Progress.CompletedTask).ToList();

            return Task.FromResult(approveTasks);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Priority> GetPriority(int choice, Priority priority)
    {
        if (!Enum.IsDefined(typeof(Priority), priority))
            throw new InvalidEnumArgumentException(nameof(priority), (int)priority, typeof(Priority));
        if (choice <= 0 || choice >= 6) throw new ArgumentOutOfRangeException(nameof(choice));

        try
        {
            priority = choice switch
            {
                1 => Priority.Urgent,
                2 => Priority.High,
                3 => Priority.Medium,
                4 => Priority.Low,
                5 => Priority.Minor,
            };

            return priority;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteTesterFromTasksAsync(User tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));

        try
        {
            var tasks = await GetTasksByTester(tester);

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    var userTask = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Tester);
                    
                    if (userTask != null)
                    {
                        task.AssignedUsers.Remove(userTask);
                        await Update(task.Id, task);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteDeveloperFromTasksAsync(List<ProjectTask> tasks)
    {
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));

        try
        {
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    var userTask = task.AssignedUsers.FirstOrDefault(ut => ut.User.Role == UserRole.Developer);
                    
                    if (userTask != null)
                    {
                        task.AssignedUsers.Remove(userTask);
                        await Update(task.Id, task);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ProjectTask> GetTaskByName(string taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException(nameof(taskName));

        try
        {
            ProjectTask task = await GetByPredicate(t => t.Name == taskName);

            if (task == null) throw new ArgumentNullException(nameof(task));

            return task;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateDueDateInTaskAsync(ProjectTask task, string[] date)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (date == null) throw new ArgumentNullException(nameof(date));

        try
        {
            task.DueDates = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
            await Update(task.Id, task);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteTasksWithProject(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        try
        {
            if (project.Tasks.Count != 0)
            {
                var tasksCopy = project.Tasks.ToList();

                foreach (var task in tasksCopy)
                {
                    await Delete(task.Id);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteTask(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        try
        {
            await Delete(task.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DateTime> CreateDueDateForTask(Project project, string[] date)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (date == null) throw new ArgumentNullException(nameof(date));

        try
        {
            DateTime enteredDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            DateTime now = DateTime.Now;
            if (enteredDate < now)
            {
                enteredDate = now.AddDays(1);
            }

            DateTime term = enteredDate <= project.DueDates.Date
                ? enteredDate
                : project.DueDates.Date;

            return term;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ProjectTask> CreateTaskAsync(string taskName, string taskDescription, DateTime term,
        Priority priority, User tester, User stakeHolder, Project project)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (term == default(DateTime)) throw new ArgumentException("date cannot be empty");
        if (!Enum.IsDefined(typeof(Priority), priority))
            throw new InvalidEnumArgumentException(nameof(priority), (int)priority, typeof(Priority));
        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException(nameof(taskName));
        if (string.IsNullOrWhiteSpace(taskDescription)) throw new ArgumentNullException(nameof(taskDescription));

        try
        {
            var task = new ProjectTask
            {
                Name = taskName,
                Description = taskDescription,
                DueDates = term,
                Priority = priority,
                ProjectId = project.Id,
                Project = project
            };
            await Add(task);
            await _userTask.AddUserTask(stakeHolder, task);
            await _userTask.AddUserTask(tester, task);
            await _userProjectService.AddUserProject(tester, project);

            return task;
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

    public Task<User> GetTesterFromTask(ProjectTask task)
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