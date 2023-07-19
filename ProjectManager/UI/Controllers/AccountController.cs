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

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
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
}