using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.Entities
{
    public class BookDetail
    {
        public BookDetail()
        {
            Title = string.Empty;                        
            Created = DateTime.Today;
        }

        public string Title { get; set; }        
        public int? PublishYear { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? Link { get; set; }
        public string? Format { get; set; }        
        public DateTime Created { get; set; }
    }
}
