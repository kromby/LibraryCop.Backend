using BusinessLogic.DataAccess.Model;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.DataAccess
{
    public class BoksalaApiDataAccess : IBookFinderDataAccess
    {
        private readonly ILogger<BoksalaApiDataAccess> _log;
        public int Priority { get; private set; }

        public BoksalaApiDataAccess(ILogger<BoksalaApiDataAccess> log)
        {
            _log = log;
            Priority = 3;
        }

        public async Task<Book?> GetBook(string isbn)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for book '{isbn}' from Boksala.", nameof(BoksalaApiDataAccess), nameof(GetBook), isbn);
            using HttpClient client = new();
            string body = new StringBuilder().Append("{\"query\":{\"bool\":{\"should\":[{\"multi_match\":{\"query\":\"").Append(isbn).Append("\",\"type\":\"most_fields\",\"fields\":[\"print_isbn_canonical\",\"eisbn_canonical\",\"sku\"]}}]}}}").ToString();
            _log.LogDebug("[{Class}.{Method}] Book: '{isbn}' Body: '{body}'", nameof(BoksalaApiDataAccess), nameof(GetBook), isbn, body);
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://www.boksala.is/elastic/boksala_elastic3/_search"),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _log.LogInformation("[{Class}.{Method}] Invalid API call for book '{isbn}' StatusCode: '{StatusCode}'", nameof(BoksalaApiDataAccess), nameof(GetBook), isbn, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();

            return Parse(result, isbn);
        }

        public async Task<Book> GetBook(string isbn, Book partialBook)
        {
            var tempBook = await GetBook(isbn);

            if (tempBook != null)
            {
                partialBook.Detail.ImageUrl = tempBook.Detail.ImageUrl;
                partialBook.Detail.Link = tempBook.Detail.Link;
            }

            partialBook.IsComplete = true;
            partialBook.Saved = false;
            return partialBook;
        }

        private Book? Parse(string json, string isbn)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<BoksalaResult>(json);
                if (result == null)
                {
                    _log.LogInformation("[{Class}.{Method}] Could not parse JSON for book '{isbn}'", nameof(BoksalaApiDataAccess), nameof(Parse), isbn);
                    return null;
                }

                var hit = result.hits.hits.First();

                Book book = new(isbn, false, Guid.NewGuid())
                {                    
                    Publisher = hit._source.publisher,
                    Detail = new BookDetail()
                    {
                        Author = hit._source.authors != null && hit._source.authors.Length > 0 ? hit._source.authors.First().name : "Unknown",
                        Description = string.IsNullOrWhiteSpace(hit._source.description) ? null : hit._source.description,
                        ImageUrl = hit._source.cover_image,
                        Link = hit._source.permalink,
                        Title = hit._source.title,
                    }                                 
                };

                var textInfo = CultureInfo.CurrentCulture.TextInfo;                
                book.Publisher = textInfo.ToTitleCase(book.Publisher.ToLower());
                book.Detail.Author = textInfo.ToTitleCase(book.Detail.Author.ToLower());
                book.Detail.Title = textInfo.ToTitleCase(book.Detail.Title.ToLower());

                if (int.TryParse(hit._source.publication_date.ToString(), out int year))
                {
                    book.Detail.PublishYear = year;
                }

                if(hit._source.product_categories_lvl != null)
                {
                    foreach(var category in hit._source.product_categories_lvl.lvl0)
                    {
                        book.Labels.Add(category);
                    }
                }

                return book;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "[{Class}.{Method}] Exception processin book '{isbn}': {Exception}", nameof(BoksalaApiDataAccess), nameof(Parse), isbn, ex.Message);
                _log.LogInformation("[{Class}.{Method}] Json: {Json}", nameof(BoksalaApiDataAccess), nameof(Parse), json);
                return null;
            }
        }

        public Task<IList<Book>> GetBooksByTitle(string title)
        {
            throw new NotImplementedException();
        }        
    }
}
