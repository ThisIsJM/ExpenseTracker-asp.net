
using ExpenseTracker.Models;

namespace ExpenseTracker.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllUserTransactions();
        Task<List<Transaction>> GetThisWeekUserTransactions();
        Task<List<Transaction>> GetThisMonthUserTransactions();
        Task<Transaction> GetByIdAsync(int id);
        bool Add(Transaction transaction);
        bool Update(Transaction transaction);
        bool Delete(Transaction transaction);
        bool Save();
    }
}
