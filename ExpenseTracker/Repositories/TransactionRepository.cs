using ExpenseTracker.Data;
using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TransactionRepository(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        //CRUD OPERATIONS
        public bool Add(Transaction transaction)
        {
            context.Transactions.Add(transaction);
            return Save();
        }

        public bool Update(Transaction transaction)
        {
            context.Update(transaction);
            return Save();
        }

        public bool Delete(Transaction transaction)
        {
            context.Remove(transaction);
            return Save();
        }

        public bool Save()
        {
            return context.SaveChanges() > 0;
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await context.Transactions.Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Transaction>> GetAllUserTransactions()
        {
            string userId = userManager.GetUserId(httpContextAccessor.HttpContext?.User);

            return context.Transactions.Where(t => t.UserId == userId).ToList();
        }

        public async Task<List<Transaction>> GetThisWeekUserTransactions()
        {
            string userId = userManager.GetUserId(httpContextAccessor.HttpContext?.User);
            DateTime oneWeekAgo = DateTime.Today.AddDays(-7);

            return context.Transactions.Where(t => t.UserId == userId && t.Date >= oneWeekAgo && t.Date <= DateTime.Today).ToList();

        }

        public async Task<List<Transaction>> GetThisMonthUserTransactions()
        {
            string userId = userManager.GetUserId(httpContextAccessor.HttpContext?.User);
            DateTime oneWeekAgo = DateTime.Today.AddDays(-30);

            return context.Transactions.Where(t => t.UserId == userId && t.Date >= oneWeekAgo && t.Date <= DateTime.Today).ToList();
        }
    }
}
