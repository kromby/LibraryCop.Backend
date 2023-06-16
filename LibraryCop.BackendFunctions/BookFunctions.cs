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
using BusinessLogic;
using BackendFunctions.Model;

namespace LibraryCop.BackendFunctions
{
    public class BookFunctions
    {
        private readonly BookFinderInteractor _finderInteractor;
        private readonly BookManagementInteractor _managementInteractor;
        private readonly AuthenticationInteractor _authenticationInteractor;

        public BookFunctions(BookFinderInteractor bookFinderInteractor, BookManagementInteractor managementInteractor, AuthenticationInteractor authenticationInteractor)
        {
            _finderInteractor = bookFinderInteractor;
            _managementInteractor = managementInteractor;
            _authenticationInteractor = authenticationInteractor;
        }

        [FunctionName("books")]
        public async Task<IActionResult> RunBooks(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "books")] HttpRequest req, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(BookFunctions), nameof(RunBooks));

            if (!AuthenticationUtil.GetAuthenticatedUser(_authenticationInteractor, req.Headers, out User user, log))
            {
                return new UnauthorizedResult();
            }

            try
            {
                if (req.Method == HttpMethods.Get)
                {
                    var title = req.Query["title"];
                    return await GetBooks(title, user);
                }
                else if (req.Method == HttpMethods.Post)
                {
                    var book = await req.ReadFromJsonAsync<BookCreate>();
                    return await PostBook(book, user);
                }
                else
                {
                    return new NotFoundObjectResult($"Method '{req.Method}' not supported.");
                }
            }
            catch (ArgumentException aex)
            {
                return new BadRequestObjectResult(aex.Message);
            }
            catch(Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw;
            }
        }

        private async Task<IActionResult> GetBooks(string title, User user)
        {
            var book = await _finderInteractor.GetBooksByTitle(title, user.LibraryID);
            return new OkObjectResult(book);

        }

        private async Task<IActionResult> PostBook(BookCreate book, User user)
        {
            await _managementInteractor.SaveBook(book.Title, book.Author, book.Publisher, book.PublishYear, user);
            return new CreatedResult("/api/books/", -1);
        }

        [FunctionName("booksIsbn")]
        public async Task<IActionResult> RunBooksIsbn(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "books/{isbn:long}")] HttpRequest req,
           long isbn, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(BookFunctions), nameof(RunBooksIsbn));

            if (!AuthenticationUtil.GetAuthenticatedUser(_authenticationInteractor, req.Headers, out User user, log))
            {
                return new UnauthorizedResult();
            }

            try
            {
                var book = await _finderInteractor.GetBook(isbn.ToString(), user);

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
