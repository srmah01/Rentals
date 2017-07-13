using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.WPFClient.Modules.ViewModels
{
    public class FooterViewModel : BindableBase
    {
        private string footerText;

        public FooterViewModel()
        {
            FooterText = "Rentals Footer";
        }

        public string FooterText
        {
            get { return footerText; }
            set { SetProperty(ref footerText, value); }
        }
    }
}
