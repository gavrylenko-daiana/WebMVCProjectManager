using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserTaskService : IGenericService<UserTask>
{
    Task AddUserTask(AppUser user, ProjectTask task);

    Task<UserTask> GetUserTaskByUserIdAndTaskId(string? userId, Guid taskId);

    Task<bool> IsUserInTask(string? userId, Guid taskId);
}