using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Model
{
    internal class UserModel : ITableEntity
    {
        public UserModel()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            Name = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            LibraryID = new Guid();
            CreatedBy = new Guid();
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid LibraryID { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
