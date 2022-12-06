using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BackendFunctions.Logic.Entities
{
    public class Book
    {
        public string ISBN { get; set; }

        public string Title { get; set; }

        public string Publisher { get; set; }

        public string Description { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }
}
