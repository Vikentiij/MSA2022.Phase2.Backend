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
        /// <param name="tag">The tag for the cat picture to return (required)</param>
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

        /// <summary>
        /// Save cat tag
        /// </summary>
        /// <returns>A 202 Accepted response</returns>
        [HttpPost]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SaveTag(string tag)
        {
            if (tag == "") return BadRequest("Tag cannot be empty");

            // TODO: check if tag already exists
            // if tag found return BadRequest($"Tag {tag} already exists");

            // Fetching the list of all tags that exist in cataas
            var response = await _client.GetAsync("/api/tags");
            var allTags = await response.Content.ReadFromJsonAsync<List<string>>();

            if (allTags == null || allTags.Count == 0) return Problem($"Unable to fetch tags from {_client.BaseAddress}");

            if (!allTags.Contains(tag)) {
                var exampleValidTags = new List<string>();
                var random = new Random();
                for (int i = 0; i < 3; i++)
                {
                    exampleValidTags.Add(allTags[random.Next(allTags.Count)]);
                }
                return BadRequest($"There are no cat pictures for this tag. Try such tags as {string.Join(", ", exampleValidTags)}");
            }

            return Accepted();
        }

        /// <summary>
        /// Update cat tag
        /// </summary>
        /// <returns>A 202 Accepted response</returns>
        [HttpPut]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTag(string oldTag, string newTag)
        {
            if (oldTag == "" || newTag == "") return BadRequest("Tag cannot be empty");

            // TODO: check if oldTag and newTag exists
            // if oldTag not found return NotFound($"Cannot update tag {oldTag} because it is not found");
            // if newTag found return BadRequest($"Tag {newTag} already exists");

            // Fetching the list of all tags that exist in cataas
            var response = await _client.GetAsync("/api/tags");
            var allTags = await response.Content.ReadFromJsonAsync<List<string>>();

            if (allTags == null || allTags.Count == 0) return Problem($"Unable to fetch tags from {_client.BaseAddress}");

            if (!allTags.Contains(newTag))
            {
                var exampleValidTags = new List<string>();
                var random = new Random();
                for (int i = 0; i < 3; i++)
                {
                    exampleValidTags.Add(allTags[random.Next(allTags.Count)]);
                }
                return BadRequest($"There are no cat pictures for the new tag. Try such tags as {string.Join(", ", exampleValidTags)}");
            }

            // TODO: repace oldTag with newTag 

            return Accepted();
        }

        /// <summary>
        /// Delete cat tag
        /// </summary>
        /// <returns>A 204 No Content Response</returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTag(string tag)
        {
            if (tag == "") return BadRequest("Tag cannot be empty");

            // TODO: check if tag does not exist
            // if tag not found return NotFound($"Tag {tag} not found");

            // TODO: delete tag

            return NoContent();
        }
    }
}