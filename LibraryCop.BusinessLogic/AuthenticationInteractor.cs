using BusinessLogic;
using BusinessLogic.Entities;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCop.BusinessLogic
{
    public class AuthenticationInteractor
    {
        private const string LIBRARY_CLAIM = "library";

        private readonly AuthenticationInfo _authenticationInfo;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ILogger<AuthenticationInteractor> _log;

        public AuthenticationInteractor(AuthenticationInfo authenticationInfo, IUserDataAccess userDataAccess, ILogger<AuthenticationInteractor> log)
        {
            if (authenticationInfo == null)
                throw new ArgumentNullException(nameof(authenticationInfo));
            authenticationInfo.Validate();

            _authenticationInfo = authenticationInfo;
            _userDataAccess = userDataAccess;
            _log = log;
        }

        public User? GetUserFromToken(string scheme, string value)
        {
            if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(value))
            {
                _log.LogInformation("[{Class}.{Method}] Scheme or token value is missing. Scheme: '{scheme}'", nameof(AuthenticationInteractor), nameof(GetUserFromToken), scheme);
                return null;
            }

            if (scheme.Trim().ToLower() != "token")
            {
                _log.LogInformation("[{Class}.{Method}] Scheme is not token. Scheme: '{scheme}'", nameof(AuthenticationInteractor), nameof(GetUserFromToken), scheme);
                return null;
            }

            try
            {
                var user = DecryptToken(value);
                //if (string.IsNullOrWhiteSpace(userIdString))
                //{
                //    _log.LogInformation($"[{nameof(AuthenticationInteractor)}] UserId is missing. userIdString: '{userIdString}'");
                //    return null;
                //}

                return user;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"[{nameof(AuthenticationInteractor)}] Error in GetUserIdFromToken(..)");
                return null;
            }
        }

        public async Task<string> Login(string username, string password, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _log.LogInformation("[{Class}.{Method}] Username is null or empty", nameof(AuthenticationInteractor), nameof(Login));
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _log.LogInformation("[{Class}.{Method}] Password is null or empty", nameof(AuthenticationInteractor), nameof(Login));
                throw new ArgumentNullException(nameof(password));
            }

            string hashed = HashPassword(password.Trim(), Encoding.UTF32.GetBytes(_authenticationInfo.Salt));

            var user = await _userDataAccess.GetUser(username.Trim());

            if(user == null)
            {
                _log.LogWarning("[{Class}.{Method}] User '{Username}' not authenticated.", nameof(AuthenticationInteractor), nameof(Login), username);
                throw new UnauthorizedAccessException("Username and/or password incorrect.");
            }

            if(user.Password != hashed)
            {
                _log.LogWarning("[{Class}.{Method}] User '{Username}' not authenticated.", nameof(AuthenticationInteractor), nameof(Login), username);
                throw new UnauthorizedAccessException("Username and/or password incorrect.");
            }

            var jwt = GetToken(user.UserID, user.LibraryID, user.Name);

            return jwt;
        }

        private static string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 10000, 256 / 8));
        }


        private string GetToken(Guid userID, Guid libraryID, string name)
        {
            _log.LogInformation($"[{nameof(AuthenticationInteractor)}] Starting GetToken(..)");
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userID.ToString()), // subject                
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // unique identifier - JWT ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // issued at
                new Claim(JwtRegisteredClaimNames.Name, name), // End-User's full name in displayable form including all name parts
                new Claim(LIBRARY_CLAIM, libraryID.ToString()), // Library
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationInfo.Key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(_authenticationInfo.Issuer, _authenticationInfo.Audience, claims, expires: DateTime.Now.AddMinutes(7*24*60), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User? DecryptToken(string token)
        {
            _log.LogInformation($"[{nameof(AuthenticationInteractor)}] Starting DecryptToken(..)");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationInfo.Key));
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _authenticationInfo.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _authenticationInfo.Audience
                };

                if (handler.ValidateToken(token, validations, out var tokenSecure).Identity is not ClaimsIdentity identity)
                {
                    throw new Exception("boom - Identity is not valid");
                }

                //return identity.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                var userID = Guid.Parse(identity.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var libraryID = Guid.Parse(identity.Claims.First(c => c.Type == LIBRARY_CLAIM).Value);
                var name = identity.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value;

                User user = new(userID, libraryID, name);

                return user;
            }
            catch (SecurityTokenExpiredException steeex)
            {
                _log.LogError(steeex, $"[{nameof(AuthenticationInteractor)}] Token expired");
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"[{nameof(AuthenticationInteractor)}] Unexpected error decrypting token");
                return null;
            }
        }
    }
}
