using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Models;
using MusicBoxServer.Services;
using MusicBoxServer.Utils;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayListController : ControllerBase
    {
        private readonly PlayListService _playlistService;
        private readonly ApiResponseController response = new();

        public PlayListController(PlayListService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var playlists = await _playlistService.GetAllPlayListsAsync();
            return response.Success(playlists);
        }

        [HttpGet("{playlistId}")]
        public async Task<IActionResult> GetById(int playlistId)
        {
            var playlist = await _playlistService.GetPlayListByIdAsync(playlistId);
            if (playlist == null)
            {
                return response.NotFound();
            }
            return response.Success(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlayList playlist)
        {
            await _playlistService.AddPlayListAsync(playlist);
            return response.CreatedResponse(new { playlist.PlayListID, playlist });
        }

        [HttpPut("{playlistId}")]
        public async Task<IActionResult> Update(int playlistId, [FromBody] PlayList playlist)
        {
            if (playlistId != playlist.PlayListID)
            {
                return response.BadRequest();
            }

            await _playlistService.UpdatePlayListAsync(playlist);
            return response.NoContent();
        }

        [HttpDelete("{playlistId}")]
        public async Task<IActionResult> Delete(int playlistId)
        {
            await _playlistService.DeletePlayListAsync(playlistId);
            return response.NoContent();
        }

        [HttpGet("{playlistId}/detail")]
        public async Task<IActionResult> GetPlayListDetails(int playlistId)
        {
            var playlistDetails = await _playlistService.GetPlayListDetailsByIdAsync(playlistId);
            if (playlistDetails == null || playlistDetails.PlayList == null)
            {
                return response.NotFound();
            }
            return response.Success(playlistDetails);
        }
    }

}
