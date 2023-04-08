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
using BusinessLogic.Entities;
using System.Net.Http;

namespace BackendFunctions
{
    public class LibraryCatalogueFunctions
    {
        private readonly LibraryCatalogueInteractor _interactor;
        private readonly AuthenticationInteractor _authenticationInteractor;

        public LibraryCatalogueFunctions(AuthenticationInteractor authenticationInteractor, LibraryCatalogueInteractor libraryCatalogueInteractor)
        {
            _interactor = libraryCatalogueInteractor;
            _authenticationInteractor = authenticationInteractor;
        }

        [FunctionName("libraryCatalogue")]
        public async Task<IActionResult> RunLibraryBooks(
            [HttpTrigger(AuthorizationLevel.Function, "post", "put", Route = "libraries/books/{isbn:long}")] HttpRequest req,
            long isbn, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(LibraryCatalogueFunctions), nameof(RunLibraryBooks));

            if(!AuthenticationUtil.GetAuthenticatedUser(_authenticationInteractor, req.Headers, out AuthenticatedUser user, log))
            {
                return new UnauthorizedResult();
            }

            if (req.Method == HttpMethod.Post.ToString())
            {
                var resultState = await _interactor.AddBook(isbn.ToString(), user.LibraryID, user.UserID);
                return new CreatedResult($"/libraries/{resultState.LibraryID}/books/{resultState.ISBN}", resultState);
            }
            else if (req.Method == HttpMethod.Put.ToString())
            {
                var resultState = await _interactor.BorrowBook(isbn.ToString(), user.LibraryID, user.UserID);
                return new OkObjectResult(resultState);
            }
            else
            {
                return new NotFoundResult();
            }    
        }
    }
}
