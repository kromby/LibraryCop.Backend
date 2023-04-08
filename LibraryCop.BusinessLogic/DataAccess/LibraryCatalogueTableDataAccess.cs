using Azure.Data.Tables;
using BusinessLogic.DataAccess.Model;
using LibraryCop.BusinessLogic.DataAccess.Model;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
    public class LibraryCatalogueTableDataAccess : ILibraryCatalogoueDataAccess
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<LibraryCatalogueTableDataAccess> _log;

        public LibraryCatalogueTableDataAccess(ConnectionInfo connectionInfo, ILogger<LibraryCatalogueTableDataAccess> log)
        {
            _tableClient = new TableClient(connectionInfo.ConnectionString, "State");
            _log = log;
        }

        public async Task<BookState?> GetState(string isbn, Guid libraryID)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}'.", nameof(LibraryCatalogueTableDataAccess), nameof(GetState), isbn);

            var asyncResults = _tableClient.QueryAsync<StateModel>(x => x.RowKey == isbn && x.PartitionKey == libraryID.ToString());
            await foreach(var state in asyncResults)
            {
                _log.LogInformation("[{Class}.{Method}] Book state '{isbn}' found.", nameof(LibraryCatalogueTableDataAccess), nameof(GetState), isbn);

                var bookState = new BookState(isbn, Enum.Parse<State>(state.State.ToString()), libraryID)
                {
                    CreatedBy = state.CreatedBy,
                    Created = state.Created,                    
                };

                return bookState;
            }

            _log.LogInformation("[{Class}.{Method}] Book '{isbn}' not found.", nameof(LibraryCatalogueTableDataAccess), nameof(GetState), isbn);
            return null;
        }

        public async Task SaveBookState(BookState state)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}'.", nameof(LibraryCatalogueTableDataAccess), nameof(SaveBookState), state.ISBN);
            var model = new StateModel(state);

            try
            {
                var response = await _tableClient.UpsertEntityAsync<StateModel>(model);

                _log.LogInformation("[{Class}.{Method}] State save for '{isbn}' - '{message}'.", nameof(LibraryCatalogueTableDataAccess), nameof(SaveBookState), state.ISBN, response.Status);

            }catch(Exception ex)
            {
                _log.LogError(ex, "[{Class}.{Method}] State save for '{isbn}' - '{message}'.", nameof(LibraryCatalogueTableDataAccess), nameof(SaveBookState), state.ISBN, ex.Message);
            }
        }
    }
}
