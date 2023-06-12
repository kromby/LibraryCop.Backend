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

        public async Task SaveBook(string title, string author, string publisher, int publishedYear)
        {
            _log.LogDebug("[{class}.{method}] Saving book '{title}'", nameof(BookManagementInteractor), nameof(SaveBook), title);

            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentNullException(nameof(author));
            if (string.IsNullOrWhiteSpace(publisher)) throw new ArgumentNullException(nameof(publisher));
            if (publishedYear < 1900) throw new ArgumentException("Invalid year.", nameof(publishedYear));
            if (publishedYear > DateTime.Now.Year) throw new ArgumentException("Can not have a year in the future", nameof(publishedYear));

            var guid = Guid.NewGuid();

            Book book = new(guid.ToString(), false)
            {
                IsComplete = true,
                Publisher = publisher,
                Detail = new()
                {
                    Author = author,
                    Title = title,
                    PublishYear = publishedYear,
                }
            };

            await _bookManagementDataAccess.SaveBook(book);
        }
    }
}
