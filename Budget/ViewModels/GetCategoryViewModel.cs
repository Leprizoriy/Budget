using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.ViewModels
{
    public class GetCategoryViewModel : BindableBase
    {
        private string name;

        public int Id { get; set; }
        public string Currency { get; set; }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public int ParentId { get; set; }
        public List<GetCategoryChildViewModel> Children { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
