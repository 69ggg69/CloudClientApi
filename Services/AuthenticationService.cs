using CloudContactApi.Interfaces;
using Newtonsoft.Json;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly string _tenantUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _scope;
    private readonly string _grantType;

    public AuthenticationService(HttpClient httpClient, string tenantUrl, string clientId, string clientSecret, string scope, string grantType)
    {
        _httpClient = httpClient;
        _tenantUrl = tenantUrl;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _scope = scope;
        _grantType = grantType;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_tenantUrl}/configapi/v2/oauth/token");
        var collection = new List<KeyValuePair<string, string>>
        {
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("scope", _scope),
            new("grant_type", _grantType)
        };
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Authentication failed with status code {response.StatusCode}");
        }

        var responseData = await response.Content.ReadAsStringAsync();
        return ExtractTokenFromResponse(responseData);
    }

    private string ExtractTokenFromResponse(string responseData)
    {
        if (string.IsNullOrWhiteSpace(responseData))
        {
            throw new Exception("Response data is null or empty");
        }

        var json = JsonConvert.DeserializeObject<dynamic>(responseData);
        if (json == null)
        {
            throw new Exception("Failed to deserialize response JSON");
        }

        if (json.access_token == null)
        {
            throw new Exception("Access token not found in response JSON");
        }

        return json.access_token;
    }
}
