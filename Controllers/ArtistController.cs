using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Models;
using MusicBoxServer.Services;
using MusicBoxServer.Utils;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly ArtistService _artistService;
        private readonly ApiResponseController response = new();

        public ArtistController(ArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var artists = await _artistService.GetAllArtistsAsync();
            return response.Success(artists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);
            if (artist == null) return NotFound();
            return response.Success(artist);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Artist artist)
        {
            await _artistService.AddArtistAsync(artist);
            return response.CreatedResponse(new { artist.ArtistID, artist });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Artist artist)
        {
            if (id != artist.ArtistID) return response.BadRequest();
            await _artistService.UpdateArtistAsync(artist);
            return response.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _artistService.DeleteArtistAsync(id);
            return response.NoContent();
        }

        [HttpGet("{artistId}/details")]
        public async Task<IActionResult> GetArtistDetails(int artistId)
        {
            var artistDetails = await _artistService.GetArtistDetailsByIdAsync(artistId);
            if (artistDetails == null || artistDetails.Artist == null)
            {
                return NotFound();
            }
            return Ok(artistDetails);
        }
    }
}
