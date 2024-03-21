using Microsoft.AspNetCore.Http.HttpResults;
using test_api_rest.Models;
using test_api_rest.Services.Interfaces;
using test_api_rest.Utils.Interfaces;

namespace test_api_rest.Services
{
    public class LoginService(IRequest request): ILoginService
    {

        private readonly IRequest _request = request;

        public async Task<UserAccessData> LoginAuthentication(User user)
        {
            try
            {
                UserAccessData userAccessData = new();

                // obtiene la llave publica
                string publicKey = await _request.GetPublicKey(user);

                if (publicKey == string.Empty)
                    throw new Exception("error obtendiendo llaves publica");

                // genera llave AES con vector
                string aesKeyBase64 = Utils.Utils.GetAesKeyAndIvBytesBase64();

                // encripta la llave AES con la llave publica obtenida al principio
                string aesKeyEnrypt = Utils.Utils.GetSessionKeyBase64EncryptedWithPublicKey(aesKeyBase64, publicKey);

                // encripta la contrasena del usuario con la llave AES generada anterior mente
                user.password = Utils.Utils.EncryptAes256WithIvBase64(user.password, aesKeyBase64);

                Tokens tokens = await _request.GetToken(user, aesKeyEnrypt);

                if (tokens is null)
                    throw new Exception("error obtendiendo token de autenticacion y refresco");

                // se desencriptan los token obtenidos con la llave AES generada anterior mente
                tokens.accessToken = Utils.Utils.DecryptAes256WithIvBase64(tokens.accessToken, aesKeyBase64);
                tokens.refreshToken = Utils.Utils.DecryptAes256WithIvBase64(tokens.refreshToken, aesKeyBase64);

                userAccessData.Tokens = tokens;
                userAccessData.AesKey = aesKeyBase64;
                userAccessData.login = user.login;


                return userAccessData;

            }
            catch (Exception)
            {
                return null;
            }
         

        }


    }
}
