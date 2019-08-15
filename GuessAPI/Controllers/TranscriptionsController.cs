using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using GuessAPI.Model;

namespace GuessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscriptionsController : ControllerBase
    {
        private readonly GuessContext _context;

        public TranscriptionsController(GuessContext context)
        {
            _context = context;
        }

        // GET: api/Transcriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transcription>>> GetTranscription()
        {
            return await _context.Transcription.ToListAsync();
        }

        // GET api/Transcriptions/GetRandomTranscription
        [HttpGet("GetRandomTranscription")]
        public async Task<ActionResult<IEnumerable<Video>>> GetRandom()
        {
            var sizeOfList = _context.Transcription.ToListAsync().Result.Count;
            bool isget = false;

            // initialize transcription
            var transcription = await _context.Transcription.FindAsync(0);

            // only break after it finds a non-null transcription
            while (isget == false)
            {
                // randomize the id
                Random rnd = new Random();
                int id = _context.Transcription.ToListAsync().Result[rnd.Next(0, sizeOfList)].TranscriptionId;

                // find the transcription based on the generated id
                transcription = await _context.Transcription.FindAsync(id);

                if (transcription != null)
                {
                    isget = true;
                }
            }

            if (String.IsNullOrEmpty(transcription.Phrase))
            {
                return BadRequest("Search string cannot be null or empty.");
            }

            // Choose transcriptions that has the phrase 
            var videos = await _context.Video.Include(video => video.Transcription).Select(video => new Video
            {
                VideoId = video.VideoId,
                VideoTitle = video.VideoTitle,
                VideoLength = video.VideoLength,
                WebUrl = video.WebUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                IsFavourite = video.IsFavourite,
                Transcription = video.Transcription.Where(tran => tran.Phrase.Contains(transcription.Phrase)).ToList()
            }).ToListAsync();

            for (int i = 0; i < videos.Count; i++)
            {
                videos[i].IsFavourite = false;
            }

            // Removes all videos with empty transcription
            videos.RemoveAll(video => video.Transcription.Count == 0);
            int iCount = 0;

            if (videos.Count > 1)
            {
                iCount = videos.Count - 1;
                videos.RemoveRange(1, iCount);
            }
            //else if (videos.Count == 1)
            //{
            //    videos.RemoveRange(1, 1);
            //}
           
            //videos[0].IsFavourite = true;
            return Ok(videos);

        }

        // GET: api/Transcriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transcription>> GetTranscription(int id)
        {
            var transcription = await _context.Transcription.FindAsync(id);

            if (transcription == null)
            {
                return NotFound();
            }

            return transcription;
        }

        // PUT: api/Transcriptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTranscription(int id, Transcription transcription)
        {
            if (id != transcription.TranscriptionId)
            {
                return BadRequest();
            }

            _context.Entry(transcription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TranscriptionExists(id))
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

        // POST: api/Transcriptions
        [HttpPost]
        public async Task<ActionResult<Transcription>> PostTranscription(Transcription transcription)
        {
            _context.Transcription.Add(transcription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTranscription", new { id = transcription.TranscriptionId }, transcription);
        }

        // DELETE: api/Transcriptions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Transcription>> DeleteTranscription(int id)
        {
            var transcription = await _context.Transcription.FindAsync(id);
            if (transcription == null)
            {
                return NotFound();
            }

            _context.Transcription.Remove(transcription);
            await _context.SaveChangesAsync();

            return transcription;
        }

        private bool TranscriptionExists(int id)
        {
            return _context.Transcription.Any(e => e.TranscriptionId == id);
        }

      
    }

}
