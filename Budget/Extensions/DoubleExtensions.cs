using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Extensions
{
    public static class DoubleExtensions
    {
        public static double TwoSymbolsAfterDot(this double amount)
        {
            return amount = Math.Round(amount, 2);
        }
    }
}
