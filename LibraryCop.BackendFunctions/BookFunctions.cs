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
using BackendFunctions;
using LibraryCop.BusinessLogic.Entities;

namespace LibraryCop.BackendFunctions
{
    public class BookFunctions
    {
        private readonly BookFinderInteractor _interactor;
        private readonly AuthenticationInteractor _authenticationInteractor;

        public BookFunctions(BookFinderInteractor bookFinderInteractor, AuthenticationInteractor authenticationInteractor)
        {
            _interactor = bookFinderInteractor;
            _authenticationInteractor = authenticationInteractor;
        }

        [FunctionName("books")]
        public async Task<IActionResult> RunBooks(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "books")] HttpRequest req, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(BookFunctions), nameof(RunBooks));

            if (!AuthenticationUtil.GetAuthenticatedUser(_authenticationInteractor, req.Headers, out AuthenticatedUser user, log))
            {
                return new UnauthorizedResult();
            }

            try
            {
                var title = req.Query["title"];
                var book = await _interactor.GetBooksByTitle(title, user.LibraryID);
                return new OkObjectResult(book);
            }
            catch (ArgumentException aex)
            {
                return new BadRequestObjectResult(aex.Message);
            }
        }

        [FunctionName("booksIsbn")]
        public async Task<IActionResult> RunBooksIsbn(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "books/{isbn:long}")] HttpRequest req,
           long isbn, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(BookFunctions), nameof(RunBooksIsbn));

            if (!AuthenticationUtil.GetAuthenticatedUser(_authenticationInteractor, req.Headers, out AuthenticatedUser user, log))
            {
                return new UnauthorizedResult();
            }

            try
            {
                var book = await _interactor.GetBook(isbn.ToString(), user.LibraryID);

                if (book == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(book);
            }
            catch (ArgumentException aex)
            {
                return new BadRequestObjectResult(aex.Message);
            }
        }
    }
}
