using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Dtos;
using MusicBoxServer.Models;
using MusicBoxServer.Services;
using MusicBoxServer.Utils;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly FavoriteService _favoriteService;

        private readonly ApiResponseController response = new();

        public FavoriteController(FavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetFavoriteSongs()
        {
            if (HttpContext.Items.TryGetValue("UserId", out var userIdRes))
            {
                var userIdAsString = userIdRes.ToString();
                int userId = int.Parse(userIdAsString);
                // UserId 存在，使用它
                var playlist = await _favoriteService.GetFavoritesByUserIdAsync(userId);
                // 检查列表是否为空
                if (playlist == null)
                {
                    // 返回空的 PlayList
                    return Ok(new List<SongInfo>());
                }

                return response.Success(playlist);
            }
            return response.Unauthorized();
        }

        [HttpGet("addSong/{songId}")]
        public async Task<IActionResult> AddFavoriteSong(int songId)
        {
            if (HttpContext.Items.TryGetValue("UserId", out var userIdRes))
            {
                var userIdAsString = userIdRes.ToString();
                int userId = int.Parse(userIdAsString);
                var favorite = new FavoriteRequest
                {
                    UserID = userId,
                    ID = songId,
                    DateFavorited = DateTime.UtcNow
                };

                await _favoriteService.AddFavoriteSongAsync(favorite);
                return response.Success("Add song successfully");
            }
            else
            {
                return response.UnauthorizedResponse("UserID is not available in the context.");
            }
        }

        [HttpGet("removeSong/{songId}")]
        public async Task<IActionResult> RemoveFavorite(int songId)
        {
            if (HttpContext.Items.TryGetValue("UserId", out var userIdRes))
            {
                var userIdAsString = userIdRes.ToString();
                int userId = int.Parse(userIdAsString);
                await _favoriteService.RemoveFavoriteSongAsync(userId, songId);
                return response.Success("Remove song successfully");
            }
            else
            {
                return Unauthorized("UserID is not available in the context.");
            }
        }
    }
}
