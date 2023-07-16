using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class UserTaskService : GenericService<UserTask>, IUserTaskService
{
    public UserTaskService(IRepository<UserTask> repository) : base(repository)
    {
    }
    
    public async Task<bool> IsUserInTask(Guid userId, Guid taskId)
    {
        if (userId == Guid.Empty) throw new Exception(nameof(userId));
        if (taskId == Guid.Empty) throw new Exception(nameof(taskId));

        try
        {
            var userTask = await GetUserTaskByUserIdAndTaskId(userId, taskId);
        
            return userTask != null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    public async Task<UserTask> GetUserTaskByUserIdAndTaskId(Guid userId, Guid taskId)
    {
        if (userId == Guid.Empty) throw new Exception(nameof(userId));
        if (taskId == Guid.Empty) throw new Exception(nameof(taskId));

        try
        {
            var userTask = await GetAll();
            var getUserTask = userTask.FirstOrDefault(up => up.UserId == userId && up.ProjectTaskId == taskId);

            return getUserTask!;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    public async Task AddUserTask(AppUser user, ProjectTask task)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (task == null) throw new ArgumentNullException(nameof(task));

        try
        {
            if (!await IsUserInTask(user.Id, task.Id))
            {
                var userTask = new UserTask()
                {
                    UserId = user.Id,
                    ProjectTaskId = task.Id,
                    User = user,
                    ProjectTask = task
                };
                await Add(userTask);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}