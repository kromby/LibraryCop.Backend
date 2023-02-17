using Azure;
using Azure.Data.Tables;
using LibraryCop.BusinessLogic.DataAccess;
using LibraryCop.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess.Model
{
    internal class StateModel : ITableEntity
    {
        public StateModel()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
        }

        public StateModel(BookState state)
        {
            PartitionKey = state.LibraryID.ToString();
            RowKey = state.ISBN;

            State = (int)state.State;

            CreatedBy = state.CreatedBy;
            Created = DateTime.SpecifyKind(state.Created, DateTimeKind.Utc);
            ChangedBy = state.ChangedBy ?? state.ChangedBy;
            Changed = state.Changed?.ToUniversalTime();
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public int State { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? ChangedBy { get; set; }
        public DateTime? Changed { get; set; }
    }
}
