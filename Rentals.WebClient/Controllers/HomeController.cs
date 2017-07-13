using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Rentals.Helpers;
using System.Net.Http;

namespace Rentals.WebClient.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var client = RentalsHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/hello");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                ViewBag.HelloResponse = responseString;
            }
            else
            {
                Content("An error occurred.");
            }

            return View();
        }

    }
}