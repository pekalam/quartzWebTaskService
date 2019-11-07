using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApi.Auth
{
    public class ApiKeyService
    {
        private readonly ApiKeysStrings _apiKeysStrings;
        private ILogger<ApiKeyService> _logger;

        public ApiKeyService(ApiKeysStrings apiKeysStrings, ILogger<ApiKeyService> logger)
        {
            _apiKeysStrings = apiKeysStrings;
            _logger = logger;
        }

        public bool Authenticate(string apiKeyString, out ApiKey apiKey)
        {
            if (apiKeyString.Equals(_apiKeysStrings.ClientKey))
            {
                apiKey = ApiKey.From(apiKeyString, true);
                return true;
            }

            if (apiKeyString.Equals(_apiKeysStrings.ManagmentAppKey))
            {
                apiKey = ApiKey.From(apiKeyString, false);
                return true;
            }

            apiKey = null;
            return false;
        }
    }
}
