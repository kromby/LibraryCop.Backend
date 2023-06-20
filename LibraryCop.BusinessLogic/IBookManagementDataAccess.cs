using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic
{
    public interface IBookManagementDataAccess
    {
        public Task<string> SaveBook(Book book);        

        //public int GetStats { get; set; }

        //public int GetTags { get; set; }
    }
}
