using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class LoginViewModel
    {
        public string EmailAddress {  get; set; }
        public string Password { get; set; }
    }
}
