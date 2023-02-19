using Azure.Data.Tables;
using BusinessLogic;
using BusinessLogic.Entities;
using LibraryCop.BackendFunctions;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.DataAccess;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace LibraryCop.BackendFunctions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("host.json", optional: true)
            //    .AddEnvironmentVariables()
            //    .Build();

            ConfigureServices(builder.Services);            
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("ContentStorage.ConnectionString");

            var key = Environment.GetEnvironmentVariable("Authentication.Key");
            var issuer = Environment.GetEnvironmentVariable("Authentication.Issuer");
            var audience = Environment.GetEnvironmentVariable("Authentication.Audience");
            var salt = Environment.GetEnvironmentVariable("Authentication.Salt");

            services.AddMvcCore().AddNewtonsoftJson(options => {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            });

            // Authentication
            services.AddSingleton(new AuthenticationInfo(key, issuer, audience, salt));

            services.AddSingleton(new TableClient(connectionString, "Book"));
            services.AddSingleton<ConnectionInfo>(new ConnectionInfo(connectionString));

            services.AddSingleton<AuthenticationInteractor>();

            services.AddSingleton<IBookFinderDataAccess, BookTableDataAccess>();
            services.AddSingleton<IBookFinderDataAccess, LeitirApiDataAccess>();
            services.AddSingleton<IBookFinderDataAccess, BoksalaApiDataAccess>();
            services.AddSingleton<IBookManagementDataAccess, BookTableDataAccess>();
            services.AddSingleton<BookFinderInteractor>();

            services.AddSingleton<ILibraryCatalogoueDataAccess, LibraryCatalogueTableDataAccess>();
            services.AddSingleton<LibraryCatalogueInteractor>();
        }
    }
}
