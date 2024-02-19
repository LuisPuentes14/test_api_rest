using Microsoft.AspNetCore.Mvc;
using test_api_rest.Services.Interfaces;
using Newtonsoft.Json;
using test_api_rest.Models;

namespace test_api_rest.Controllers
{
    [Route("/[controller]")]
    public class RequestController(IRequestService requestService) : Controller
    {

        private readonly IRequestService _requestService = requestService;
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Route("{procedureName}")]
        public async Task<IActionResult> GetResponseOfRequest([FromBody] object body, string procedureName)
        {

            var UserAccesData = HttpContext.Session.GetString("UserAccesData");

            if (UserAccesData == null)
            {
                return NotFound("Usuario no encontrado");
            }

            UserAccessData dataUser = JsonConvert.DeserializeObject<UserAccessData>(HttpContext.Session.GetString("UserAccesData"));          

            string response = await _requestService.GetResponseOfRequest(dataUser, procedureName, body);

            return Ok(response);

        }

        [HttpPost]      
        public async Task<IActionResult> CloseSessionUser([FromBody] object body, string procedureName)
        {

            var UserAccesData = HttpContext.Session.GetString("UserAccesData");

            if (UserAccesData == null)
            {
                return NotFound("Usuario no encontrado");
            }

            UserAccessData dataUser = JsonConvert.DeserializeObject<UserAccessData>(HttpContext.Session.GetString("UserAccesData"));

            string response = await _requestService.GetResponseOfRequest(dataUser, procedureName, body);

            return Ok(response);

        }


    }
}
