using System.Security.Claims;
using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserService _userService;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager, IUserService userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var login = new LoginViewModel();
        return View(login);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View(loginViewModel);

        var user = await _userManager.FindByNameAsync(loginViewModel.UsernameOrEmailAddress) ??
                   await _userManager.FindByEmailAsync(loginViewModel.UsernameOrEmailAddress);

        if (user == null)
        {
            TempData["Error"] = "Entered incorrect username or email. Please try again.";
            return View(loginViewModel);
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

        if (checkPassword)
        {
            var singInAttempt = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);

            if (singInAttempt.Succeeded)
            {
                return RedirectToAction("Index", "Project");
            }
        }

        TempData["Error"] = "Entered incorrect password. Please try again.";
        return View(loginViewModel);
    }

    [HttpGet]
    public IActionResult Register()
    {
        var register = new RegisterViewModel();
        return View(register);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return View(registerViewModel);

        var user = await _userManager.FindByNameAsync(registerViewModel.UserName);

        if (user != null)
        {
            TempData["Error"] = "This username is already in use";
            return View(registerViewModel);
        }

        user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

        if (user != null)
        {
            TempData["Error"] = "This username is already in use";
            return View(registerViewModel);
        }

        var newUser = new AppUser()
        {
            Email = registerViewModel.EmailAddress,
            UserName = registerViewModel.UserName,
            Role = registerViewModel.Role
        };

        var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

        if (!await _roleManager.RoleExistsAsync(UserRole.StakeHolder.ToString()))
            await _roleManager.CreateAsync(new IdentityRole(UserRole.StakeHolder.ToString()));

        if (!await _roleManager.RoleExistsAsync(UserRole.Tester.ToString()))
            await _roleManager.CreateAsync(new IdentityRole(UserRole.Tester.ToString()));

        if (!await _roleManager.RoleExistsAsync(UserRole.Developer.ToString()))
            await _roleManager.CreateAsync(new IdentityRole(UserRole.Developer.ToString()));

        if (newUserResponse.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(newUser, registerViewModel.Role.ToString());

            if (!roleResult.Succeeded) return View("Error");
        }

        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
        }
        catch
        {
            return View("Error");
        }

        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        var forgotPasswordViewModel = new ForgotPasswordBeforeEnteringViewModel();

        return View(forgotPasswordViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordBeforeEnteringViewModel forgotPasswordBeforeEnteringViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(forgotPasswordBeforeEnteringViewModel);
        }

        var user = await _userManager.FindByEmailAsync(forgotPasswordBeforeEnteringViewModel.Email);

        if (user == null)
        {
            TempData["Error"] = "You wrote an incorrect email. Try again!";

            return View(forgotPasswordBeforeEnteringViewModel);
        }

        var emailCode = await _userService.SendCodeToUser(forgotPasswordBeforeEnteringViewModel.Email);

        return RedirectToAction("CheckEmailCode",
            new { code = emailCode, email = forgotPasswordBeforeEnteringViewModel.Email });
    }

    [HttpGet]
    public IActionResult CheckEmailCode(int code, string email)
    {
        var forgotPasswordCodeViewModel = new ForgotPasswordBeforeEnteringViewModel()
        {
            Email = email,
        };

        TempData["Code"] = code;

        return View(forgotPasswordCodeViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CheckEmailCode(int code,
        ForgotPasswordBeforeEnteringViewModel forgotPasswordCodeViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(forgotPasswordCodeViewModel);
        }

        if (code == forgotPasswordCodeViewModel.EmailCode)
        {
            return RedirectToAction("ResetPassword", new { email = forgotPasswordCodeViewModel.Email });
        }
        else
        {
            TempData["Error"] = "Invalid code. Please try again.";

            return RedirectToAction("Login", "Account");
        }
    }

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        TempData["SuccessMessage"] = "Code is valid. You can reset your password.";

        var resetPassword = new NewPasswordViewModel()
        {
            Email = email
        };

        return View(resetPassword);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(NewPasswordViewModel newPasswordViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(newPasswordViewModel);
        }

        var user = await _userManager.FindByEmailAsync(newPasswordViewModel.Email);
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPasswordViewModel.NewPassword);
        await _userManager.UpdateAsync(user);
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }
}