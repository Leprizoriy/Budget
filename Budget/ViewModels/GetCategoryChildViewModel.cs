using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.ViewModels
{
    public class GetCategoryChildViewModel : BindableBase
    {
        private double doll;
        private double euro;
        private double hrn;
        private double pln;
        private string parentCategoryName;
        private string name;
        private double dollSumParent;
        private double euroSumParent;
        private double hrnSumParent;
        private double plnSumParent;

        public int Id { get; set; }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string ParentCategoryName { get => parentCategoryName; set => SetProperty(ref parentCategoryName, value); }
        public double Usd { get => doll; set => SetProperty(ref doll, value); }
        public double Eur { get => euro; set => SetProperty(ref euro, value); }
        public double Uah { get => hrn; set => SetProperty(ref hrn, value); }
        public double Pln { get => pln; set => SetProperty(ref pln, value); }
        public int UsdParentCategoryId { get; set; }
        public int EurParentCategoryId { get; set; }
        public int UahParentCategoryId { get; set; }
        public int PlnParentCategoryId { get; set; }
        public double UsdSumParent { get => dollSumParent; set => SetProperty(ref dollSumParent, value); }
        public double EurSumParent { get => euroSumParent; set => SetProperty(ref euroSumParent, value); }
        public double UahSumParent { get => hrnSumParent; set => SetProperty(ref hrnSumParent, value); }
        public double PlnSumParent { get => plnSumParent; set => SetProperty(ref plnSumParent, value); }

        public override string ToString()
        {
            return Name;
        }
    }
}
