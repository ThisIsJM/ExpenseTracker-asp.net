using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models
{
    public class User : IdentityUser
    {
        public ICollection<Transaction> Transactions { get; set; }
    }
}
