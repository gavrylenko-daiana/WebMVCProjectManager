using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class UserProjectService : GenericService<UserProject>, IUserProjectService
{
    public UserProjectService(IRepository<UserProject> repository) : base(repository)
    {
    }

    public async Task<UserProject> GetUserAndProject(Guid projectId)
    {
        if (projectId == Guid.Empty) throw new Exception(nameof(projectId));
        
        try
        {
            var userProject = await GetByPredicate(up => up.ProjectId == projectId);

            return userProject;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> IsUserInProject(Guid userId, Guid projectId)
    {
        if (userId == Guid.Empty) throw new Exception(nameof(userId));
        if (projectId == Guid.Empty) throw new Exception(nameof(projectId));

        try
        {
            var userProject = await GetUserProjectByUserIdAndProjectId(userId, projectId);
        
            return userProject != null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    public async Task<UserProject> GetUserProjectByUserIdAndProjectId(Guid userId, Guid projectId)
    {
        if (userId == Guid.Empty) throw new Exception(nameof(userId));
        if (projectId == Guid.Empty) throw new Exception(nameof(projectId));

        try
        {
            var userProject = await GetAll();
            var getUserProject = userProject.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);

            return getUserProject!;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task AddUserProject(User user, Project project)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (project == null) throw new ArgumentNullException(nameof(project));

        try
        {
            if (!await IsUserInProject(user.Id, project.Id))
            {
                var userProject = new UserProject
                {
                    UserId = user.Id,
                    ProjectId = project.Id,
                    User = user,
                    Project = project
                };
                await Add(userProject);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}