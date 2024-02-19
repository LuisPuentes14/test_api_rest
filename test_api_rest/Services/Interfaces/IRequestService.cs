using test_api_rest.Models;

namespace test_api_rest.Services.Interfaces
{
    public interface IRequestService
    {
        Task<string> GetResponseOfRequest(UserAccessData userAccessData, string procedureName, object body);
    }
}
