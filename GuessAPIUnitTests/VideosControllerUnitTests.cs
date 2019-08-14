﻿using GuessAPI.Controllers;
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

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new guessContext(options))
            {
               // populate videos db
                context.Video.Add(videos[0]);
                context.Video.Add(videos[1]);

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

    }
}
