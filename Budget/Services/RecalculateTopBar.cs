using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Services
{
    public interface RecalculateCallBack
    {
        void Recalculate(List<GetTransactionViewModel> transactions);
    }

    public class RecalculateTopBarService
    {
        public static RecalculateCallBack recalculateCallBack;

        public static void RecalculateTopBar(List<GetTransactionViewModel> transactions)
        {
            recalculateCallBack.Recalculate(transactions);
        }
    }
}
