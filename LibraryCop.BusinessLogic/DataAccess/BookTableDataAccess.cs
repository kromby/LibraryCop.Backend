using Azure.Data.Tables;
using BusinessLogic;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Model;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
    public class BookTableDataAccess : IBookManagementDataAccess, IBookFinderDataAccess
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<BookTableDataAccess> _log;

        public BookTableDataAccess(ConnectionInfo connectionInfo, ILogger<BookTableDataAccess> log)
        {
            Priority = 1;
            _tableClient = new TableClient(connectionInfo.ConnectionString, "Book");
            _log = log;
        }

        public int Priority { get; private set; }        

        public async Task<Book?> GetBook(string isbn)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for book '{isbn}'.", nameof(BookTableDataAccess), nameof(GetBook), isbn);

            var asyncResults = _tableClient.QueryAsync<BookModel>(x => x.RowKey == isbn);
            await foreach (var bookEntity in asyncResults)
            {
                _log.LogInformation("[{Class}.{Method}] Book '{isbn}' found.", nameof(BookTableDataAccess), nameof(GetBook), isbn);
                var book = ParseBookFromModel(bookEntity);
                return book;
            }

            _log.LogInformation("[{Class}.{Method}] Book '{isbn}' not found.", nameof(BookTableDataAccess), nameof(GetBook), isbn);
            return null;
        }

        public async Task<IList<Book>> GetBooksByTitle(string title)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for book by title '{name}'.", nameof(BookTableDataAccess), nameof(GetBooksByTitle), title);

            var asyncAllRows = _tableClient.QueryAsync<BookModel>();
            var list = new List<Book>();
            await foreach (var bookEntity in asyncAllRows)
            {
                try
                {
                    Book book = ParseBookFromModel(bookEntity);
                    list.Add(book);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "[{Class}.{Method}] Unable to parse book '{isbn}'", nameof(BookTableDataAccess), nameof(GetBooksByTitle), bookEntity!.RowKey);
                    continue;
                }
            }

            return list.Where(b => b.Detail.Title.ToLower().Contains(title.ToLower())).ToList();
        }

        private static Book ParseBookFromModel(BookModel bookEntity)
        {
            return new Book(bookEntity.RowKey, true)
            {
                Publisher = bookEntity.PartitionKey,
                Detail = new BookDetail()
                {
                    Author = bookEntity.Author,
                    Description = bookEntity.Description,
                    ImageUrl = bookEntity.ImageUrl,
                    Link = bookEntity.Link,
                    Title = bookEntity.Title ?? string.Empty,
                    Created = bookEntity.Timestamp.HasValue ? bookEntity.Timestamp.Value.LocalDateTime : DateTime.Today,
                    Format = bookEntity.Format,
                    PublishYear = bookEntity.PublishYear
                }
            };
        }

        public async Task SaveBook(Book book)
        {
            var entity = new BookModel(book);
            await _tableClient.UpsertEntityAsync<BookModel>(entity);
        }
    }
}
