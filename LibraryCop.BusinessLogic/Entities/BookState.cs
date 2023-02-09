using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.Entities
{
    public class BookState
    {
        public State State { get; set; }
    }

    public enum State
    {
        Wishlist,
        In,
        OnLoan,
        NotOwned
    }
}
