using BusinessLogic.Utils;
using LibraryCop.BusinessLogic.DataAccess;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic
{
    public class LibraryCatalogueInteractor
    {
        private readonly ILibraryCatalogueDataAccess _libraryCatalogueDataAccess;
        private readonly ILogger<LibraryCatalogueInteractor> _log;

        public LibraryCatalogueInteractor(ILibraryCatalogueDataAccess libraryCatalogueDataAccess, ILogger<LibraryCatalogueInteractor> log)
        {
            _libraryCatalogueDataAccess = libraryCatalogueDataAccess ?? throw new ArgumentNullException(nameof(libraryCatalogueDataAccess));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<BookState> AddBook(string isbn, Guid libraryID, Guid userID)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}', libraryID '{libraryID}'.", nameof(LibraryCatalogueInteractor), nameof(AddBook), isbn, libraryID);
            VerifyInput(isbn, libraryID, userID);

            BookState bookState = new(isbn, State.In, libraryID)
            {
                Created = DateTime.Now,
                CreatedBy = userID
            };

            await _libraryCatalogueDataAccess.SaveBookState(bookState);

            return bookState;
        }

        private static void VerifyInput(string isbn, Guid libraryID, Guid userID)
        {
            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentNullException(nameof(isbn));
            if (libraryID == Guid.Empty) throw new ArgumentNullException(nameof(libraryID));
            if (userID == Guid.Empty) throw new ArgumentNullException(nameof(userID));
            if (!ISBNHelper.IsValid(isbn)) throw new ArgumentException("ISBN is invalid.", nameof(isbn));
        }

        public async Task<BookState> BorrowBook(string isbn, Guid libraryID, Guid userID)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}', libraryID '{libraryID}'.", nameof(LibraryCatalogueInteractor), nameof(BorrowBook), isbn, libraryID);
            VerifyInput(isbn, libraryID, userID);

            BookState? bookState = await _libraryCatalogueDataAccess.GetState(isbn, libraryID) ?? new(isbn, State.In, libraryID)
            {
                Created = DateTime.Now,
                CreatedBy = userID
            };

            bookState.State = (bookState.State == State.In) ? State.OnLoan : State.In;
            bookState.Changed = DateTime.Now;
            bookState.ChangedBy = userID;

            await _libraryCatalogueDataAccess.SaveBookState(bookState);

            return bookState;
        }

        public async Task<BookState> GetBookState(string isbn, Guid libraryID) {
            var state = await _libraryCatalogueDataAccess.GetState(isbn, libraryID);

            state ??= new BookState(isbn, State.NotOwned, libraryID) { Created = DateTime.Now, CreatedBy = libraryID};

            return state;
        }
    }
}
