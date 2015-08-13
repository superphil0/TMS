using System;
using System.Collections.Generic;
namespace TMS
{
    public class Player
    {
      public enum LineUpStatus
      {
        YES,
        NO,
        MAYBE
      };

        
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
        public string PlayerStatsUrl
        {
            get;
            set;
        }
        public string PlayerNr
        {
            get;
            set;
        }
        public int PlayerId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string NameInNativeCountry
        {
            get;
            set;
        }
        public string Age
        {
            get;
            set;
        }
        public string MainPosition
        {
            get;
            set;
        }
        public DateTime? ContractFrom
        {
            get;
            set;
        }
        public DateTime? ContractTo
        {
            get;
            set;
        }
        public string MarketValue
        {
            get;
            set;
        }
        public List<Statistics> Statistics
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
        public string Injury
        {
            get;
            set;
        }
        public string Number
        {
            get;
            set;
        }
        public string PrefferedFoot
        {
            get;
            set;
        }
        public List<string> SecondaryPositions
        {
            get;
            set;
        }

        public string NationalPlayer
        {
            get;
            set;
        }
        public string NationalPlayerUrl
        {
            get;
            set;
        }

        public LineUpStatus Lineup
        {
            get;
            set;
        }
    }
}
