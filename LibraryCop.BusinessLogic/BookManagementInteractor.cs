using BusinessLogic.Utils;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class BookManagementInteractor
    {
        private readonly IBookManagementDataAccess _bookManagementDataAccess;
        private readonly ILogger<BookManagementInteractor> _log;

        public BookManagementInteractor(IBookManagementDataAccess bookManagementDataAccess, ILogger<BookManagementInteractor> log)
        {
            _bookManagementDataAccess = bookManagementDataAccess;
            _log = log;
        }

        public async Task<string> SaveBook(string isbn, string title, string author, string publisher, int publishYear, User user)
        {
            _log.LogDebug("[{class}.{method}] Saving book '{title}'", nameof(BookManagementInteractor), nameof(SaveBook), title);

            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentNullException(nameof(author));
            if (string.IsNullOrWhiteSpace(publisher)) throw new ArgumentNullException(nameof(publisher));
            if (publishYear < 1900) throw new ArgumentException("Invalid year.", nameof(publishYear));
            if (publishYear > DateTime.Now.Year) throw new ArgumentException("Can not have a year in the future", nameof(publishYear));

            if (string.IsNullOrWhiteSpace(isbn))
                isbn = ISBNHelper.GenerateISBN();
            else
            {
                if(!ISBNHelper.IsValid(isbn))
                    throw new ArgumentException("ISBN is invalid.", nameof(isbn));
            }
                    

            Book book = new(isbn, false, user.UserID)
            {
                IsComplete = true,
                Publisher = publisher,                
                Detail = new()
                {
                    Author = author,
                    Title = title,
                    PublishYear = publishYear,
                }
            };

            return await _bookManagementDataAccess.SaveBook(book);
        }
    }
}
