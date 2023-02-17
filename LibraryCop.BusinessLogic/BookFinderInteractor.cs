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

        public BookFinderInteractor(IList<IBookFinderDataAccess> bookFinderDataAccesses, IBookManagementDataAccess bookManagementDataAccess, ILogger<BookFinderInteractor> log)
        {
            if (bookFinderDataAccesses == null || bookFinderDataAccesses.Count == 0)
                throw new ArgumentNullException(nameof(bookFinderDataAccesses), "Can not be null or empty.");
            _bookFinderDataAccesses = bookFinderDataAccesses;
            _bookManagementDataAccess = bookManagementDataAccess;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<Book?> GetBook(string isbn)
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
                        var stateTask = GetState(isbn);
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

                    book.Operations = await GetOperations(isbn, book.State.State);

                    break;
                }
            }

            return book;
        }

        public async Task<IList<BookOperation>> GetOperations(string isbn, State state)
        {
            var list = new List<BookOperation>();

            if (state.Equals(State.NotOwned))
            {
                list.Add(new BookOperation() { ID = 1, Name = "Skrá bók", Description = "Bæta við í bókasafn skólans", Path = $"/books/{isbn}/" });
                list.Add(new BookOperation() { ID = 2, Name = "Óskalisti", Description = "Setja á óskalistann", Path = $"/books/{isbn}/" });
            }
            else if(state.Equals(State.In))
            {
                list.Add(new BookOperation() { ID = 1, Name = "Taka út", Description = "Fá bók lánaða", Path = $"/books/{isbn}/" });                
            }

            return list;
        }

        public async Task<BookState> GetState(string isbn)
        {
            return new BookState(isbn, State.In, Guid.Empty);
        }

        public async Task<IList<string>> GetLabels(string isbn)
        {
            var list = new List<string>();
            return list;
        }
    }
}