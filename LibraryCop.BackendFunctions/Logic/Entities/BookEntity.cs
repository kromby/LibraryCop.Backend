using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BackendFunctions.Logic.Entities
{
    internal class BookEntity : ITableEntity
    {
        public BookEntity()
        {

        }

        public BookEntity(Book book)
        {
            RowKey = book.ISBN;
            PartitionKey = book.Publisher;
            Title = book.Title;
            Description = book.Description;
            Author = book.Author;
            ImageUrl= book.ImageUrl;
            Link = book.Link;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }
}
