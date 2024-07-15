using CloudContactApi.Interfaces;

public class RecordingsService : IRecordingsService
{
    private readonly HttpClient _httpClient;
    private readonly string _tenantUrl;
    private readonly IAuthenticationService _authenticationService;

    public RecordingsService(HttpClient httpClient, string tenantUrl, IAuthenticationService authenticationService)
    {
        _httpClient = httpClient;
        _tenantUrl = tenantUrl;
        _authenticationService = authenticationService;
    }

    public async Task<string> GetAudioFileAsync(string giid, string stepId)
    {

        var token = await _authenticationService.GetAccessTokenAsync();

        var requestUrl = $"{_tenantUrl}/configapi/v2/recordings/audio?giid={giid}&stepid={stepId}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

}
