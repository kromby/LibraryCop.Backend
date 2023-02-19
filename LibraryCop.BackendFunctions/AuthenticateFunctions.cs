using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using BackendFunctions.Model;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using LibraryCop.BusinessLogic;

namespace BackendFunctions
{
    public class AuthenticateFunctions
    {
        private readonly AuthenticationInteractor _authenticationInteractor;

        public AuthenticateFunctions(AuthenticationInteractor authenticationInteractor)
        {
            _authenticationInteractor = authenticationInteractor;
        }



        [FunctionName("authenticate")]
        public async Task<IActionResult> RunAuthenticate(
                   [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
                   ILogger log)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            log.LogInformation("[{Class}.{Method}] C# HTTP trigger function processed a request.", nameof(AuthenticateFunctions), nameof(RunAuthenticate));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            AuthenticateBody body = JsonConvert.DeserializeObject<AuthenticateBody>(requestBody);

            if(body == null)
            {
                log.LogInformation("[{{Class}}.{{Method}}] Body is invalid.", nameof(AuthenticateFunctions), nameof(RunAuthenticate));
                return new BadRequestResult();
            }

            //log.LogInformation($"[RunAuthenticate] Request username: {body.Username}");
            log.LogInformation("[{{Class}}.{{Method}}] Request IP Address: {IP}", nameof(AuthenticateFunctions), nameof(RunAuthenticate), req.HttpContext.Connection.RemoteIpAddress.MapToIPv4());

            try
            {
                var jwt = await _authenticationInteractor.Login(body.Username, body.Password, req.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
                return new OkObjectResult(jwt);
            }
            catch (ArgumentException aex)
            {
                log.LogError(aex, "[{Class}.{Method}] Invalid input", nameof(AuthenticateFunctions), nameof(RunAuthenticate));
                return new BadRequestObjectResult(aex.Message);
            }
            catch (UnauthorizedAccessException uaex)
            {
                log.LogError(uaex, "[{Class}.{Method}] Unauthorized", nameof(AuthenticateFunctions), nameof(RunAuthenticate));
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "[{Class}.{Method}] Unhandled error", nameof(AuthenticateFunctions), nameof(RunAuthenticate));
                throw;
            }
            finally
            {
                stopwatch.Stop();
                log.LogInformation("[{Class}.{Method}] Elapsed: {ElapsedMilliseconds} ms.", nameof(AuthenticateFunctions), nameof(RunAuthenticate), stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
