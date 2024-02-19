using test_api_rest.Models;

namespace test_api_rest.Services.Interfaces
{
    public interface ILoginService
    {
        Task<UserAccessData> LoginAuthentication(User user);
    }
}
