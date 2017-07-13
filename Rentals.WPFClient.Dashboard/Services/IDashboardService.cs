using System.Threading;
using System.Threading.Tasks;

namespace Rentals.WPFClient.Dashboard.Services
{
    public interface IDashboardService
    {
        Task<string> GetText();
        Task<string> GetText(CancellationToken token);
    }
}