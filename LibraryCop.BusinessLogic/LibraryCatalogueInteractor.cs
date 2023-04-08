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
        private readonly ILibraryCatalogoueDataAccess _libraryCatalogoueDataAccess;
        private readonly ILogger<LibraryCatalogueInteractor> _log;

        public LibraryCatalogueInteractor(ILibraryCatalogoueDataAccess libraryCatalogoueDataAccess, ILogger<LibraryCatalogueInteractor> log)
        {
            this._libraryCatalogoueDataAccess = libraryCatalogoueDataAccess ?? throw new ArgumentNullException(nameof(libraryCatalogoueDataAccess));
            this._log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<BookState> AddBook(string isbn, Guid libraryID, Guid userID)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}', libraryID '{libraryID}'.", nameof(LibraryCatalogueInteractor), nameof(AddBook), isbn, libraryID);

            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentNullException(nameof(isbn));
            if (libraryID == Guid.Empty) throw new ArgumentNullException(nameof(libraryID));
            if (userID == Guid.Empty) throw new ArgumentNullException(nameof(userID));
            if (!VerifyISBN(isbn)) throw new ArgumentException("ISBN is invalid.", nameof(isbn));

            BookState bookState = new(isbn, State.In, libraryID)
            {
                Created = DateTime.Now,
                CreatedBy = userID
            };

            await _libraryCatalogoueDataAccess.SaveBookState(bookState);

            return bookState;
        }

        public async Task<BookState> BorrowBook(string isbn, Guid libraryID, Guid userID)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}', libraryID '{libraryID}'.", nameof(LibraryCatalogueInteractor), nameof(BorrowBook), isbn, libraryID);

            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentNullException(nameof(isbn));
            if (libraryID == Guid.Empty) throw new ArgumentNullException(nameof(libraryID));
            if (userID == Guid.Empty) throw new ArgumentNullException(nameof(userID));
            if (!VerifyISBN(isbn)) throw new ArgumentException("ISBN is invalid.", nameof(isbn));

            BookState? bookState = await _libraryCatalogoueDataAccess.GetState(isbn, libraryID);

            bookState ??= new(isbn, State.In, libraryID)
                {
                    Created = DateTime.Now,
                    CreatedBy = userID
                };

            bookState.State = (bookState.State == State.In) ? State.OnLoan : State.In;
            bookState.Changed = DateTime.Now;
            bookState.ChangedBy = userID;

            await _libraryCatalogoueDataAccess.SaveBookState(bookState);

            return bookState;
        }

        internal static bool VerifyISBN(string isbn)
        {
            if (isbn.Length == 10)
                return VerifyISBN10(isbn);
            else if (isbn.Length == 13)
                return VerifyISBN13(isbn);
            else return false;
        }

        internal static bool VerifyISBN10(string isbn)
        {
            // Remove any non-numeric characters from the ISBN
            isbn = new string(isbn.Where(char.IsDigit).ToArray());

            // Calculate the check digit
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i + 1) * digit;
            }
            int checkDigit = sum % 11;

            // Compare the calculated check digit to the actual check digit
            if (checkDigit == 10)
            {
                return (isbn[9] == 'X');
            }
            else
            {
                return (isbn[9] - '0') == checkDigit;
            }
        }

        internal static bool VerifyISBN13(string isbn)
        {
            // Remove any non-numeric characters from the ISBN
            isbn = new string(isbn.Where(char.IsDigit).ToArray());

            // Calculate the check digit
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i % 2 == 0) ? digit : 3 * digit;
            }
            int checkDigit = (10 - (sum % 10)) % 10;

            // Compare the calculated check digit to the actual check digit
            return (isbn[12] - '0') == checkDigit;
        }

        public async Task<BookState> GetBookState(string isbn, Guid libraryID) {
            var state = await _libraryCatalogoueDataAccess.GetState(isbn, libraryID);

            state ??= new BookState(isbn, State.NotOwned, libraryID);

            return state;
        }
    }
}
