using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMS
{
    public class Game
    {
        public decimal? ProgressMinute
        {
            get
            {
                try
                {
                    if (GameStatus == GameStatusEnum.LIVE)
                    {
                        if (Progress.Equals("HT")) return 45.5M;
                        else
                        {
                            string progress = Progress;
                            if (progress.Contains("+"))
                            {
                                progress = Progress.Substring(0, Progress.IndexOf("+"));
                            }
                            return decimal.Parse(progress.Replace("'", ""));
                        }
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    Logger.Exception(e);
                    return null;
                }
            }
        }

        public string Progress { get; set; }
        public DateTime? Time { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }
        public string LineupUrl { get; set; }
        public string Result { get; set; }

        public enum GameStatusEnum { FINISHED, LIVE, UPCOMING }

        public GameStatusEnum GameStatus
        {
            get
            {
                if (Progress.Contains("'") || Progress.Equals("HT"))
                {
                    return Game.GameStatusEnum.LIVE;
                }
                else if (Progress.Contains(":"))
                {
                    return GameStatusEnum.UPCOMING;
                }
                else
                    return GameStatusEnum.FINISHED;
            }
        }

        public DateTime LastChange { get; set; }

        public override string ToString()
        {
            return Progress.PadRight(8) + " " + Result.PadRight(8) + " " + Home + " - " + Away;
        }

        public bool HasLineup { get; set; }
    }
}
