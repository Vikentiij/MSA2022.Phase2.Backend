using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        CatTagDb _context;

        private readonly ILogger<CatsController> _logger;

        public CatsController(ILogger<CatsController> logger, IHttpClientFactory clientFactory, CatTagDb context)
        {
            _logger = logger;

            _context = context;

            if (clientFactory is null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("cataas");
        }

        /// <summary>
        /// Returns all saved cat tags
        /// </summary>
        /// <returns>A JSON object with a list of all saved cat tags</returns>
        [HttpGet]
        [Route("tags")]
        [ProducesResponseType(200)]
        public async Task<IEnumerable<string>> GetTagsAsync()
        {
            var tagsList = new List<string>();
            var savedTags = await _context.CatTags.ToListAsync();

            if (savedTags != null)
                foreach (var tag in savedTags)
                    tagsList.Add(tag.Tag);

            return tagsList;
        }


        /// <summary>
        /// Returns a random cat picture by a saved tag
        /// </summary>
        /// <param name="tag">The tag for the cat picture to return (required)</param>
        /// <returns>A JSON object with cat picture URL</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPicture(string tag)
        {
            if (tag == "") return BadRequest("Tag cannot be empty");

            if (!_context.CatTags.Where(t => t.Tag == tag.ToLower()).Any())
                return NotFound($"Tag {tag} not found, create it first");

            var response = await _client.GetAsync($"/cat/{tag}?json=true");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound("Nothing found by this tag");

            var cataasApiResponse = await response.Content.ReadFromJsonAsync<CataasApiResponse>();
            if (cataasApiResponse == null) return NotFound("API didn't return any results");

            return Ok(new CatPicture() { id = cataasApiResponse.id, url = $"https://cataas.com{cataasApiResponse.url}", tags = cataasApiResponse.tags });
        }

        /// <summary>
        /// Save cat tag
        /// </summary>
        /// <returns>A 201 Created response</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SaveTag(string tag)
        {
            if (tag == "") return BadRequest("Tag cannot be empty");

            if (_context.CatTags.Where(t => t.Tag == tag.ToLower()).Any())
                return BadRequest($"Tag {tag} already exists");

            // Fetching the list of all tags that exist in cataas
            var response = await _client.GetAsync("/api/tags");
            var allTags = await response.Content.ReadFromJsonAsync<List<string>>();

            if (allTags == null || allTags.Count == 0) return Problem($"Unable to fetch tags from {_client.BaseAddress}");

            if (!allTags.Contains(tag))
            {
                var exampleValidTags = new List<string>();
                var random = new Random();
                for (int i = 0; i < 3; i++)
                {
                    exampleValidTags.Add(allTags[random.Next(allTags.Count)]);
                }
                return BadRequest($"We won't be able to return any cat pictures for this tag. Please use another tag, such as {string.Join(", ", exampleValidTags)}");
            }

            // Save tag to DB
            await _context.AddAsync(new CatTag() { Tag = tag.ToLower() });
            await _context.SaveChangesAsync();
            return Created($"/Cats?tag={tag}", tag);
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

            var tagToReplace = _context.CatTags.Where(t => t.Tag == oldTag.ToLower()).FirstOrDefault();
            if (tagToReplace == null)
                return NotFound($"Cannot update tag {oldTag} because it is not found");
            if (_context.CatTags.Where(t => t.Tag == newTag.ToLower()).Any())
                return BadRequest($"Tag {newTag} already exists");

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
                return BadRequest($"We won't be able to return any cat pictures for this tag. Please use another tag, such as {string.Join(", ", exampleValidTags)}");
            }

            // Eepace oldTag with newTag
            tagToReplace.Tag = newTag;
            await _context.SaveChangesAsync();

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

            var tagToDelete = _context.CatTags.Where(t => t.Tag == tag.ToLower()).FirstOrDefault();
            if (tagToDelete == null)
                return NotFound($"Tag {tag} not found");

            _context.CatTags.Remove(tagToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}