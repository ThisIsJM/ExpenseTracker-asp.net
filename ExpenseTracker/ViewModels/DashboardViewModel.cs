using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModels
{
    public class DashboardViewModel
    {
        public string Username { get; set; }
        public List<Transaction> Transactions { get; set; }
        public CreateTransactionViewModel CreateTransactionVM { get; set; }

        public int GetTotalIncome()
        {
            int total = 0;

            foreach (Transaction item in Transactions)
            {
                if (item.Type == Data.Enum.TransactionType.Income)
                {
                    total += item.Amount;
                }
            }

            return total;
        }

        public int GetTotalExpenses()
        {
            int total = 0;

            foreach (Transaction item in Transactions)
            {
                if (item.Type == Data.Enum.TransactionType.Expenses)
                {
                    total += item.Amount;
                }
            }

            return total;
        }

        public int GetTotalBalance()
        {
            return GetTotalIncome() - GetTotalExpenses();
        }
    }
}
