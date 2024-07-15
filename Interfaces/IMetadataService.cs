namespace CloudContactApi.Interfaces
{
    public interface IMetadataService
    {
        Task<string> GetMetadataAsync(string giid, string stepId);

    }
}
