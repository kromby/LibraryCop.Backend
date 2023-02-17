using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.Entities
{
    public class BookState
    {
        public BookState(string isbn, State state, Guid libraryID)
        {
            ISBN = isbn;
            State = state;
            LibraryID = libraryID; 
        }

        public string ISBN { get; set; }
        public State State { get; set; }
        public Guid LibraryID { get; set; }
        
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? ChangedBy { get; set; }
        public DateTime? Changed { get; set; }
    }

    public enum State
    {
        In,
        OnLoan,
        NotOwned
    }
}
