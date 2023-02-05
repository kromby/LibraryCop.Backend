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
                    if(!book.Saved)
                    {
                        await _bookManagementDataAccess.SaveBook(book);
                    }

                    break;
                }
            }

            return book;
        }
    }
}