using LibraryCop.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic
{
    public interface IBookFinderDataAccess
    {
        /// <summary>
        /// Data accesses are ordered by priority and check in the order from lowest to highest.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Retrieves information about a specific book.
        /// </summary>
        /// <param name="isbn">The International Standard Book Number.</param>
        /// <returns>Information about a book.</returns>
        public Task<Book?> GetBook(string isbn);
    }
}
