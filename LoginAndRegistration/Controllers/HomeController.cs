using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LoginAndRegistration.Models;
using Microsoft.AspNetCore.Identity;

namespace LoginAndRegistration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    //! Index -> Login & Registrations Forms
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    //! Display Success View
    [SessionCheck]
    [HttpGet("success")]
    public ViewResult Success()
    {
        return View();
    }

    //! Register User
    [HttpPost("users/create")]
    public IActionResult Register(User newUser)
    {
        if(!ModelState.IsValid)
        {
            return View("Index");
        }

        PasswordHasher<User> Hasher = new();
        newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

        _context.Add(newUser);
        _context.SaveChanges();
        
        HttpContext.Session.SetInt32("UserId", newUser.UserId);
        return RedirectToAction("Success");
    }

    //! Login User
    [HttpPost("users/login")]
    public IActionResult Login(LoginUser loginForm)
    {
        if(!ModelState.IsValid)
        {
            return View("Index");
        }

        User? UserInDb = _context.Users.FirstOrDefault(u => u.Email == loginForm.LogEmail);

        if(UserInDb == null)
        {
            ModelState.AddModelError("Email", "Invalid Email/Password");
            return View("Index");
        }

        PasswordHasher<LoginUser> Hasher = new();
        var result = Hasher.VerifyHashedPassword(loginForm, UserInDb.Password, loginForm.LogPassword);

        if(result == 0)
        {
            ModelState.AddModelError("Email", "Invalid Email/Password");
            return View("Index");
        }

        HttpContext.Session.SetInt32("UserId", UserInDb.UserId);
        return RedirectToAction("Success");
    }

    //! Logout User
    [HttpGet("users/logout")]
    public RedirectToActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

//! Check if User is in Session
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {

        int? userId = context.HttpContext.Session.GetInt32("UserId");

        if(userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}

