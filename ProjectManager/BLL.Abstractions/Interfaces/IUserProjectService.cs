using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserProjectService : IGenericService<UserProject>
{
    Task<UserProject> GetUserAndProject(Guid projectId);

    Task<bool> IsUserInProject(Guid userId, Guid projectId);

    Task<UserProject> GetUserProjectByUserIdAndProjectId(Guid userId, Guid projectId);

    Task AddUserProject(User user, Project project);
}