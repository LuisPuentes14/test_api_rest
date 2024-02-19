using System.Net;

namespace test_api_rest.Models
{
    public class ResponseRequestServer
    {
        public HttpStatusCode statusCode { get; set; }
        public object body { get; set; }

    }
}
