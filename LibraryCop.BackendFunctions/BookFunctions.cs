using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryCop.BackendFunctions.Logic;

namespace LibraryCop.BackendFunctions
{
    public static class BookFunctions
    {
        [FunctionName("books")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "books/{isbn:long}")] HttpRequest req,
           long isbn, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            SearchBookInteractor interactor= new SearchBookInteractor();
            var book = await interactor.GetBook(isbn.ToString());

            if(book == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(book);
        }
    }
}
