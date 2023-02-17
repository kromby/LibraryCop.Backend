using LibraryCop.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic
{
    public interface ILibraryCatalogoueDataAccess
    {
        Task SaveBookState(BookState state);
    }
}
