using test_api_rest.Models;
using test_api_rest.Services.Interfaces;
using test_api_rest.Utils.Interfaces;

namespace test_api_rest.Services
{
    public class RequestService(IRequest request) : IRequestService
    {
        private readonly IRequest _request = request;


        public async Task<string> GetResponseOfRequest(UserAccessData userAccessData, string procedureName, object body)
        {
            string response = await _request.GetResponseOfRequest(userAccessData, procedureName, body);
            return response;
        }

    }
}
