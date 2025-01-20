using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nagrand.Models;
using Nagrand.Services;
using System.Security.Claims;

namespace Nagrand.Controllers;
public class AccountController : Controller
{
    private readonly UserService _userService;

    public AccountController(UserService userService)
    {
        _userService = userService;
    }

    // Страница регистрации
    public IActionResult Register()
    {
        return View();
    }

    // Обработка регистрации
    [HttpPost]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            bool isRegistered = _userService.RegisterUser(user);
            if (isRegistered)
                return RedirectToAction("Login");

            ModelState.AddModelError("", "Пользователь с таким именем уже существует.");
        }
        return View(user);
    }

    // Страница входа
    public IActionResult Login()
    {
        return View();
    }

    // Обработка входа
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        bool isAuthenticated = _userService.AuthenticateUser(username, password);
        if (isAuthenticated)
        {
            // Создание куки для аутентификации
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Установка аутентификации
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Неверное имя пользователя или пароль.");
        return View();
    }

    // Страница выхода
    public async Task<IActionResult> Logout()
    {
        // Завершаем сессию (выход)
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }
}