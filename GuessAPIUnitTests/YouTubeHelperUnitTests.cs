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
    class YouTubeHelperUnitTests
    {
        public static readonly DbContextOptions<GuessContext> Options
       = new DbContextOptionsBuilder<GuessContext>().UseInMemoryDatabase(databaseName: "testDatabase").Options;


    }
}
