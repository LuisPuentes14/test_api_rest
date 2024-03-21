using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using test_api_rest.Models;
using test_api_rest.Utils.Interfaces;

namespace test_api_rest.Utils
{
    public class Request(IConfiguration configuration) : IRequest
    {

        private readonly IConfiguration _configuration = configuration;

        public async Task<string> GetPublicKey(User user)
        {

            ResponseRequestServer response = new ResponseRequestServer();

            Console.WriteLine("Obteniendo llave publica");

            var url = _configuration["ServerConnection"] + "/api/v1/generate_keys";

            // El objeto que quieres enviar en la petición POST, convertido a JSON
            var miObjeto = new { username = user.login };
            var json = JsonConvert.SerializeObject(miObjeto);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            string contenidoRespuesta;

            using (var cliente = new HttpClient())
            {
                // Envía la petición POST
                var respuesta = await cliente.PostAsync(url, contenido);

                response.statusCode = respuesta.StatusCode;

                // Asegúrate de que la petición fue exitosa
                if (respuesta.IsSuccessStatusCode)
                {
                    // Lee y procesa la respuesta
                    contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();

                    PublicKeyRsa publicKeyRsa = JsonConvert.DeserializeObject<PublicKeyRsa>(contenidoRespuesta);
                    contenidoRespuesta = publicKeyRsa.publicKey;

                    response.body = publicKeyRsa;
                }
                else
                {
                    contenidoRespuesta = string.Empty;
                }
            }
            return contenidoRespuesta;
        }


        public async Task<Tokens> GetToken(User user, string aesKeyEncrypt)
        {
            Console.WriteLine("obteniedo token");

            Tokens tokens = null;

            var url = _configuration["ServerConnection"] + "/api/v1/jsonwebtoken_authenticate";

            // El objeto que quieres enviar en la petición POST, convertido a JSON
            var miObjeto = new { username = user.login, password = user.password, sessionkey = aesKeyEncrypt };
            var json = JsonConvert.SerializeObject(miObjeto);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            using (var cliente = new HttpClient())
            {
                // Envía la petición POST
                var respuesta = await cliente.PostAsync(url, contenido);

                Console.WriteLine(respuesta);

                // valida que la respuesta fue exitosa
                if (respuesta.IsSuccessStatusCode)
                    // Lee y procesa la respuesta 
                    tokens = JsonConvert.DeserializeObject<Tokens>(await respuesta.Content.ReadAsStringAsync());
            }

            return tokens;

        }

        public async Task<string> GetResponseOfRequest(UserAccessData userAccessData, string procedureName, object body)
        {
            Console.WriteLine("Iniciando peticion.");

            string url =$"{_configuration["ServerConnection"]}/api/v1/{procedureName}";
            // string url = "http://localhost:8081/api/v1/procedureName";

            var contenidoRespuesta = string.Empty;

            // Crear una instancia de HttpClient
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var ivx = Utils.GetIvBytesBase64();
                    var newAesKeyBase64 = Utils.GetAesKeyAndIvBytesBase64(userAccessData.AesKey, ivx);

                    var jj = userAccessData.Tokens.refreshToken;


                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-User", Convert.ToBase64String( Encoding.UTF8.GetBytes(userAccessData.login))) ;
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Iv", ivx);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Utils.EncryptAes256WithIvBase64(userAccessData.Tokens.accessToken, newAesKeyBase64)}");

                    var json = body.ToString();
                    var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                    Console.WriteLine(httpClient.DefaultRequestHeaders);

                    var respuesta = await httpClient.PostAsync(url, contenido);

                    Console.WriteLine(respuesta);

                    //if (respuesta.IsSuccessStatusCode)
                    //{
                    // Lee y procesa la respuesta
                    contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                    Console.WriteLine(contenidoRespuesta);

                    // }

                }
                 catch (Exception ex)
                {

                    throw;
                }
                
            }

            return contenidoRespuesta;
        }


        public async Task<string> CloseSessionUser(UserAccessData userAccessData)
        {
            Console.WriteLine("cerrando session");

            Tokens tokens = new();

            var url = _configuration["ServerConnection"] + "/api/v1/request_user_logout";

            // El objeto que quieres enviar en la petición POST, convertido a JSON
            var miObjeto = new { in_user_login = userAccessData.login };
            var json = JsonConvert.SerializeObject(miObjeto);

            var contenidoRespuesta = string.Empty;


            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            using (var cliente = new HttpClient())
            {
                var ivx = Utils.GetIvBytesBase64();
                var newAesKeyBase64 = Utils.GetAesKeyAndIvBytesBase64(userAccessData.AesKey, ivx);


                cliente.DefaultRequestHeaders.TryAddWithoutValidation("X-User", userAccessData.login);
                cliente.DefaultRequestHeaders.TryAddWithoutValidation("X-Iv", ivx);
                cliente.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Utils.EncryptAes256WithIvBase64(userAccessData.Tokens.accessToken, newAesKeyBase64)}");


                // Envía la petición POST
               var   respuesta = await cliente.PostAsync(url, contenido);

                Console.WriteLine(respuesta);

                contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();

                return contenidoRespuesta;

            }  




        }
    }
}
