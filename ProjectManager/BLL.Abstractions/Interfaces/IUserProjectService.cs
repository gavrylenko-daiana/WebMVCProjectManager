using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserProjectService : IGenericService<UserProject>
{
    Task<UserProject> GetUserAndProject(Guid projectId);

    Task<bool> IsUserInProject(string? userId, Guid projectId);

    Task<UserProject> GetUserProjectByUserIdAndProjectId(string? userId, Guid projectId);

    Task AddUserProject(AppUser user, Project project);
}