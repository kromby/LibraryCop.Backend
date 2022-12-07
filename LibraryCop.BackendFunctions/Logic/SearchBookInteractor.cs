using Azure.Data.Tables;
using LibraryCop.BackendFunctions.Logic.Entities;
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

        public SearchBookInteractor()
        {
            var connectionString = Environment.GetEnvironmentVariable("ContentStorage.ConnectionString");
            _rawTableClient = new TableClient(connectionString, "LibraryCopRaw");
            _parsedTableClient = new TableClient(connectionString, "LibraryCop");
        }

        public async Task<Book> GetBook(string isbn)
        {
            var book = GetBookLocally(isbn);

            if(book == null)
            {
                var internetBookInfo = await GetBookFromInternet(isbn);
                var rawTask = SaveRawBookLocally(internetBookInfo, isbn, "Unknown");
                book = ParseInernetBook(internetBookInfo);                
                var parsedTask = SaveBookLocally(book);

                rawTask.Wait();
                parsedTask.Wait();
            }

            return book;
        }

        private Book GetBookLocally(string isbn)
        {
            var result = _parsedTableClient.Query<BookEntity>(x => x.RowKey == isbn);

            if(result.Count() == 1)
            {
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

            return null;
        }

        private async Task<string> GetBookFromInternet(string isbn)
        {
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

        private Book ParseInernetBook(string json)
        {
            var result = JsonConvert.DeserializeObject<BoksalaResult>(json);

            var hit = result.hits.hits.First();

            Book book = new()
            {
                ISBN = hit._source.print_isbn_canonical,
                Author = hit._source.authors != null && hit._source.authors.Count() > 0 ? hit._source.authors.First().name : "Unknown",
                Description = hit._source.description,
                ImageUrl = hit._source.cover_image,
                Link = hit._source.permalink,
                Publisher = hit._source.publisher,
                Title = hit._source.title
            };

            return book;
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
