using Prism.Mvvm;
using Rentals.WPFClient.Async;
using Rentals.WPFClient.Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rentals.WPFClient.Dashboard.ViewModels
{
    //public sealed class CountUrlBytesViewModel
    //{
    //    public CountUrlBytesViewModel(DashboardViewModel parent, string url, IAsyncCommand command)
    //    {
    //        LoadingMessage = "Loading (" + url + ")...";
    //        Command = command;
    //        RemoveCommand = new AsyncDelegateCommand(() => parent.Operations.Remove(this));
    //    }

    //    public string LoadingMessage { get; private set; }

    //    public IAsyncCommand Command { get; private set; }

    //    public ICommand RemoveCommand { get; private set; }
    //}

    public class DashboardViewModel : BindableBase
    {
        public DashboardViewModel()
        {
            Url = "http://www.example.com/";
            CountUrlBytesCommand = AsyncCommand.Create(token => MyService.DownloadAndCountBytesAsync(Url, token));

            //Operations = new ObservableCollection<CountUrlBytesViewModel>();
            //CountUrlBytesCommand = new AsyncDelegateCommand(() =>
            //{
            //    var countBytes = AsyncCommand.Create(token => MyService.DownloadAndCountBytesAsync(Url, token));
            //    countBytes.Execute(null);
            //    Operations.Add(new CountUrlBytesViewModel(this, Url, countBytes));
            //});
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        //public ObservableCollection<CountUrlBytesViewModel> Operations { get; private set; }

        //public ICommand CountUrlBytesCommand { get; private set; }
        public IAsyncCommand CountUrlBytesCommand { get; private set; }
        
    }

}
