﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{albumId}")]
        public async Task<IActionResult> GetById(int albumId)
        {
            var album = await _albumService.GetAlbumByIdAsync(albumId);
            if (album == null)
            {
                return response.NotFoundResponse();
            }
            return response.Success(album);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Album album)
        {
            await _albumService.AddAlbumAsync(album);
            return response.CreatedResponse(new { album.AlbumID, album });
        }

        [HttpPut("{albumId}")]
        public async Task<IActionResult> Update(int albumId, [FromBody] Album album)
        {
            if (albumId != album.AlbumID)
            {
                return response.BadRequest();
            }

            await _albumService.UpdateAlbumAsync(album);
            return response.NoContentResponse();
        }

        [HttpDelete("{albumId}")]
        public async Task<IActionResult> Delete(int albumId)
        {
            await _albumService.DeleteAlbumAsync(albumId);
            return response.NoContentResponse();
        }

        [HttpGet("{albumId}/details")]
        public async Task<IActionResult> GetAlbumDetails(int albumId)
        {
            var albumDetails = await _albumService.GetAlbumDetailsByIdAsync(albumId);
            if (albumDetails == null || albumDetails.Album == null)
            {
                return response.NotFoundResponse();
            }
            return response.Success(albumDetails);
        }
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentAlbums()
        {
            var recentAlbums = await _albumService.GetRecentAlbumsAsync();
            if (recentAlbums == null || recentAlbums.Count == 0)
            {
                return response.NotFoundResponse("No recent albums found.");
            }
            return response.Success(recentAlbums);
        }

    }
}
