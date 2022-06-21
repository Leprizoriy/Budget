using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models
{
    public class User
    {
        public string Token { get; set; }
        public DateTime ExpirationTokenTime { get; set; }
        public LoginViewModel AssosiatedCredemtials { get; set; } = new LoginViewModel();
    }
}
