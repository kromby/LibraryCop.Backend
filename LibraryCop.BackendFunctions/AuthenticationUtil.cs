using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendFunctions
{
    internal class AuthenticationUtil
    {
        private const string AUTHORIZATION_HEADER_NAME = "X-Custom-Authorization";

        public static bool GetAuthenticatedUser(AuthenticationInteractor authenticationInteractor, IHeaderDictionary headers, out AuthenticatedUser user, ILogger log)
        {
            log.LogInformation("[{Class}.{Method}] AUTHORIZATION_HEADER_NAME: '{AUTHORIZATION_HEADER_NAME}'", nameof(AuthenticationUtil), nameof(GetAuthenticatedUser), AUTHORIZATION_HEADER_NAME);
            if (!headers.ContainsKey(AUTHORIZATION_HEADER_NAME))
            {
                log.LogInformation("[{Class}.{Method}]  AuthorisationHeader is missing.", nameof(AuthenticationUtil), nameof(GetAuthenticatedUser));
                user = null;
                return false;
            }

            var authorizationHeader = headers[AUTHORIZATION_HEADER_NAME].ToString().Split(" ");

            user = authenticationInteractor.GetUserFromToken(authorizationHeader[0], authorizationHeader[1]);
            return true;
        }
    }
}
