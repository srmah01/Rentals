using Rentals.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Rentals.WPFClient.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        public async Task<string> GetText()
        {
            var client = RentalsHttpClient.GetClient();
            string responseString = String.Empty;

            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            HttpResponseMessage response = await client.GetAsync("api/hello");

            if (response.IsSuccessStatusCode)
            {
                responseString = await response.Content.ReadAsStringAsync();
            }
            else
            {
                responseString = "An error occurred.";
            }

            return responseString;

        }

        public async Task<string> GetText(CancellationToken token = new CancellationToken())
        {
            return await GetText();
        }
    }
}
