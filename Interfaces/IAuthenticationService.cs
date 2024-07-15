namespace CloudContactApi.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> GetAccessTokenAsync();

    }
}
