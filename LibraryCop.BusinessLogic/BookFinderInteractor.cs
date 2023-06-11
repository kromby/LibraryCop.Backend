using BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;

namespace LibraryCop.BusinessLogic
{
    public class BookFinderInteractor
    {
        private const int DEFAULT_DATAACCESS_PRIORITY = 1;

        private readonly IList<IBookFinderDataAccess> _bookFinderDataAccesses;
        private readonly IBookManagementDataAccess _bookManagementDataAccess;
        private readonly ILogger<BookFinderInteractor> _log;
        private readonly LibraryCatalogueInteractor _libraryCatalogueInteractor;

        public BookFinderInteractor(IList<IBookFinderDataAccess> bookFinderDataAccesses, IBookManagementDataAccess bookManagementDataAccess, LibraryCatalogueInteractor libraryCatalogueInteractor, ILogger<BookFinderInteractor> log)
        {
            if (bookFinderDataAccesses == null || bookFinderDataAccesses.Count == 0)
                throw new ArgumentNullException(nameof(bookFinderDataAccesses), "Can not be null or empty.");
            _bookFinderDataAccesses = bookFinderDataAccesses;
            _bookManagementDataAccess = bookManagementDataAccess ?? throw new ArgumentNullException(nameof(bookManagementDataAccess), "Can not be null.");
            _libraryCatalogueInteractor = libraryCatalogueInteractor ?? throw new ArgumentNullException(nameof(libraryCatalogueInteractor), "Can not be null.");
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<Book?> GetBook(string isbn, Guid libraryID)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for book '{isbn}'.", nameof(BookFinderInteractor), nameof(GetBook), isbn);

            Book? book = null;

            foreach (var bookFinder in _bookFinderDataAccesses.OrderBy(t => t.Priority))
            {
                try
                {
                    if (book == null)
                        book = await bookFinder.GetBook(isbn);
                    else
                    {
                        book = await bookFinder.GetBook(isbn, book);

                        if (string.IsNullOrWhiteSpace(book.Detail.ImageUrl))
                            continue;
                        else
                            break;
                    }

                    if (book == null)
                        continue;

                    if (book.Saved)
                    {
                        var stateTask = GetState(isbn, libraryID);
                        var labelsTask = GetLabels(/*isbn*/);

                        book.State = await stateTask;
                        //labelsTask.Wait();
                        book.Labels = labelsTask; //.Result;
                    }
                    else
                    {
                        book.State.Created = DateTime.Now;
                        book.State.CreatedBy = libraryID;
                    }

                    book.Operations = GetOperations(isbn, book.State.State);

                    if (book.IsComplete || !string.IsNullOrWhiteSpace(book.Detail.ImageUrl))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "[{Class}.{Method}] Exception looking for book '{isbn}'.", nameof(BookFinderInteractor), nameof(GetBook), isbn);
                    continue;
                }
            }

            if (book == null)
            {
                _log.LogWarning("[{class}.{method}] Book {isbn} not found.", nameof(BookFinderInteractor), nameof(GetBook), isbn);
            }
            else if (!book.Saved)
            {
                await _bookManagementDataAccess.SaveBook(book);
            }

            return book;
        }

        public async Task<IList<Book>> GetBooksByTitle(string title, Guid libraryID)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for book by name '{isbn}'.", nameof(BookFinderInteractor), nameof(GetBooksByTitle), title);

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));

            try
            {
                var bookFinder = _bookFinderDataAccesses.Where(t => t.Priority == DEFAULT_DATAACCESS_PRIORITY).First();
                var list = await bookFinder.GetBooksByTitle(title);

                foreach (var book in list)
                {

                    try
                    {
                        var stateTask = GetState(book.ISBN, libraryID);
                        var labelsTask = GetLabels(/*isbn*/);

                        book.State = await stateTask;
                        //labelsTask.Wait();
                        book.Labels = labelsTask; //.Result;

                        book.Operations = GetOperations(book.ISBN, book.State.State);
                    }
                    catch (Exception ex)
                    {
                        if (book != null && !string.IsNullOrEmpty(book.ISBN))
                            _log.LogError(ex, "[{Class}.{Method}] Exception looking for book '{name}' with ISBN '{isbn}'.", nameof(BookFinderInteractor), nameof(GetBook), title, book.ISBN);
                        else
                            _log.LogError(ex, "[{Class}.{Method}] Exception looking for book '{name}' - book object is empty.", nameof(BookFinderInteractor), nameof(GetBook), title);
                        continue;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "[{Class}.{Method}] Exception looking for book '{name}'.", nameof(BookFinderInteractor), nameof(GetBook), title);
                return new List<Book>();
            }
        }

        public static IList<BookOperation> GetOperations(string isbn, State state)
        {
            var list = new List<BookOperation>();

            if (state.Equals(State.NotOwned))
            {
                list.Add(new BookOperation() { ID = 1, Name = "Skrá bók", Description = "Bæta við í bókasafn skólans", Method = HttpMethod.Post.ToString(), Path = $"/libraries/books/{isbn}" });
                //list.Add(new BookOperation() { ID = 4, Name = "Óskalisti", Description = "Setja á óskalistann", Path = $"/books/{isbn}/" });
            }
            else if (state.Equals(State.In))
            {
                list.Add(new BookOperation() { ID = 2, Name = "Taka út", Description = "Fá bók lánaða", Method = HttpMethod.Put.ToString(), Path = $"/libraries/books/{isbn}" });
            }
            else if (state.Equals(State.OnLoan))
            {
                list.Add(new BookOperation() { ID = 3, Name = "Skila", Description = "Skila bók", Method = HttpMethod.Put.ToString(), Path = $"/libraries/books/{isbn}" });
            }

            return list;
        }

        public async Task<BookState> GetState(string isbn, Guid libraryID)
        {
            return await _libraryCatalogueInteractor.GetBookState(isbn, libraryID);
        }

        public static IList<string> GetLabels(/*string isbn*/)
        {
            var list = new List<string>();
            return list;
        }
    }
}