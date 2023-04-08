using BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;

namespace LibraryCop.BusinessLogic
{
    public class BookFinderInteractor
    {
        private readonly IList<IBookFinderDataAccess> _bookFinderDataAccesses;
        private readonly IBookManagementDataAccess _bookManagementDataAccess;
        private readonly ILogger<BookFinderInteractor> _log;
        private readonly LibraryCatalogueInteractor _libraryCatalogueInteractor;

        public BookFinderInteractor(IList<IBookFinderDataAccess> bookFinderDataAccesses, IBookManagementDataAccess bookManagementDataAccess, LibraryCatalogueInteractor libraryCatalogueInteractor, ILogger<BookFinderInteractor> log)
        {
            if (bookFinderDataAccesses == null || bookFinderDataAccesses.Count == 0)
                throw new ArgumentNullException(nameof(bookFinderDataAccesses), "Can not be null or empty.");
            _bookFinderDataAccesses = bookFinderDataAccesses;
            _bookManagementDataAccess = bookManagementDataAccess;
            _libraryCatalogueInteractor = libraryCatalogueInteractor;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<Book?> GetBook(string isbn, Guid libraryID)
        {
            _log.LogInformation("[{Method}] Looking for book '{isbn}'.", nameof(GetBook), isbn);

            Book? book = null;

            foreach (var bookFinder in _bookFinderDataAccesses.OrderBy(t => t.Priority))
            {
                book = await bookFinder.GetBook(isbn);

                if (book != null)
                {
                    if (book.Saved)
                    {
                        var stateTask = GetState(isbn, libraryID);
                        var labelsTask = GetLabels(isbn);

                        stateTask.Wait();
                        book.State = stateTask.Result;
                        labelsTask.Wait();
                        book.Labels = labelsTask.Result;
                    }
                    else
                    {
                        await _bookManagementDataAccess.SaveBook(book);
                    }

                    book.Operations = GetOperations(isbn, book.State.State);

                    break;
                }
            }

            return book;
        }

        public static IList<BookOperation> GetOperations(string isbn, State state)
        {
            var list = new List<BookOperation>();

            if (state.Equals(State.NotOwned))
            {
                list.Add(new BookOperation() { ID = 1, Name = "Skrá bók", Description = "Bæta við í bókasafn skólans", Method = HttpMethod.Post.ToString(), Path = $"/libraries/books/{isbn}" });
                //list.Add(new BookOperation() { ID = 4, Name = "Óskalisti", Description = "Setja á óskalistann", Path = $"/books/{isbn}/" });
            }            
            else if(state.Equals(State.In))
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
            var state = await _libraryCatalogueInteractor.GetBookState(isbn, libraryID);

            if(state != null) { return state; }
            return new BookState(isbn, State.In, Guid.Empty);
        }

        public static async Task<IList<string>> GetLabels(string isbn)
        {
            var list = new List<string>();
            return list;
        }
    }
}