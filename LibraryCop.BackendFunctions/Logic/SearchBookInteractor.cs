using Azure.Data.Tables;
using LibraryCop.BackendFunctions.Logic.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BackendFunctions.Logic
{
    public class SearchBookInteractor
    {
        private readonly TableClient _rawTableClient;
        private readonly TableClient _parsedTableClient;
        private readonly ILogger _log;

        public SearchBookInteractor(ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("ContentStorage.ConnectionString");
            _rawTableClient = new TableClient(connectionString, "LibraryCopRaw");
            _parsedTableClient = new TableClient(connectionString, "LibraryCop");
            _log = log;
        }

        public async Task<Book> GetBook(string isbn)
        {
            _log.LogInformation("[{Method}] Looking for book '{isbn}'.", nameof(GetBook), isbn);

            var book = GetBookLocally(isbn);

            if (book == null)
            {
                var internetBookInfo = await GetBookFromLeitir(isbn);
                var rawTask = SaveRawBookLocally(internetBookInfo, isbn, "Leitir");
                book = ParseLeitirBook(internetBookInfo);

                if (book == null)
                {
                    _log.LogInformation("[{Method}] Book '{isbn}' not found at Leitir.", nameof(GetBook), isbn);
                    internetBookInfo = await GetBookFromBoksala(isbn);
                    rawTask.Wait();
                    rawTask = SaveRawBookLocally(internetBookInfo, isbn, "Boksalan");
                    book = ParseBoksalaBook(internetBookInfo);
                }

                if (book != null)
                {
                    var parsedTask = SaveBookLocally(book);
                    parsedTask.Wait();
                }

                rawTask.Wait();                
            }

            if(book != null)
            {
                _log.LogInformation("[{Method}] Book '{isbn}': '{Title}' found!", nameof(GetBook), isbn, book.Title);
            }

            return book;
        }

        private Book GetBookLocally(string isbn)
        {
            var result = _parsedTableClient.Query<BookEntity>(x => x.RowKey == isbn);
            if (result.Count() == 1)
            {
                _log.LogInformation("[{Method}] Book '{isbn}' found locally.", nameof(GetBookLocally), isbn);

                var book = new Book()
                {
                    Author = result.First().Author,
                    Description = result.First().Description,
                    ImageUrl = result.First().ImageUrl,
                    ISBN = result.First().RowKey,
                    Link = result.First().Link,
                    Publisher = result.First().PartitionKey,
                    Title = result.First().Title
                };

                return book;
            }

            _log.LogInformation("[{Method}] Book '{isbn}' not found.", nameof(GetBookLocally), isbn);
            return null;
        }

        private async Task<string> GetBookFromLeitir(string isbn)
        {
            _log.LogInformation("[{Method}] Looking for book '{isbn}' from Leitir.", nameof(GetBookFromLeitir), isbn);
            using HttpClient client = new();
            var url = $"https://leitir.is/primaws/rest/pub/pnxs?q=any,contains,{isbn}&vid=354ILC_NETWORK:10000_UNION";
            return await client.GetStringAsync(url);
        }

        private async Task<string> GetBookFromBoksala(string isbn)
        {
            _log.LogInformation("[{Method}] Looking for book '{isbn}' from Boksala.", nameof(GetBookFromBoksala), isbn);
            using HttpClient client = new();
            string body = new StringBuilder().Append("{\"query\":{\"match\":{\"print_isbn_canonical\":").Append(isbn).Append("}}}").ToString();
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://www.boksala.is/elastic/boksala_index/_search"),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);

            var result = await response.Content.ReadAsStringAsync();

            return result;

            //return "{\"took\": 1,\"timed_out\": false,\"_shards\": {  \"total\": 1,  \"successful\": 1,  \"skipped\": 0,  \"failed\": 0},\"hits\": {  \"total\": {    \"value\": 1,    \"relation\": \"eq\"  },  \"max_score\": 12.730946,  \"hits\": [    {      \"_index\": \"boksala_index\",      \"_type\": \"_doc\",      \"_id\": \"dk1198887\",      \"_score\": 12.730946,      \"_source\": {        \"sku\": \"9789979332817\",        \"create_date\": \"2020-04-22 19:03:47\",        \"update_date\": \"2021-12-01 00:13:06\",        \"delete_date\": null,        \"print_isbn_canonical\": \"9789979332817\",        \"eisbn_canonical\": null,        \"title\": \"S\\u00c1 HL\\u00c6R BEST...! SAG\\u00d0I PABBI\",        \"publisher\": \"MALOGME1\",        \"kind\": \"book\",        \"format\": null,        \"contributers\": null,        \"language\": null,        \"sales_rights\": null,        \"exclude_sales_rights\": null,        \"description\": \" \",        \"publication_date\": null,        \"subtitle\": null,        \"cover_image\": \"https:\\/\\/www.boksala.is\\/wp-content\\/uploads\\/2017\\/10\\/9789979332817.jpg\",        \"edition\": \"1\",        \"off_sale_date_ok\": null,        \"page_count\": null,        \"imprint_name\": null,        \"publisher_list_price\": null,        \"authors\": [          {            \"name\": \"BERGSTR\\u00d6M, GUNILLA\"          }        ],        \"variations\": [],        \"min_price\": \"2790\",        \"max_price\": \"2790\",        \"variations_count\": 0,        \"price\": \"2790\",        \"book_type\": \"dk\",        \"book_type_cat\": \"B\\u00e6kur\",        \"book_type_rank\": 100,        \"product_categories_lvl\": {          \"lvl0\": [            \"Barna- og unglingab\\u00e6kur\"          ],          \"lvl1\": [            \"Barna- og unglingab\\u00e6kur > Barnab\\u00e6kur\"          ]        },        \"bokalistar\": null,        \"bokalistar_ids\": [],        \"bokalistar_lvl\": null,        \"bokalisti\": null,        \"permalink\": \"https:\\/\\/www.boksala.is\\/product\\/sa-hlaer-best-sagdi-pabbi\\/\"      }    }  ]}\r\n}";
        }

        private Book ParseLeitirBook(string json)
        {
            var result = JsonConvert.DeserializeObject<LeitirResult>(json);

            try
            {
                var docs = result.docs.First();

                Book book = new()
                {
                    Title = docs.pnx.display.title.First(),
                    Author = docs.pnx.addata.au != null ? docs.pnx.addata.au.First() : docs.pnx.display.creator != null ? docs.pnx.display.creator.First() : "Unknown",
                    Publisher = docs.pnx.addata.pub.First() ?? docs.pnx.display.publisher.First(),
                    Description = docs.pnx.display.subject.First(),
                    ISBN = docs.pnx.display.identifier.First().Replace("$$CISBN$$V", "")
                };

                book.ImageUrl = $"https://proxy-euf.hosted.exlibrisgroup.com/exl_rewrite/syndetics.com/index.php?client=primo&isbn={book.ISBN}/sc.jpg";

                if (int.TryParse(docs.pnx.addata.date.First(), out int year))
                {
                    book.PublishYear = year;
                }

                return book;
            }
            catch(Exception ex)
            {
                _log.LogError(ex, ex.Message);
                return null;
            }           
        }

        private Book ParseBoksalaBook(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<BoksalaResult>(json);

                var hit = result.hits.hits.First();

                Book book = new()
                {
                    ISBN = hit._source.print_isbn_canonical,
                    Author = hit._source.authors != null && hit._source.authors.Length > 0 ? hit._source.authors.First().name : "Unknown",
                    Description = hit._source.description,
                    ImageUrl = hit._source.cover_image,
                    Link = hit._source.permalink,
                    Publisher = hit._source.publisher,
                    Title = hit._source.title
                };

                return book;
            }
            catch(Exception ex)
            {
                _log.LogError(ex, ex.Message);
                return null;
            }
        }

        private async Task SaveRawBookLocally(string book, string key, string publisher)
        {
            var rawEntity = new BookRawEntity()
            {
                PartitionKey = publisher,
                RowKey = key,
                JSON = book
            };
            await _rawTableClient.UpsertEntityAsync<BookRawEntity>(rawEntity);
        }

        private async Task SaveBookLocally(Book book)
        {
            var entity = new BookEntity(book);
            await _parsedTableClient.UpsertEntityAsync<BookEntity>(entity);
        }
    }
}
