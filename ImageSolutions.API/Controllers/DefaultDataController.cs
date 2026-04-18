using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ImageSolutions.API.Controllers
{
    [RoutePrefix("DefaultData")]
    public class DefaultDataController : ApiController
    {
        public string[] MyData = { "Data1","Data2","Data3" };

        [HttpGet]
        public string[] GetData()
        {
            return MyData;
        }

        [HttpGet]
        [Route("{id}")]
        public List<string> GetData(int id)
        {
            List<string> Return = new List<string>();
            foreach(string _string in MyData)
            {
                Return.Add(_string);
            }
            Return.Add(Convert.ToString(id));

            return Return;
        }

        [HttpPost]
        [Route("test")]
        public HttpResponseMessage TestPost([FromBody] Test test)
        {
            test.TestData = "Received";
            return Request.CreateResponse(HttpStatusCode.OK, test);
        }

        public class Test
        {
            public int ID { get; set; }
            public string TestData { get; set; }
        }
    }
}
