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
    public class LeaderBoardsControllerUnitTests
    {
        public static readonly DbContextOptions<guessContext> options
       = new DbContextOptionsBuilder<guessContext>().UseInMemoryDatabase(databaseName: "testDatabase").Options;

        public static readonly IList<LeaderBoard> leaderBoards = new List<LeaderBoard>
        {
             new LeaderBoard()
            {
                    PlayerName =  "Mina",
                    Score = 100,
            },

             new LeaderBoard()
             {
                 PlayerName = "Sakya",
                 Score = 200,
             }
        };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new guessContext(options))
            {
                // populate the db
                context.LeaderBoard.Add(leaderBoards[0]);
                context.LeaderBoard.Add(leaderBoards[1]);

                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new guessContext(options))
            {
                // clear the db
                context.LeaderBoard.RemoveRange(context.LeaderBoard);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestGetSuccessfully()
        {
            using (var context = new guessContext(options))
            {
                // make a new transcription controller
                LeaderBoardsController leaderBoardsController = new LeaderBoardsController(context);

                // get the result
                ActionResult<IEnumerable<LeaderBoard>> result = await leaderBoardsController.GetLeaderBoard();

                // see if the result is null
                Assert.IsNotNull(result);

            }
        }
    }
}
