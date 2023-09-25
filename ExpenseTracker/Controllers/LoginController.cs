using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class LoginController : Controller
    {
        public readonly ApplicationDbContext context;
        public readonly UserManager<User> userManager;
        public readonly SignInManager<User> signInManager;

        public LoginController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            LoginViewModel loginVM = new LoginViewModel();
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginVM)
        { 
            if(!ModelState.IsValid) return View(loginVM);

            var user = await userManager.FindByEmailAsync(loginVM.EmailAddress);

            if (user != null)
            {
                var passwordCheck = await userManager.CheckPasswordAsync(user, loginVM.Password);

                if (passwordCheck)
                {
                    var result = await signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }

            TempData["Error"] = "Invalid Credentials";
            return View(loginVM);
        }

    }
}
