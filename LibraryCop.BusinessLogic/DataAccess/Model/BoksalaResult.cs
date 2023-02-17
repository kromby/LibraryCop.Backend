using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Model
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles
    public class BoksalaResult
    {
        public int Took { get; set; }
        public bool Timed_out { get; set; }
        public _Shards _shards { get; set; }
        public Hits hits { get; set; }
    }

    public class _Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class Hits
    {
        public Total total { get; set; }
        public float max_score { get; set; }
        public Hit[] hits { get; set; }
    }

    public class Total
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public float _score { get; set; }
        public _Source _source { get; set; }
    }

    public class _Source
    {
        public string sku { get; set; }
        public string create_date { get; set; }
        public string update_date { get; set; }
        public object delete_date { get; set; }
        public string print_isbn_canonical { get; set; }
        public object eisbn_canonical { get; set; }
        public string title { get; set; }
        public string publisher { get; set; }
        public string kind { get; set; }
        public object format { get; set; }
        public object contributers { get; set; }
        public object language { get; set; }
        public object sales_rights { get; set; }
        public object exclude_sales_rights { get; set; }
        public string description { get; set; }
        public object publication_date { get; set; }
        public object subtitle { get; set; }
        public string cover_image { get; set; }
        public string edition { get; set; }
        public object off_sale_date_ok { get; set; }
        public object page_count { get; set; }
        public object imprint_name { get; set; }
        public object publisher_list_price { get; set; }
        public Author[] authors { get; set; }
        public object[] variations { get; set; }
        public string min_price { get; set; }
        public string max_price { get; set; }
        public int variations_count { get; set; }
        public string price { get; set; }
        public string book_type { get; set; }
        public string book_type_cat { get; set; }
        public int book_type_rank { get; set; }
        public Product_Categories_Lvl product_categories_lvl { get; set; }
        public object bokalistar { get; set; }
        public object[] bokalistar_ids { get; set; }
        public object bokalistar_lvl { get; set; }
        public object bokalisti { get; set; }
        public string permalink { get; set; }
    }

    public class Product_Categories_Lvl
    {
        public string[] lvl0 { get; set; }
        public string[] lvl1 { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
