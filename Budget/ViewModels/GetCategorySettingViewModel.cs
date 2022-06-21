using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.ViewModels
{
    public class GetCategorySettingViewModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string CategoryName { get; set; }
    }
}
