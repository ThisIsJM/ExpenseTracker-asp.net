using ExpenseTracker.Data.Enum;
using ExpenseTracker.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.ViewModels
{
    public class CreateTransactionViewModel
    {
        public int Id { get; set; } = 1;
        [Display(Name = "Transaction Type")]
        public TransactionType Type { get; set; }
        [Display(Name = "Input Description")]
        public string Description { get; set; }
        [Display(Name = "Input Amount")]
        public int Amount { get; set; }
        [Display(Name = "Input Date")]
        public DateTime Date { get; set; }
    }
}
