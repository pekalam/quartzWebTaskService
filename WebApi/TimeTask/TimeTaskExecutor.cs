using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using WebApi.Auth;

namespace WebApi.TimeTask
{
    public class TimeTaskExecutor
    {
        private HttpClient _httpClient = new HttpClient();
        private ICallData _callData;
        private Uri _endpointUri;
        private ILogger _logger;
        private AsyncPolicy _clientPolicy;

        public TimeTaskExecutor(Uri endpointUri, ICallData callData, ILogger logger, ApiKeysStrings apiKeys)
        {
            _endpointUri = endpointUri;
            _callData = callData;
            _logger = logger;
            _clientPolicy = Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(new[] {TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)},
                    (exception, span) =>
                    {
                        _logger.LogDebug($"Retrying after exception: {exception} {exception.Message}");
                    });
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKeys.ClientKey);
        }

        public async Task<bool> Exec()
        {
            var result = await _clientPolicy.ExecuteAndCaptureAsync(() =>
            {
                return _httpClient.PostAsync(_endpointUri, _callData, new JsonMediaTypeFormatter()
                    { SerializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All } });
            });
            var response = result.Result; 

            if (response?.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}