using System;
using System.Collections.Generic;
namespace TMS
{
    public class Team
    {
        public Team()
        {
            Schedule = new List<Match>();
        }
        public int TeamId
        {
            get;
            set;
        }
        public string TmId
        {
            get;
            set;
        }
        public string TeamName
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public string CompetitionId
        {
            get;
            set;
        }
        public string CompetitionName
        {
            get;
            set;
        }

        public string TeamFullName
        {
            get { return this.TeamName + " (" + this.CompetitionName + ")"; }

        }

        public List<Player> Players
        {
            get;
            set;
        }
        public List<Player> PlayersWithUrls
        {
            get;
            set;
        }

        public List<string> GameLineups
        {
            get;
            set;
        }

        public string AlternativeName
        {
            get;
            set;
        }

        public string UrlName { get; set; }

        public List<Match> Schedule { get; set; }

        public string Tag { get; set; }
    }
}
