using Newtonsoft.Json;

namespace CloudContactApi
{
    public class CloudContactApiService
    {
        public HttpClient HttpClient { get; }
        public string TenantUrl { get; }
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _scope;
        private readonly string _grantType;

        public CloudContactApiService(HttpClient httpClient, string tenantUrl, string clientId, string clientSecret, string scope, string grantType)
        {
            HttpClient = httpClient;
            TenantUrl = tenantUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = scope;
            _grantType = grantType;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{TenantUrl}/oauth/token");
            var collection = new List<KeyValuePair<string, string>>
            {
                new("client_id", _clientId),
                new("client_secret", _clientSecret),
                new("scope", _scope),
                new("grant_type", _grantType)
            };
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Authentication failed with status code {response.StatusCode}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            return ExtractTokenFromResponse(responseData);
        }

        private string ExtractTokenFromResponse(string responseData)
        {
            if (string.IsNullOrWhiteSpace(responseData))
            {
                throw new InvalidOperationException("Response data is null or empty");
            }

            var json = JsonConvert.DeserializeObject<dynamic>(responseData);
            if (json == null)
            {
                throw new JsonException("Failed to deserialize response JSON");
            }

            if (json.access_token == null)
            {
                throw new KeyNotFoundException("Access token not found in response JSON");
            }

            return json.access_token;
        }

        private async Task<string> SendAuthorizedRequestAsync(string url)
        {
            var token = await GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public Task<string> GetAudioFileAsync(string giid, string stepId)
        {
            var requestUrl = $"{TenantUrl}/recordings/audio?giid={giid}&stepid={stepId}";
            return SendAuthorizedRequestAsync(requestUrl);
        }

        public Task<string> GetMetadataAsync(string giid, string stepId)
        {
            var requestUrl = $"{TenantUrl}/recordings/metadata?giid={giid}&stepid={stepId}";
            return SendAuthorizedRequestAsync(requestUrl);
        }
    }

    public static class CloudContactApiExtensions
    {
        internal static async Task<string> SendRequestAsync(this CloudContactApiService api, string relativeUrl, string giid, string stepId)
        {
            var token = await api.GetAccessTokenAsync();

            var requestUrl = $"{api.TenantUrl}/{relativeUrl}?giid={giid}&stepid={stepId}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await api.HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
