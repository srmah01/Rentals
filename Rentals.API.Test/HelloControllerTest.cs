using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentalsAPI.Controllers;
using System.Web.Http.Results;

namespace Rentals.API.Test
{
    [TestClass]
    public class HelloControllerTest
    {
        [TestMethod]
        public void GetShouldReturnResponse()
        {
            var controller = new HelloController();

            var response = controller.Get() as OkNegotiatedContentResult<HelloResponse>; ;

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Content.Data.Contains("Hello there!!"));
        }
    }
}
