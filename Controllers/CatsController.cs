using Microsoft.AspNetCore.Mvc;
using MSA2022.Phase2.Backend.Models;

namespace MSA2022.Phase2.Backend.Controllers
{
    /// <summary>
    /// This is the cats controller that allows to save cat tags and to receive cat images foir the saved tags
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly HttpClient _client;

        private readonly ILogger<CatsController> _logger;

        public CatsController(ILogger<CatsController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;

            if (clientFactory is null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("cataas");
        }

        /// <summary>
        /// Returns all saved cat tags
        /// </summary>
        /// <returns>A JSON object with a list of cat tags</returns>
        [HttpGet]
        [Route("tags")]
        [ProducesResponseType(200)]
        public async Task<IEnumerable<CatTag>> GetTagsAsync()
        {
            var response = await _client.GetAsync("/api/tags");
            var allTags = await response.Content.ReadFromJsonAsync<string[]>();

            var savedTags = new List<CatTag>();

            if (allTags != null)
                foreach (var tag in allTags)
                {
                    if (tag != "") savedTags.Add(new CatTag { Tag = tag });
                }

            return savedTags;
        }


        /// <summary>
        /// Returns a random cat picture by tag
        /// </summary>
        /// <returns>A JSON object with cat picture URL</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPicture(string tag)
        {
            if (tag == "") return BadRequest("Tag cannot be empty");

            var response = await _client.GetAsync($"/cat/{tag}?json=true");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound("Nothing found by this tag");

            var catPicture = await response.Content.ReadFromJsonAsync<CatPicture>();
            return Ok(catPicture);
        }
    }
}