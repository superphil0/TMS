using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMS
{
    public class GameLineups
    {
        public GameLineups()
        {

            HomeLineup = new List<string>();
            AwayLineup = new List<string>();
        }
     
        public List<string> HomeLineup { get; set; }
        public List<string> AwayLineup { get; set; }
    }
}
