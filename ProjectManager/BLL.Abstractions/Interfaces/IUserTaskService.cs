using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserTaskService : IGenericService<UserTask>
{
    Task AddUserTask(AppUser user, ProjectTask task);

    Task<UserTask> GetUserTaskByUserIdAndTaskId(Guid userId, Guid taskId);

    Task<bool> IsUserInTask(Guid userId, Guid taskId);
}