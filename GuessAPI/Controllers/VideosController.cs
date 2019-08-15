using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuessAPI.Model;
using GuessAPI.DAL;
using GuessAPI.Helper;

namespace GuessAPI.Controllers
{
    // DTO (Data Transfer object) inner class to help with Swagger documentation
    // Allow swagger ui to recognize the url to display it in the swagger ui
    public class Urldto
    {
        public String Url { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private IVideoRepository _videoRepository;
        private readonly IMapper _mapper;
        private readonly GuessContext _context;

        public VideosController(GuessContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            this._videoRepository = new VideoRepository(new GuessContext());
        }

        // GET: api/Videos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideo()
        {
            return await _context.Video.ToListAsync();
        }


        // GET: api/Videos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = await _context.Video.FindAsync(id);

            if (video == null)
            {
                return NotFound();
            }

            return video;
        }

        // PUT: api/Videos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, Video video)
        {
            if (id != video.VideoId)
            {
                return BadRequest();
            }

            _context.Entry(video).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
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


        //PUT with PATCH to handle isFavourite
        [HttpPatch("update/{id}")]
        public VideoDto Patch(int id, [FromBody]JsonPatchDocument<VideoDto> videoPatch)
        {
            //get original video object from the database
            Video originVideo = _videoRepository.GetVideoById(id);
            //use automapper to map that to DTO object
            VideoDto videoDto = _mapper.Map<VideoDto>(originVideo);
            //apply the patch to that DTO
            videoPatch.ApplyTo(videoDto);
            //use automapper to map the DTO back ontop of the database object
            _mapper.Map(videoDto, originVideo);
            //update video in the database
            _context.Update(originVideo);
            _context.SaveChanges();
            return videoDto;
        }

        // POST: api/Videos
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo([FromBody]Urldto data)
        {
            String videoUrl;
            String youtubeId;
            Video video;
            try
            {
                // Constructing the video object from our helper function
                videoUrl = data.Url;
                youtubeId = YouTubeHelper.GetYouTubeIdFromUrl(videoUrl);
                video = YouTubeHelper.GetVideoInfo(youtubeId);
            }
            catch
            {
                return BadRequest("Invalid YouTube URL");
            }

            // Determine if we can get transcriptions from YouTube
            if (!YouTubeHelper.CanGetTranscriptions(youtubeId))
            {
                return BadRequest("Subtitle does not exist on YouTube, failed to add video");
            }

            // Add this video object to the database
            _context.Video.Add(video);
            await _context.SaveChangesAsync();

           // video.VideoId = video.VideoId % 10;

            // Get the primary key of the newly created video record
            int id = video.VideoId;

            // This is needed because context are NOT thread safe, therefore we create another context for the following task.
            // We will be using this to insert transcriptions into the database on a seperate thread
            // So that it doesn't block the API.
            GuessContext tempContext = new GuessContext();
            TranscriptionsController transcriptionsController = new TranscriptionsController(tempContext);

            // This will be executed in the background.
            Task addCaptions = Task.Run(async () =>
            {
                List<Transcription> transcriptions = new List<Transcription>();
                transcriptions = YouTubeHelper.GetTranscriptions(youtubeId);

                for (int i = 0; i < transcriptions.Count; i++)
                {
                    // Get the transcription objects form transcriptions and assign VideoId to id, the primary key of the newly inserted video
                    Transcription transcription = transcriptions.ElementAt(i);
                    transcription.VideoId = id;
                    // Add this transcription to the database
                    await transcriptionsController.PostTranscription(transcription);
                }
            });

            // Return success code and the info on the video object
            return CreatedAtAction("GetVideo", new { id = video.VideoId }, video);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Video>> DeleteVideo(int id)
        {
            List<int> videoIDlist = new List<int>(new int[] { 1, 10, 11, 12, 15 });
            //if (videoIDlist.Contains(id))
            //{
            //    return BadRequest("The video cannot be deleted");
            //}
             //_context.Video.
            var video = await _context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            _context.Video.Remove(video);
            await _context.SaveChangesAsync();

            return video;
        }

        // GET api/Videos/SearchByTranscriptions/HelloWorld
        [HttpGet("SearchByTranscriptions/{searchString}")]
        public async Task<ActionResult<IEnumerable<Video>>> Search(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return BadRequest("Search string cannot be null or empty.");
            }

            // Choose transcriptions that has the phrase 
            var videos = await _context.Video.Include(video => video.Transcription).Select(video => new Video {
                VideoId = video.VideoId,
                VideoTitle = video.VideoTitle,
                VideoLength = video.VideoLength,
                WebUrl = video.WebUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                IsFavourite = video.IsFavourite,
                Transcription = video.Transcription.Where(tran => tran.Phrase.Contains(searchString)).ToList()
            }).ToListAsync();

            // Removes all videos with empty transcription
            videos.RemoveAll(video => video.Transcription.Count == 0);
            return Ok(videos);

        }

        // GET: api/Videos
        [HttpGet("GetRandomVideo")]
        public async Task<ActionResult<IEnumerable<Video>>> GetRandomVideo()
        {
            // Removes all videos with empty transcription

            var sizeOfList = _context.Video.ToListAsync().Result.Count;
            bool isGet = false;

            // initialize transcription
            var ivideo = await _context.Video.FindAsync(0);
            // Choose transcriptions that has the phrase 
            var videos = await _context.Video.Select(video => new Video
            {
                VideoId = video.VideoId,
                VideoTitle = video.VideoTitle,
                VideoLength = video.VideoLength,
                WebUrl = video.WebUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                IsFavourite = false,
                Transcription = video.Transcription
            }).ToListAsync();

            // only break after it finds a non-null transcription
            while (isGet == false)
            {
                // randomize the id
                Random rnd = new Random();

                // rnd.next's upper bound is EXCLUSIVE
                int rng = rnd.Next(0, sizeOfList);
                int id = _context.Video.ToListAsync().Result[rng].VideoId;
                int id2 = _context.Video.ToListAsync().Result[rng].VideoId;

                // if the video selected is not 0, set the previous video to the second video
                if (rng != 0)
                {
                    id2 = _context.Video.ToListAsync().Result[rng - 1].VideoId;
                }

                //  if the video selected is at index 0 but not the last video and there is more than one video,
                // set video 2 into the next video
                else if (rng != videos.Count - 1 && videos.Count > 1)
                {
                    id2 = _context.Video.ToListAsync().Result[rng + 1].VideoId;
                }
                // find the transcription based on the generated id
                ivideo = await _context.Video.FindAsync(id);

                if (ivideo != null)
                {
                    isGet = true;
                }

                videos.RemoveAll(video => video.Transcription.Count == 0);
                videos.RemoveAll(video => video.VideoId != id && video.VideoId != id2);
                //if (rng != 0 && rng != videos.Count)
                //{
                //    videos.RemoveRange(0, rng);
                //    videos.RemoveRange(rng + 2, );
                //}
            }
            return Ok(videos);
        }


        private bool VideoExists(int id)
        {
            return _context.Video.Any(e => e.VideoId == id);
        }
    }
}
