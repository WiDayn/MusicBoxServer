using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Models;
using MusicBoxServer.Services;
using MusicBoxServer.Utils;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlbumController : ControllerBase
    {
        private readonly AlbumService _albumService;
        private readonly ApiResponseController response = new();

        public AlbumController(AlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            return response.Success(albums);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return response.NotFound();
            }
            return response.Success(album);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Album album)
        {
            await _albumService.AddAlbumAsync(album);
            return response.CreatedResponse(new { album.AlbumID, album });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Album album)
        {
            if (id != album.AlbumID)
            {
                return response.BadRequest();
            }

            await _albumService.UpdateAlbumAsync(album);
            return response.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _albumService.DeleteAlbumAsync(id);
            return response.NoContent();
        }

    }
}
