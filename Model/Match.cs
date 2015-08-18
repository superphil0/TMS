using System;
namespace TMS
{
	public class Match
	{
		public string TmId
		{
			get;
			set;
		}
		public string TmUrl
		{
			get;
			set;
		}
		public DateTime? Date
		{
			get;
			set;
		}
		public string HomeTeamName
		{
			get;
			set;
		}
		public string VisitingTeamName
		{
			get;
			set;
		}

    public Team HomeTeam
    {
      get;
      set;
    }
    public Team VisitingTeam
    {
      get;
      set;
    }

    public string Result
		{
			get;
			set;
		}
		public string Position
		{
			get;
			set;
		}
		public string Goals
		{
			get;
			set;
		}
		public string Assists
		{
			get;
			set;
		}
		public string OwnGoals
		{
			get;
			set;
		}
		public string YellowCards
		{
			get;
			set;
		}
		public string YellowRedCards
		{
			get;
			set;
		}
		public string RedCards
		{
			get;
			set;
		}
		public string SubstitutedOn
		{
			get;
			set;
		}
		public string SubstitutedOff
		{
			get;
			set;
		}
		public string MinutesPlayed
		{
			get;
			set;
		}
		public string Description
		{
			get;
			set;
		}
	}
}
