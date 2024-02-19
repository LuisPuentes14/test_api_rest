using Microsoft.AspNetCore.Authentication;
using test_api_rest.Services;
using test_api_rest.Services.Interfaces;
using test_api_rest.Utils;
using test_api_rest.Utils.Interfaces;

namespace test_api_rest.IOC
{
    public static  class Dependencies    {

        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static void InyectarDependencia(this IServiceCollection services, IConfiguration Configuration)
        {
            // Segregación de interfaces           
            //services.AddSingleton<ValidateAppParameters>();
            //services.AddSingleton<IGenerateIncidenceExpirationNotificationsRepository, GenerateIncidenceExpirationNotificationsRepository>();
            //services.AddSingleton<IGetNotificationsPushRepository, GetNotificationsPushRepository>();
            //services.AddSingleton<IDeleteNotificationPushRepository, DeleteNotificationPushRepository>();
            //services.AddSingleton<IJWT, JWT>();
            //services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IRequest, Request>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRequestService, RequestService>(); 


        }
    }
}
