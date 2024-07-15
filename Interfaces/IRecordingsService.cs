namespace CloudContactApi.Interfaces
{
    public interface IRecordingsService
    {
        Task<string> GetAudioFileAsync(string giid, string stepId);
    }
}
