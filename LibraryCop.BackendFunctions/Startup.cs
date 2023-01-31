using Azure.Data.Tables;
using LibraryCop.BackendFunctions;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.DataAccess;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddSingleton(new TableClient(connectionString, "LibraryCop"));

            services.AddSingleton<IBookFinderDataAccess, BookFinderTableDataAccess>();
            services.AddSingleton<BookFinderInteractor>();
        }
    }
}
