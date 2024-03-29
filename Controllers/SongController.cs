﻿using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Models;
using MusicBoxServer.Services;
using MusicBoxServer.Utils;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SongController : ControllerBase
    {
        private readonly SongService _songService;
        private readonly ApiResponseController response = new();
        public SongController(SongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _songService.GetAllSongsAsync();
            return response.Success(songs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return response.NotFound();
            }
            return response.Success(song);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Song song)
        {
            await _songService.AddSongAsync(song);
            return response.CreatedResponse(new { song.SongID, song });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Song song)
        {
            if (id != song.SongID)
            {
                return response.BadRequest();
            }

            await _songService.UpdateSongAsync(song);
            return response.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _songService.DeleteSongAsync(id);
            return response.NoContent();
        }

        [HttpGet("{songId}/lyrics")]
        public async Task<IActionResult> GetLyrics(int songId)
        {
            var lyrics = await _songService.GetLyricsAsync(songId);
            return response.Success(lyrics);
        }

        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> SearchSongs(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            var searchResults = await _songService.SearchSongsAsync(keyword);
            if (searchResults == null || !searchResults.Any())
            {
                return response.NotFoundResponse("No songs found.");
            }

            return response.Success(searchResults);
        }

    }
}
