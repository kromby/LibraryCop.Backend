using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryCop.BusinessLogic.Entities;
using LibraryCop.BusinessLogic;

namespace BackendFunctions
{
    public class LibraryCatalogueFunctions
    {
        private readonly LibraryCatalogueInteractor _interactor;

        public LibraryCatalogueFunctions(LibraryCatalogueInteractor libraryCatalogueInteractor)
        {
            _interactor = libraryCatalogueInteractor;
        }

        [FunctionName("libraryCatalogue")]
        public async Task<IActionResult> RunLibraryBooks(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "libraries/books")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(LibraryCatalogueFunctions), nameof(RunLibraryBooks));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            BookState state = JsonConvert.DeserializeObject<BookState>(requestBody);

            var resultState = await _interactor.AddBook(state.ISBN, state.LibraryID, state.CreatedBy);

            return new CreatedResult($"/library/{resultState.LibraryID}/books/{resultState.ISBN}", resultState);
        }
    }
}
