using Azure;
using BusinessLogic.DataAccess.Model;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.DataAccess;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
    public class LeitirApiDataAccess : IBookFinderDataAccess
    {
        private readonly ILogger<BookTableDataAccess> _log;
        public int Priority { get; private set; }

        public LeitirApiDataAccess(ILogger<BookTableDataAccess> log)
        {
            _log = log;
            Priority = 2;
        }

        public async Task<Book?> GetBook(string isbn)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for book '{isbn}'.", nameof(LeitirApiDataAccess), nameof(GetBook), isbn);
            using HttpClient client = new();
            var url = $"https://leitir.is/primaws/rest/pub/pnxs?q=any,contains,{isbn}&vid=354ILC_NETWORK:10000_UNION";

            var result = await client.GetAsync(url);
            if(!result.IsSuccessStatusCode)
            {
                _log.LogInformation("[{Class}.{Method}] Invalid API call for book '{isbn}' StatusCode: '{StatusCode}'", nameof(LeitirApiDataAccess), nameof(GetBook), isbn, result.StatusCode);
                return null;
            }

            var json = await result.Content.ReadAsStringAsync();
            
            return Parse(json, isbn);
        }

        private Book? Parse(string json, string isbn)
        {
            var result = JsonConvert.DeserializeObject<LeitirResult>(json);

            if (result == null)
            {
                _log.LogInformation("[{Class}.{Method}] Could not parse JSON for book '{isbn}'", nameof(LeitirApiDataAccess), nameof(Parse), isbn);
                return null;
            }

            try
            {
                var docs = result.docs.First();

                Book book = new(isbn, false)
                {                    
                    Publisher = docs.pnx.addata.pub.First() ?? docs.pnx.display.publisher.First(),               
                    Detail = new BookDetail()
                    {
                        Title = docs.pnx.display.title.First(),
                        Author = docs.pnx.addata.au != null ? docs.pnx.addata.au.First() : docs.pnx.display.creator != null ? docs.pnx.display.creator.First() : "Unknown",
                    }
                };

                if(docs.pnx.display.format != null)
                {
                    book.Detail.Format = docs.pnx.display.format.First();
                }

                if (int.TryParse(docs.pnx.addata.date.First(), out int year))
                {
                    book.Detail.PublishYear = year;
                }

                foreach(var subject in docs.pnx.display.subject)
                {
                    book.Labels.Add(subject);
                }

                return book;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "[{Class}.{Method}] Exception processin book '{isbn}': {Exception}", nameof(LeitirApiDataAccess), nameof(Parse), isbn, ex.Message);
                _log.LogInformation("[{Class}.{Method}] Json: {Json}", nameof(LeitirApiDataAccess), nameof(Parse), json);
                return null;
            }
        }
    }
}
