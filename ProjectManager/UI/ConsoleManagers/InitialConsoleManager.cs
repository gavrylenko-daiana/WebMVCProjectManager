using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class InitialConsoleManager : ConsoleManager<IUserService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userConsoleManager;
    private readonly StakeHolderConsoleManager _stakeHolderManager;
    private readonly DeveloperConsoleManager _developerConsoleManager;
    private readonly TesterConsoleManager _testerConsoleManager;

    public InitialConsoleManager(IUserService userService, StakeHolderConsoleManager stakeHolderConsoleManager,
        UserConsoleManager userConsoleManager, DeveloperConsoleManager developerConsoleManager,
        TesterConsoleManager testerConsoleManager) : base(userService)
    {
        _stakeHolderManager = stakeHolderConsoleManager;
        _userConsoleManager = userConsoleManager;
        _developerConsoleManager = developerConsoleManager;
        _testerConsoleManager = testerConsoleManager;
    }

    public async Task AuthenticateUser()
    {
        try
        {
            Console.WriteLine("Please, write your username or email.\nUsername or Email: ");
            string userInput = Console.ReadLine()!;
            Console.Write("Please, write your password.(if you forgot your password, write - 'forgot')\nPassword: ");
            string password = Console.ReadLine()!;

            if (password == "forgot")
            {
                Console.Write("Please, write your email that was linked by you for update you password.\nEmail: ");
                var email = Console.ReadLine()!;

                try
                {
                    var user = await Service.GetUserByUsernameOrEmail(email);
                    await _userConsoleManager.ForgotUserPassword(user);
                    return;
                }
                catch
                {
                    Console.WriteLine("User with this email is not registered.");
                }
            }

            User getUser = await Service.Authenticate(userInput);

            if (getUser.PasswordHash == Service.GetPasswordHash(password))
            {
                if (getUser.Role == UserRole.User) await _userConsoleManager.PerformOperationsAsync(getUser);
                if (getUser.Role == UserRole.StakeHolder) await _stakeHolderManager.PerformOperationsAsync(getUser);
                if (getUser.Role == UserRole.Developer) await _developerConsoleManager.PerformOperationsAsync(getUser);
                if (getUser.Role == UserRole.Tester) await _testerConsoleManager.PerformOperationsAsync(getUser);
            }
            else
            {
                Console.WriteLine("You entered the wrong password!");
            }
        }
        catch
        {
            Console.WriteLine("You entered an invalid username or email!");
        }
    }

    public async Task CreateUserAsync()
    {
        try
        {
            Console.WriteLine("Create user");
            Console.Write("Please, write your username.\nUsername: ");
            string userName = Console.ReadLine()!;

            if (!await _userConsoleManager.UserUniquenessCheck(userName)) return;

            Console.Write("Please, write your email.\nEmail: ");
            string userEmail = Console.ReadLine()!;

            if (!await _userConsoleManager.UserUniquenessCheck(userEmail)) return;

            Console.Write("Please, write your password.\nPassword: ");
            string password = Console.ReadLine()!;
            UserRole role = await SelectRoleUser();
            Console.WriteLine("User was successfully added");
            await Service.AddNewUser(userName, userEmail, password, role);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private async Task<UserRole> SelectRoleUser()
    {
        try
        {
            UserRole role = UserRole.Developer;
            Console.WriteLine("Enter role for user: \n1) StakeHolder; 2) Developer; 3) Tester;");
            int choice = int.Parse(Console.ReadLine()!);
            role = await Service.GetRole(role, choice);

            return role;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public override async Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}