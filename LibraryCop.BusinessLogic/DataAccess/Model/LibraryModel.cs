using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Model
{
    internal class LibraryModel : ITableEntity
    {
        public LibraryModel()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            Name = string.Empty;
            Created = DateTime.Now;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Name { get; set; }
    }
}
