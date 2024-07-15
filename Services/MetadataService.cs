using CloudContactApi.Interfaces;

public class MetadataService : IMetadataService
{
    private readonly HttpClient _httpClient;
    private readonly string _tenantUrl;
    private readonly IAuthenticationService _authenticationService;

    public MetadataService(HttpClient httpClient, string tenantUrl, IAuthenticationService authenticationService)
    {
        _httpClient = httpClient;
        _tenantUrl = tenantUrl;
        _authenticationService = authenticationService;
    }

    public async Task<string> GetMetadataAsync(string giid, string stepId)
    {

        var token = await _authenticationService.GetAccessTokenAsync();

        var requestUrl = $"{_tenantUrl}/configapi/v2/recordings/metadata?giid={giid}&stepid={stepId}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
