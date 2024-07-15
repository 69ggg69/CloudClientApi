using CloudContactApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CloudContactApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordingsController : ControllerBase
    {
        private readonly IRecordingsService _recordingsService;
        private readonly IMetadataService _metadataService;

        public RecordingsController(IRecordingsService recordingsService, IMetadataService metadataService)
        {
            _recordingsService = recordingsService;
            _metadataService = metadataService;
        }

        [HttpGet("audio")]
        public async Task<IActionResult> GetAudioFile(string giid, string stepid)
        {
            try
            {
                var audioFile = await _recordingsService.GetAudioFileAsync(giid, stepid);
                return Ok(audioFile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadata(string giid, string stepid)
        {
            try
            {
                var metadata = await _metadataService.GetMetadataAsync(giid, stepid);
                return Ok(metadata);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
