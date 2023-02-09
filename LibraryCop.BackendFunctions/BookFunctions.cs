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
using System.Runtime.CompilerServices;
using LibraryCop.BusinessLogic;

namespace LibraryCop.BackendFunctions
{
    public class BookFunctions
    {
        private readonly BookFinderInteractor _interactor;

        public BookFunctions(BookFinderInteractor bookFinderInteractor)
        {
            _interactor = bookFinderInteractor;
        }

        [FunctionName("books")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "books/{isbn:long}")] HttpRequest req,
           long isbn, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(BookFunctions), nameof(Run));

            var book = await _interactor.GetBook(isbn.ToString());

            if (book == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(book);
        }

        [FunctionName("bookActions")]
        public async Task<IActionResult> RunActions([HttpTrigger(AuthorizationLevel.Function, "get", Route ="books/{isbn:long}/actions")] HttpRequest req, long isbn, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(BookFunctions), nameof(RunActions));

            return new NotFoundResult();
        }
    }
}
