using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string? StateName
        {
            get
            {
                if (State == State.NotOwned) return "Bók ekki til í bókasafni";
                else if (State == State.In) return "Bók er aðgengileg á bókasafninu";
                else if (State == State.OnLoan) return string.Format("Bók er í láni hjá {0}", LastChangedBy.ToString());
                else return "Staða óþekkt";
            }
        }
        public Guid LibraryID { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? ChangedBy { get; set; }
        public DateTime? Changed { get; set; }

        public DateTime LastChanged { get { return Changed ?? Created; } }
        public Guid LastChangedBy { get { return ChangedBy ?? CreatedBy; } }
    }

    public enum State
    {
        NotOwned,
        In,
        OnLoan
    }
}
