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

            Publisher = string.Empty;
            Detail = new();
            State = new(isbn, Entities.State.NotOwned, Guid.Empty);
            Operations = new List<BookOperation>();
            Labels = new List<string>();

            IsComplete = false;
        }

        internal bool Saved { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public bool IsComplete { get; set; }
        public BookDetail Detail { get; set; }
        public BookState State { get; set; }

        public IList<BookOperation> Operations { get; set; }

        public IList<string> Labels { get; set; }
    }
}
