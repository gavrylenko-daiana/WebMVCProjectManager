using System.Security.Claims;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.ViewModels;

namespace UI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
                return RedirectToAction("Index", "Home"); // CHANGE!
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

    // [HttpPost]
    // public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    // {
    //     if (!ModelState.IsValid) return View(registerViewModel);
    //
    //     var user = await _userManager.FindByNameAsync(registerViewModel.Username);
    //     
    //     if (user != null)
    //     {
    //         TempData["Error"] = "This username is already in use";
    //         return View(registerViewModel);
    //     }
    //     
    //     user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
    //     
    //     if (user != null)
    //     {
    //         TempData["Error"] = "This username is already in use";
    //         return View(registerViewModel);
    //     }
    //
    //     
    //     var authClaim = new List<Claim>
    //     {
    //         new Claim(ClaimTypes.Name, user.UserName)
    //     };
    //     
    // }
}