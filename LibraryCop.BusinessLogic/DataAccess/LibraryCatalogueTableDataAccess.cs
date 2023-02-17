using Azure.Data.Tables;
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

        public async Task SaveBookState(BookState state)
        {
            _log.LogInformation("[{Class}.{Method}] Saving state for book '{isbn}'.", nameof(LibraryCatalogueTableDataAccess), nameof(SaveBookState), state.ISBN);
            var model = new StateModel(state);

            try
            {
                var response = await _tableClient.AddEntityAsync<StateModel>(model);

                _log.LogInformation("[{Class}.{Method}] State save for '{isbn}' - '{message}'.", nameof(LibraryCatalogueTableDataAccess), nameof(SaveBookState), state.ISBN, response.Status);

            }catch(Exception ex)
            {
                _log.LogError(ex, "[{Class}.{Method}] State save for '{isbn}' - '{message}'.", nameof(LibraryCatalogueTableDataAccess), nameof(SaveBookState), state.ISBN, ex.Message);
            }
        }
    }
}
