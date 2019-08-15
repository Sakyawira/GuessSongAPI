using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuessAPI.Model
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardsController : ControllerBase
    {
        private readonly GuessContext _context;

        public LeaderBoardsController(GuessContext context)
        {
            _context = context;
        }

        // GET: api/LeaderBoards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaderBoard>>> GetLeaderBoard()
        {
            return await _context.LeaderBoard.ToListAsync();
        }

        // GET: api/LeaderBoards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaderBoard>> GetLeaderBoard(int id)
        {
            var leaderBoard = await _context.LeaderBoard.FindAsync(id);

            if (leaderBoard == null)
            {
                return NotFound();
            }

            return leaderBoard;
        }

        // PUT: api/LeaderBoards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaderBoard(int id, LeaderBoard leaderBoard)
        {
            if (id != leaderBoard.PlayerId)
            {
                return BadRequest();
            }

            _context.Entry(leaderBoard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaderBoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LeaderBoards
        [HttpPost]
        public async Task<ActionResult<LeaderBoard>> PostLeaderBoard(LeaderBoard leaderBoard)
        {
            _context.LeaderBoard.Add(leaderBoard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaderBoard", new { id = leaderBoard.PlayerId }, leaderBoard);
        }

        // DELETE: api/LeaderBoards/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LeaderBoard>> DeleteLeaderBoard(int id)
        {
            var leaderBoard = await _context.LeaderBoard.FindAsync(id);
            if (leaderBoard == null)
            {
                return NotFound();
            }

            _context.LeaderBoard.Remove(leaderBoard);
            await _context.SaveChangesAsync();

            return leaderBoard;
        }

        private bool LeaderBoardExists(int id)
        {
            return _context.LeaderBoard.Any(e => e.PlayerId == id);
        }
    }
}
