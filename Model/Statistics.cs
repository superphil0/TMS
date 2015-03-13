using System;
using System.Collections.Generic;
namespace TMS
{
	public class Statistics
	{
		public string Year
		{
			get;
			set;
		}
		public string GamesPlayed
		{
			get;
			set;
		}
		public string GoalsScored
		{
			get;
			set;
		}
		public string MinutesPlayed
		{
			get;
			set;
		}
		public string MinutesPerGoal
		{
			get;
			set;
		}
		public string GoalsScoredTopLeague
		{
			get;
			set;
		}
		public string MinutesPerGoalTopLeague
		{
			get;
			set;
		}
		public string Assists
		{
			get;
			set;
		}

        public List<Match> Matches
        {
            get;
            set;
        }

        public string Contract
        {
            get;
            set;
        }
	}
}
