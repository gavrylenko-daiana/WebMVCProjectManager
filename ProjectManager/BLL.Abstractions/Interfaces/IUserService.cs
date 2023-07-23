using Core.Enums;
using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserService : IGenericService<AppUser>
{
    Task<bool> UsernameIsAlreadyExist(string name);
    
    Task<AppUser> Authenticate(string userInput);

    Task<AppUser> GetUserByUsernameOrEmail(string input);

    Task UpdatePassword(Guid getUserId, string newUserPassword);

    Task<int> SendCodeToUser(string email);

    Task SendMessageEmailUserAsync(string email, string messageEmail);

    Task AddFileFromUserAsync(string path, ProjectTask projectTask);

    Task<ProjectTask> GetTaskByNameAsync(string taskName);

    Task AddNewUser(string userName, string userEmail, string password, UserRole role);
}
