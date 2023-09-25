using ExpenseTracker.Data;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITransactionRepository transactionRepository;

        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, ITransactionRepository transactionRepository)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.transactionRepository = transactionRepository;
        }

        public IActionResult Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            DashboardViewModel dashboardVM = new DashboardViewModel()
            {
                Username = User.Identity.Name,
                Transactions = await transactionRepository.GetAllUserTransactions()
            };

            return View(dashboardVM);
        }

        public async Task<IActionResult> Analysis()
        {
            if (!signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DashboardViewModel dashboardVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Inputs";
                return RedirectToAction("Dashboard");
            }

            var transaction = new Transaction()
            {
                Type = dashboardVM.CreateTransactionVM.Type,
                Description = dashboardVM.CreateTransactionVM.Description,
                Amount = dashboardVM.CreateTransactionVM.Amount,
                Date = dashboardVM.CreateTransactionVM.Date,
                UserId = userManager.GetUserId(httpContextAccessor.HttpContext?.User)
            };

            bool succcess = transactionRepository.Add(transaction);

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DashboardViewModel dashboardVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Inputs";
                return RedirectToAction("Dashboard");
            }

            var transaction = new Transaction()
            {
                Id = dashboardVM.CreateTransactionVM.Id,
                Type = dashboardVM.CreateTransactionVM.Type,
                Description = dashboardVM.CreateTransactionVM.Description,
                Amount = dashboardVM.CreateTransactionVM.Amount,
                Date = dashboardVM.CreateTransactionVM.Date,
                UserId = userManager.GetUserId(httpContextAccessor.HttpContext?.User)
            };

            bool succcess = transactionRepository.Update(transaction);

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var transactionDetails = await transactionRepository.GetByIdAsync(id);
            bool succcess = transactionRepository.Delete(transactionDetails);

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("Home/GetTransactionAsync")]
        public async Task<IActionResult> GetTransactionAsync(int id)
        { 
            Transaction transaction = await transactionRepository.GetByIdAsync(id);
            return Json(transaction);
        }

        [HttpGet]
        [Route("Home/GetThisWeekTransactions")]
        public async Task<IActionResult> GetThisWeekTransactions()
        {
            List<Transaction> transactions = await transactionRepository.GetThisWeekUserTransactions();
            return Json(transactions);
        }

        [HttpGet]
        [Route("Home/GetThisMonthTransactions")]
        public async Task<IActionResult> GetThisMonthTransactions()
        {
            List<Transaction> transactions = await transactionRepository.GetThisMonthUserTransactions();
            return Json(transactions);
        }

        [HttpGet]
        [Route("Home/GetAllTransactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            List<Transaction> transactions = await transactionRepository.GetAllUserTransactions();
            return Json(transactions);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
             
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}