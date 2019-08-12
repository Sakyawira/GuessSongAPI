using GuessAPI.Controllers;
using GuessAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace GuessAPIUnitTests
{
    class TranscriptionsControllerUnitTests
    {
        public static readonly DbContextOptions<guessContext> options
        = new DbContextOptionsBuilder<guessContext>()
    .UseInMemoryDatabase(databaseName: "testDatabase")
            .Options;

    }
}
