using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic.Entities
{
    /// <summary>
    /// User information stored in JWT.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the LibraryCop.BusinessLogic.Entities.AuthenticatedUser using the specified
        /// user identifier, library identifier and name of user.
        /// </summary>
        /// <param name="userID">Unique identifier for an user.</param>
        /// <param name="libraryID">Unique identifier for the library that an user is associated to.</param>
        /// <param name="name">Name of user.</param>
        public User(Guid userID, Guid libraryID, string name)
        {
            UserID = userID;
            LibraryID = libraryID;
            Name = name;
            Password = string.Empty;
        }

        /// <summary>
        /// Unique identifier for an user.
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Unique identifier for the library that an user is associated to.
        /// </summary>
        public Guid LibraryID { get; set; }

        /// <summary>
        /// Name of user.
        /// </summary>
        public string Name { get; set; }

        internal string Password { get; set; }
    }
}
