using Azure.Data.Tables;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
    public class BookFinderTableDataAccess : IBookFinderDataAccess
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<BookFinderTableDataAccess> _log;

        public BookFinderTableDataAccess(TableClient tableClient, ILogger<BookFinderTableDataAccess> log)
        {
            Priority = 1;
            _tableClient = tableClient;
            _log = log;
        }

        public int Priority { get; private set; }

        public async Task<Book?> GetBook(string isbn)
        {
            _log.LogInformation("[{Method}] Looking for book '{isbn}'.", nameof(GetBook), isbn);

            var result = _tableClient.Query<BookEntity>(x => x.RowKey == isbn);
            if (result.Count() == 1)
            {
                _log.LogInformation("[{Method}] Book '{isbn}' found.", nameof(GetBook), isbn);

                var book = new Book()
                {
                    Author = result.First().Author,
                    Description = result.First().Description,
                    ImageUrl = result.First().ImageUrl,
                    ISBN = result.First().RowKey,
                    Link = result.First().Link,
                    Publisher = result.First().PartitionKey,
                    Title = result.First().Title
                };

                return book;
            }

            _log.LogInformation("[{Method}] Book '{isbn}' not found.", nameof(GetBook), isbn);
            return null;
        }
    }
}
