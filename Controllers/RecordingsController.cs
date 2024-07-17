using Microsoft.AspNetCore.Mvc;

namespace CloudContactApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CloudContactController : ControllerBase
    {
        private readonly CloudContactApiService _cloudContactApiService;

        public CloudContactController(CloudContactApiService cloudContactApiService)
        {
            _cloudContactApiService = cloudContactApiService;
        }

        [HttpGet("audio")]
        public async Task<IActionResult> GetAudioFileAsync(string giid, string stepId)
        {
            try
            {
                var audioFile = await _cloudContactApiService.GetAudioFileAsync(giid, stepId);
                return Ok(audioFile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadataAsync(string giid, string stepId)
        {
            try
            {
                var metadata = await _cloudContactApiService.GetMetadataAsync(giid, stepId);
                return Ok(metadata);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("sendrequest")]
        public async Task<IActionResult> SendRequestAsync(string relativeUrl, string giid, string stepId)
        {
            try
            {
                var result = await _cloudContactApiService.SendRequestAsync(relativeUrl, giid, stepId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}