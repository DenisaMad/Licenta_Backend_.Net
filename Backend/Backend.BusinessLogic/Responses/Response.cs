using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.Responses
{
    public class Response
    {
        public string Message {  get; set; }

        public HttpStatusCode StatusCode { get; set; }

    }
}
