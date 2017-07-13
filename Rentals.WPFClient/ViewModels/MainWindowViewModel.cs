using Prism.Mvvm;

namespace Rentals.WPFClient.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Rentals";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
