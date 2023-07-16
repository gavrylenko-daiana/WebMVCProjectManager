using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public class DependencyRegistration
{
    public static void RegisterServices(IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectTaskService, ProjectTaskService>();
        services.AddScoped<IDeveloperService, DeveloperService>();
        services.AddScoped<ITesterService, TesterService>();
        services.AddScoped<IStakeHolderService, StakeHolderService>();
        services.AddScoped<ITaskFileService, TaskFileService>();
        services.AddScoped<IUserProjectService, UserProjectService>();
        services.AddScoped<IUserTaskService, UserTaskService>();

        DAL.DependencyRegistration.RegisterRepositories(services, connectionString);
    }
}