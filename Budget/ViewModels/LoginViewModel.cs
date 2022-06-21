using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
