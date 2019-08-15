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
using AutoMapper;

namespace GuessAPIUnitTests
{
    [TestClass]
    public class VideosControllerUnitTests
    {
        public static readonly DbContextOptions<guessContext> options
       = new DbContextOptionsBuilder<guessContext>().UseInMemoryDatabase(databaseName: "testDatabase").Options;

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

        private readonly IMapper _mapper;

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new guessContext(options))
            {
               // populate videos db
                context.Video.Add(videos[0]);
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
                context.Video.RemoveRange(context.Video);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestGetRandomTranscription()
        {

            using (var context = new guessContext(options))
            {
                // make a new video controller
                VideosController videosController = new VideosController(context, _mapper);

                // get the result of GetRandomVideo method
                ActionResult<IEnumerable<Video>> result = await videosController.GetRandomVideo();

                // see if the result is null
                Assert.IsNotNull(result);

            }
        }

    }
}
