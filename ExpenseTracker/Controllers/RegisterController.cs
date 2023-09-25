using ExpenseTracker.Data;
using ExpenseTracker.Data.Enum;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class RegisterController : Controller
    {
        public readonly ApplicationDbContext context;
        public readonly UserManager<User> userManager;
        public readonly SignInManager<User> signInManager;

        public RegisterController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            RegisterViewModel registerVM = new RegisterViewModel();
            return View(registerVM);
        }


        [HttpPost]
        public async Task<IActionResult> Index(RegisterViewModel registerVM)
        {
            //VALIDATE VM
            if(!ModelState.IsValid) return View(registerVM);

            //CHECK IF EMAIL ALREADY EXISTS
            var user = await userManager.FindByEmailAsync(registerVM.EmailAddress);

            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerVM);
            }

            //CREATE ACCOUNT
            var newUser = new User() { Email = registerVM.EmailAddress, UserName = registerVM.EmailAddress };
            var newUserResponse = await userManager.CreateAsync(newUser, registerVM.Password);

            if (!newUserResponse.Succeeded)
            {
                var errorMessages = string.Join("\n", newUserResponse.Errors.Select(error => error.Description));
                TempData["Error"] = errorMessages;
                return View(registerVM);
            }

            //REDIRECT TO LOGIN PAGE
            return RedirectToAction("Index", "Login");

        }
    }
}
