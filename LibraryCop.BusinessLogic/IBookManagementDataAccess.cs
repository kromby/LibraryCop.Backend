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
        public Task SaveBook(Book book);

        //public int GetState { get; set; }

        //public int GetStats { get; set; }

        //public int GetTags { get; set; }
    }
}
