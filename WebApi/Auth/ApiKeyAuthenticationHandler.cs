using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Util;

namespace WebApi.Auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly ApiKeyService _apiKeyService;
        private const string ApiKeyHeader = "X-API-Key";
        private ILogger<ApiKeyAuthenticationHandler> _logger;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, ApiKeyService apiKeyService) : base(options, logger, encoder, clock)
        {
            _apiKeyService = apiKeyService;
            _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.TryGetValue(ApiKeyHeader, out var apiHeaderValues))
            {
                if (apiHeaderValues.Count == 0 || apiHeaderValues[0].IsNullOrWhiteSpace())
                {
                    _logger.LogDebug($"Invalid {ApiKeyHeader} header value");
                    return Task.FromResult(AuthenticateResult.NoResult());
                }

                if (_apiKeyService.Authenticate(apiHeaderValues[0], out var apiKey))
                {
                    var claimsIdentity = new ClaimsIdentity(new []{apiKey.Claim});
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    var authTicket = new AuthenticationTicket(claimsPrincipal, ApiKeyAuthenticationOptions.Scheme);
                    _logger.LogDebug($"Successful authentication: {apiKey.Claim.Value} ({apiKey.Key})");
                    return Task.FromResult(AuthenticateResult.Success(authTicket));
                }
                else
                {
                    _logger.LogWarning($"Invalid API-Key received: {apiHeaderValues[0]}");
                    return Task.FromResult(AuthenticateResult.Fail("Invalid API-Key"));
                }
            }
            else
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
        }
    }
}