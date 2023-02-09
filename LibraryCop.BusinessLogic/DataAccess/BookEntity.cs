using Azure;
using Azure.Data.Tables;
using LibraryCop.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
    internal class BookEntity : ITableEntity
    {
        public BookEntity()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            Author = string.Empty;
        }

        public BookEntity(Book book)
        {
            RowKey = book.ISBN;
            PartitionKey = book.Publisher;
            Title = book.Detail.Title;
            Description = book.Detail.Description;
            Author = book.Detail.Author ?? "Unknown";
            ImageUrl = book.Detail.ImageUrl;
            Link = book.Detail.Link;
            Format = book.Detail.Format;
            PublishYear = book.Detail.PublishYear;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? Link { get; set; }
        public string? Format { get; set; }
        public int? PublishYear { get; set; }
    }
}
