using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Models;
using MusicBoxServer.Services;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConcertController : ControllerBase
    {
        private readonly ConcertService _concertService;

        public ConcertController(ConcertService concertService)
        {
            _concertService = concertService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var concerts = await _concertService.GetAllConcertsAsync();
            return Ok(concerts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var concert = await _concertService.GetConcertByIdAsync(id);
            if (concert == null)
            {
                return NotFound();
            }
            return Ok(concert);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Concert concert)
        {
            await _concertService.AddConcertAsync(concert);
            return CreatedAtAction(nameof(GetById), new { id = concert.ConcertID }, concert);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Concert concert)
        {
            if (id != concert.ConcertID)
            {
                return BadRequest();
            }

            await _concertService.UpdateConcertAsync(concert);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _concertService.DeleteConcertAsync(id);
            return NoContent();
        }
    }

}
