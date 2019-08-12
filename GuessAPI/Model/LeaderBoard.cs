using System;
using System.Collections.Generic;

namespace GuessAPI.Model
{
    public partial class LeaderBoard
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }
}
