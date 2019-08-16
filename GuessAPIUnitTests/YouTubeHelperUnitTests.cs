using System;
using GuessAPI.Controllers;
using GuessAPI.Model;
using GuessAPI.Helper;
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
    public class YouTubeHelperUnitTests
    {
        public static readonly DbContextOptions<GuessContext> Options
       = new DbContextOptionsBuilder<GuessContext>().UseInMemoryDatabase(databaseName: "testDatabase").Options;

        [TestMethod]
        public void TestGetVideoInfo()
        {
            var video = new Video();
            // YouTubeHelper =
            Video result =
                YouTubeHelper.GetVideoInfo(
                    YouTubeHelper.GetYouTubeIdFromUrl("https://www.youtube.com/watch?v=uIAScvNDQpI"));

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, video.GetType());
        }

        [TestMethod]
        public void TestGetTranscriptions()
        {
            var transcription = new Transcription();
            var transcriptionsList = new List<Transcription>();
            // YouTubeHelper =
            List<Transcription> result =
                YouTubeHelper.GetTranscriptions(
                    YouTubeHelper.GetYouTubeIdFromUrl("https://www.youtube.com/watch?v=uIAScvNDQpI"));

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, transcriptionsList.GetType());
            Assert.IsInstanceOfType(result[0], transcription.GetType());
        }

    }
}
