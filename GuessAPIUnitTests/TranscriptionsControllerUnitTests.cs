using GuessAPI.Controllers;
using GuessAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace GuessAPIUnitTests
{
    [TestClass]
    public class TranscriptionsControllerUnitTests
    {
        public static readonly DbContextOptions<guessContext> options
        = new DbContextOptionsBuilder<guessContext>().UseInMemoryDatabase(databaseName: "testDatabase").Options;


        public static readonly IList<Transcription> transcriptions = new List<Transcription>
        {
            new Transcription()
            {
                Phrase = "Jieun, let me tell you I just turned 30"
            },
            new Transcription()
            {
                Phrase = "Please keep the la la la line"
            },

            
        };
        public static readonly IList<Video> videos = new List<Video>
        {
             new Video()
            {
                    VideoId =  101,
                    VideoTitle = "[MV] IU(???) _ BBIBBI(??)",
                    VideoLength = 209,
                    WebUrl = "https://www.youtube.com/watch?v=nM0xDI5R50E",
                    ThumbnailUrl = "https://i.ytimg.com/vi/rRzxEiBLQCA/mqdefault.jpg",
                    IsFavourite =  false,
                    Transcription = null
            }
        };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new guessContext(options))
            {
                // populate the db
                context.Transcription.Add(transcriptions[0]);
                context.Transcription.Add(transcriptions[1]);

                // populate videos db
                //context.Video.Add(videos[0]);
                //context.Video.Add(videos[1]);

                context.SaveChanges();
            }
        }



        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new guessContext(options))
            {
                // clear the db
                context.Transcription.RemoveRange(context.Transcription);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestGetRandomTranscription()
        {
            using (var context = new guessContext(options))
            {
                // make a new transcription controller
                TranscriptionsController transcriptionsController = new TranscriptionsController(context);

                // get the result
                ActionResult<IEnumerable<Video>> result = await transcriptionsController.GetRandom();

                // see if the result is null
                Assert.IsNotNull(result);

            }
        }

        [TestMethod]
        public async Task TestGetSuccessfully()
        {
            using (var context = new guessContext(options))
            {
                // make a new transcription controller
                TranscriptionsController transcriptionsController = new TranscriptionsController(context);

                // get the result
                ActionResult<IEnumerable<Transcription>> result = await transcriptionsController.GetTranscription();

                // see if the result is null
                Assert.IsNotNull(result);
             
            }
        }

        [TestMethod]
        public async Task TestPutTranscriptionNoContentStatusCode()
        {
            using (var context = new guessContext(options))
            {
                string newPhrase = "this is now a different phrase";
                Transcription transcription1 = context.Transcription.Where(x => x.Phrase == transcriptions[0].Phrase).Single();
                transcription1.Phrase = newPhrase;

                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                IActionResult result = await transcriptionsController.PutTranscription(transcription1.TranscriptionId, transcription1) as IActionResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }

    }
}
