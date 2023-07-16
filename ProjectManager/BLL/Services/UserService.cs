using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class UserService : GenericService<User>, IUserService
{
    private readonly IProjectTaskService _projectTaskService;
    
    public UserService(IRepository<User> repository, IProjectTaskService projectTaskService) : base(repository)
    {
        _projectTaskService = projectTaskService;
    }

    public async Task<bool> UsernameIsAlreadyExist(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) throw new ArgumentNullException(nameof(userInput));
        
        try
        {
            var check = (await GetAll()).Any(u => u.Username == userInput || u.Email == userInput);

            return check;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
 
    public async Task<User> Authenticate(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) throw new ArgumentNullException(nameof(userInput));
        
        try
        {
            User user = await GetByPredicate(u => u.Username == userInput || u.Email == userInput);

            if (user == null) throw new ArgumentNullException(nameof(user));

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<User> GetUserByUsernameOrEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        
        try
        {
            User user = await GetByPredicate(u => u.Username == input || u.Email == input);
        
            if (user == null) throw new ArgumentNullException(nameof(user));

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdatePassword(Guid userId, string newPassword)
    {
        if (userId == Guid.Empty) throw new ArgumentException("userId cannot be empty");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentNullException(nameof(newPassword));

        try
        {
            User user = await GetById(userId);

            if (user == null) throw new ArgumentNullException(nameof(user));

            user.PasswordHash = GetPasswordHash(newPassword);
            await Update(userId, user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> SendCodeToUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        
        try
        {
            Random rand = new Random();
            int emailCode = rand.Next(1000, 9999);
            string fromMail = "dayana01001@gmail.com";
            string fromPassword = "oxizguygokwxgxgb";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Verify code for update password.";
            message.To.Add(new MailAddress($"{email}"));
            message.Body = $"<html><body> Your code: {emailCode} </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);

            return emailCode;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task SendMessageEmailUserAsync(string email, string messageEmail)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(messageEmail)) throw new ArgumentNullException(nameof(messageEmail));

        try
        {
            string fromMail = "dayana01001@gmail.com";
            string fromPassword = "oxizguygokwxgxgb";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Change notification.";
            message.To.Add(new MailAddress($"{email}"));
            message.Body = $"<html><body> {messageEmail} </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task AddFileFromUserAsync(string path, ProjectTask projectTask)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
        if (projectTask == null) throw new ArgumentNullException(nameof(projectTask));
        
        try
        {
            await _projectTaskService.AddFileToDirectory(path, projectTask);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<ProjectTask> GetTaskByNameAsync(string taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException(nameof(taskName));

        try
        {
            var task = await _projectTaskService.GetTaskByName(taskName);

            if (task == null) throw new ArgumentNullException(nameof(task));

            return task;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task AddNewUser(string userName, string userEmail, string password, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
        if (string.IsNullOrWhiteSpace(userEmail)) throw new ArgumentNullException(nameof(userEmail));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new InvalidEnumArgumentException(nameof(role), (int)role, typeof(UserRole));

        try
        {
            await Add(new User
            {
                Username = userName,
                Email = userEmail,
                PasswordHash = GetPasswordHash(password),
                Role = role
            });
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<UserRole> GetRole(UserRole role, int choice)
    {
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new InvalidEnumArgumentException(nameof(role), (int)role, typeof(UserRole));
        if (choice <= 0 && choice > 4) throw new ArgumentOutOfRangeException(nameof(choice));
        
        try
        {
            role = choice switch
            {
                1 => UserRole.StakeHolder,
                2 => UserRole.Developer,
                3 => UserRole.Tester,
                4 => UserRole.User,
            };

            return role;
        }
        catch
        {
            throw new Exception("Such a type of subscription does not exist!");
        }
    }
}