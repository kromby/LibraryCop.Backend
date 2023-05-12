using Azure.Data.Tables;
using BusinessLogic.DataAccess.Model;
using LibraryCop.BusinessLogic.DataAccess;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public class UserTableDataAccess : IUserDataAccess
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<UserTableDataAccess> _log;

        public UserTableDataAccess(ConnectionInfo connectionInfo, ILogger<UserTableDataAccess> log)
        {
            _tableClient = new TableClient(connectionInfo.ConnectionString, "User");
            _log = log;
        }

        public async Task<User?> GetUser(string username)
        {
            _log.LogInformation("[{Class}.{Method}] Looking for user '{username}'.", nameof(UserTableDataAccess), nameof(GetUser), username);

            var asyncResults = _tableClient.QueryAsync<UserModel>(x => x.Username == username);

            int count = 0;
            User? user = null;
            await foreach (var userEntity in asyncResults)
            {
                user = ParseUserFromModel(userEntity);
                count++;
            }

            if(count == 1)
                return user;

            _log.LogInformation("[{Class}.{Method}] User '{username}' not found.", nameof(UserTableDataAccess), nameof(GetUser), username);
            return null;
        }

        private static User ParseUserFromModel(UserModel userEntity)
        {
            var userID = Guid.Parse(userEntity.RowKey);
            var libraryID = Guid.Parse(userEntity.PartitionKey);

            return new User(userID, libraryID, userEntity.Name)
            {
                Password = userEntity.Password
            };
        }
    }
}
