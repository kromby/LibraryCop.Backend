using LibraryCop.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IUserDataAccess
    {
        Task<User?> GetUser(string username);

        Task<User?> GetUser(Guid id);
    }
}
