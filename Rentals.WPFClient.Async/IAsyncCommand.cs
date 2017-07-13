using System.Threading.Tasks;
using System.Windows.Input;

namespace Rentals.WPFClient.Async
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}