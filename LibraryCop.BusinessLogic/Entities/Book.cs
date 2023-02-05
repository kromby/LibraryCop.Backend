using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.Entities
{
    public class Book
    {
        public Book(string isbn, bool saved)
        {
            ISBN = isbn;            
            Saved = saved;

            Title = string.Empty;
            Publisher = string.Empty;
            Labels = new List<string>();
            Created = DateTime.Today;
        }

        internal bool Saved { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public int? PublishYear { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? Link { get; set; }        
        public string? Format { get; set; }
        public IList<string> Labels { get; set; }
        public DateTime Created { get; set; }
    }
}
