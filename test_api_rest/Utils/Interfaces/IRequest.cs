using test_api_rest.Models;

namespace test_api_rest.Utils.Interfaces
{
    public interface IRequest
    {
        Task<string> GetPublicKey(User user);
        Task<Tokens> GetToken(User user, string aesKeyEncrypt);
        Task<string> GetResponseOfRequest(UserAccessData userAccessData, string procedureName, object body);

    }
}
