using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace RentalsAPI.Controllers
{
    [RoutePrefix("api")]
    public class HelloController : ApiController
    {
        // GET api/<controller>
        [Route("hello")]
        public IHttpActionResult Get()
        {
            var helloResponse = new HelloResponse() { Data = "Hello there!!" };
            return Ok(helloResponse);
        }
    }

    public class HelloResponse
    {
        public string Data { get; set; }
    }
}