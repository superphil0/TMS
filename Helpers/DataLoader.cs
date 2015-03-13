using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TMS
{
    public class DataLoader
    {
        public async static Task<List<Game>> LoadGamesWithAnnouncedLineUpAsync(DateTime date)
        {

            string baseUrl = "http://www.livescore.com";
            List<Game> games = new List<Game>();
            try
            {

                string dat = date.ToString("yyyy-MM-dd");
                string url = baseUrl + "/soccer/" + dat + "/";

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);

                //var htmlDocument = htmlWeb.Load(url);

                var contentNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='content']");
                var matchesNodeCollection = contentNode.SelectNodes("//div[contains(@class,'row-gray')]");
                foreach (var matchNode in matchesNodeCollection)
                {
                    string s = matchNode.InnerText;
                    //if (s.Contains("? - ?"))
                    //{
                    var links = matchNode.SelectNodes("div/a");
                    if (links != null)
                    {
                        string lineupUrl = links.ElementAt(0).Attributes["href"].Value;
                        var nodeColl = matchNode.SelectNodes("div");
                        string time = nodeColl.ElementAt(0).InnerText.Trim();
                        time = time.Replace("&#x27;", "'");
                        int hour, min;
                        DateTime? dt = null;
                        if (s.Contains("? - ?"))
                        {
                            try
                            {
                                hour = int.Parse(time.Substring(0, 2));
                                min = int.Parse(time.Substring(3, 2));
                                dt = new DateTime(date.Year, date.Month, date.Day, hour, min, 0);
                                dt = dt.Value.AddHours(1);
                                time = dt.Value.ToString("HH:mm");
                            }
                            catch (Exception e)
                            {
                                Logger.Exception(e);
                            }
                        }
                        string home = WebUtility.HtmlDecode(nodeColl.ElementAt(1).InnerText.Trim());
                        string away = WebUtility.HtmlDecode(nodeColl.ElementAt(3).InnerText.Trim());
                        string result = nodeColl.ElementAt(2).InnerText.Trim();

                        Game g = new Game()
                        {
                            Time = dt,
                            Home = home,
                            Away = away,
                            LineupUrl = baseUrl + lineupUrl,
                            Progress = time,
                            Result = result.Trim()
                        };
                        games.Add(g);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }

            return games;
        }

        public async static Task<GameLineups> LoadLineupsAsync(string url)
        {
            GameLineups gl;
            try
            {
                gl = new GameLineups();
                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);

                var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@data-type='substitutions']");
                if (nodes != null)
                {
                    var node = nodes.ElementAt(0);
                    var rows = node.SelectNodes("div[contains(@class,'row-gray')]");
                    foreach (var row in rows)
                    {
                        var tc = row.SelectNodes("div");

                        if (tc.ElementAt(1).InnerText.Contains("[") == false && gl.HomeLineup.Count < 11 && gl.AwayLineup.Count < 11)
                        {

                            if (tc.Count >= 2)
                                gl.HomeLineup.Add(WebUtility.HtmlDecode(tc.ElementAt(1).InnerText.Trim()));

                            if (tc.Count >= 4)
                                gl.AwayLineup.Add(WebUtility.HtmlDecode(tc.ElementAt(3).InnerText.Trim()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                return null;
            }
            return gl;
        }

        public async static Task<List<Country>> LoadCountriesAsync()
        {
            List<Country> result;
            try
            {
                List<Country> list = new List<Country>();

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com");
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);
                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//select[@name='land_select_breadcrumb']");
                if (htmlNodeCollection != null)
                {
                    string innerHtml = htmlNodeCollection.ElementAt(0).InnerHtml;
                    string[] array = Regex.Split(innerHtml, "\\n");
                    for (int i = 2; i < array.Length - 1; i++)
                    {
                        string[] array2 = Regex.Split(array[i], "\"");
                        list.Add(new Country
                        {
                            CountryId = int.Parse(array2[1]),
                            CountryName = array2[2].Substring(1)
                        });
                    }
                }
                result = list;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                result = null;
            }
            return result;
        }

        public async static Task<List<Competition>> LoadCompetitionsAsync(int countryid)
        {
            List<Competition> result;
            try
            {
                List<Competition> list = new List<Competition>();

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/wettbewerbe/national/wettbewerbe/" + countryid);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);

                var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//select[@name='wettbewerb_select_breadcrumb']");

                if (htmlNodeCollection != null)
                {
                    string innerHtml = htmlNodeCollection.ElementAt(0).InnerHtml;
                    string[] array = Regex.Split(innerHtml, "\\n");
                    for (int i = 2; i < array.Length - 1; i++)
                    {
                        string[] array2 = Regex.Split(array[i], "\"");
                        list.Add(new Competition
                        {
                            CompetitionCountryId = countryid,
                            CompetitionId = array2[1],
                            CompetitionName = array2[2].Substring(1)
                        });
                    }
                }

                //NATIONAL TEAMS

                htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'wettbewerbe-auflistung')]/li/a");

                if (htmlNodeCollection != null)
                {
                    Competition national = new Competition()
                    {
                        CompetitionCountryId = countryid,
                        CompetitionId = "NAT_" + countryid,
                        CompetitionName = "NATIONAL"
                    };
                    foreach (HtmlNode node in htmlNodeCollection)
                    {
                        Team t = new Team();
                        t.TeamName = node.GetAttributeValue("title", "");
                        t.Url = node.GetAttributeValue("href", "");
                        t.TeamId = int.Parse(t.Url.Split('/').Last());
                        t.CompetitionId = national.CompetitionId;
                        t.CompetitionName = national.CompetitionName;
                        national.Teams.Add(t);
                    }


                    list.Insert(0, national);
                }

                result = list;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                result = null;
            }
            return result;
        }

        public async static Task<List<Team>> LoadTeamsAsync(string competitionid)
        {
            List<Team> result;
            try
            {
                List<Team> list = new List<Team>();

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/jumplist/startseite/wettbewerb/" + competitionid);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);


                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//select[@name='verein_select_breadcrumb']");
                if (htmlNodeCollection != null)
                {
                    string innerHtml = htmlNodeCollection.ElementAt(0).InnerHtml;
                    string[] array = Regex.Split(innerHtml, "\\n");
                    for (int i = 2; i < array.Length - 1; i++)
                    {
                        string[] array2 = Regex.Split(array[i], "\"");
                        list.Add(new Team
                        {
                            TeamId = int.Parse(array2[1]),
                            TeamName = array2[2].Substring(1),
                            CompetitionId = competitionid
                        });
                    }
                }
                result = list;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                result = null;
            }
            return result;
        }

        public async static Task<List<Player>> LoadPlayersAsync(int teamid)
        {
            List<Player> result = new List<Player>();
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/jumplist/startseite/verein/" + teamid);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);


                var playersTableNodes = htmlDocument.DocumentNode.SelectNodes("//table[@class='items']");
                if (playersTableNodes.Count > 0)
                {
                    var playersTableNode = playersTableNodes.First();
                    var items = playersTableNode.SelectNodes("tbody/tr");
                    if (items != null)
                    {
                        foreach (HtmlNode current in items)
                        {

                            string name = null, playerId = null, position = null, tmurl = null, playernr = null, age = null, marketValue = null, injury = null;

                            //PLAYER NAME, PLAYER ID
                            var data = current.SelectNodes(".//a[@class='spielprofil_tooltip']");
                            if (data != null)
                            {
                                name = data.First().InnerText;
                                playerId = data.First().GetAttributeValue("id", null);
                                tmurl = data.First().GetAttributeValue("href", null);
                            }

                            //MAIN POSITION
                            data = current.SelectNodes(".//td/table/tr");
                            if (data != null)
                                position = data.Last().InnerText.Replace("&nbsp;", "");

                            //PLAYER NUMBER
                            data = current.SelectNodes(".//div[@class='rn_nummer']");
                            if (data != null)
                                playernr = data.First().InnerText;

                            //AGE
                            data = current.SelectNodes("td");
                            if (data != null)
                            {
                                if (data.Count > 2)
                                {
                                    age = data.ElementAt(2).InnerText;
                                    if (age.IndexOf("(") != -1)
                                    {
                                        age = age.Substring(age.IndexOf("(") + 1);
                                        age = age.Replace(")", "").Trim();
                                    }
                                }
                                if (data.Count > 4)
                                    marketValue = data.ElementAt(4).InnerText;

                                data = current.SelectNodes(".//td[@class='hauptlink']/span");
                                if (data != null)
                                {
                                    injury = data.First().GetAttributeValue("title", null);
                                }
                            }

                            //ADD PLAYER TO LISTs
                            result.Add(new Player
                            {
                                Name = WebUtility.HtmlDecode(name),
                                PlayerId = int.Parse(playerId),
                                MainPosition = Helper.PositionShortName(position),
                                TmUrl = tmurl,
                                PlayerNr = playernr,
                                Age = age,
                                MarketValue = marketValue,
                                Injury = injury,
                                Lineup = Player.LineUpStatus.YES
                            });
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
            return result;
        }

        public async static Task<Statistics> LoadPlayerStatisticsAsync(string playerStatsUrl, bool loadMatches, string year, string playerPosition)
        {
            string contract = null;
            Statistics statistics = new Statistics();
            Statistics result;
            try
            {
                new List<Team>();

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/" + playerStatsUrl);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);


                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//table[@class='items']/tfoot/tr/td");
                if (htmlNodeCollection != null)
                {
                    short num = 0;
                    short num2 = 0;
                    short num3 = 0;
                    short num4 = 0;
                    short num5 = 0;
                    short.TryParse(htmlNodeCollection.ElementAt(2).InnerText, out num);
                    short.TryParse(htmlNodeCollection.ElementAt(3).InnerText, out num2);
                    short.TryParse(htmlNodeCollection.ElementAt(4).InnerText, out num5);
                    if (playerPosition != "GK")
                    {
                        short.TryParse(htmlNodeCollection.ElementAt(12).InnerText.Replace(".", "").Replace("'", ""), out num4);
                    }
                    else
                    {
                        short.TryParse(htmlNodeCollection.ElementAt(10).InnerText, out num2);
                        short.TryParse(htmlNodeCollection.ElementAt(11).InnerText, out num5);
                    }
                    short.TryParse(htmlNodeCollection.ElementAt(htmlNodeCollection.Count - 1).InnerText.Replace(".", "").Replace("'", ""), out num3);
                    statistics.GamesPlayed = num.ToString();
                    statistics.GoalsScored = num2.ToString();
                    statistics.MinutesPlayed = num3.ToString();
                    statistics.Assists = num5.ToString();
                    statistics.Year = year;
                    statistics.MinutesPerGoal = num4.ToString();
                }
                HtmlNodeCollection htmlNodeCollection2 = htmlDocument.DocumentNode.SelectNodes("//table[@class='items']/tbody/tr/td");
                if (htmlNodeCollection2 != null)
                {
                    short num6 = 0;
                    short num7 = 0;
                    short.TryParse(htmlNodeCollection2.ElementAt(3).InnerText, out num6);
                    if (playerPosition != "GK")
                    {
                        short.TryParse(htmlNodeCollection2.ElementAt(12).InnerText.Replace(".", "").Replace("'", ""), out num7);
                    }
                    if (playerPosition != "GK")
                    {
                        short.TryParse(htmlNodeCollection2.ElementAt(12).InnerText.Replace(".", "").Replace("'", ""), out num7);
                    }
                    else
                    {
                        short.TryParse(htmlNodeCollection2.ElementAt(10).InnerText, out num6);
                        short.TryParse(htmlNodeCollection2.ElementAt(11).InnerText, out num7);
                    }
                    statistics.GoalsScoredTopLeague = num6.ToString();
                    statistics.MinutesPerGoalTopLeague = num7.ToString();
                }
                if (loadMatches)
                {
                    string text = "";
                    string text2 = "";
                    HtmlNode htmlNode = htmlDocument.DocumentNode.SelectNodes("//table[@class='profilheader']").ElementAt(0);
                    HtmlNodeCollection htmlNodeCollection3 = htmlNode.SelectNodes("tr/td");
                    if (htmlNodeCollection3 != null)
                    {
                        text = htmlNodeCollection3.ElementAt(4).InnerText;
                        text = text.Replace("\r\n", "").Replace("\t", "");
                    }
                    HtmlNode htmlNode2 = htmlDocument.DocumentNode.SelectNodes("//table[@class='profilheader']").ElementAt(1);
                    htmlNodeCollection3 = htmlNode2.SelectNodes("tr/td");
                    if (htmlNodeCollection3 != null)
                    {
                        text2 = htmlNodeCollection3.ElementAt(0).InnerText;
                        text2 = text2.Replace("\r\n", "").Replace("\t", "");
                    }
                    contract = text + "-" + text2;
                    List<Match> matches = new List<Match>();
                    htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[@class='responsive-table']/table/tbody/tr");
                    if (htmlNodeCollection != null)
                    {
                        foreach (HtmlNode current in htmlNodeCollection)
                        {
                            HtmlNodeCollection htmlNodeCollection4 = current.SelectNodes("td");
                            if (htmlNodeCollection4.Count > 1)
                            {
                                Match match = new Match();
                                string arg_3AA_0 = current.InnerHtml;
                                try
                                {
                                    match.Date =
                                      new DateTime?(DateTime.ParseExact(htmlNodeCollection4.ElementAt(1).InnerText,
                                        "MMM d, yyyy", null, DateTimeStyles.None));
                                }
                                catch (Exception)
                                {
                                }
                                match.HomeTeam = htmlNodeCollection4.ElementAt(3).InnerText;
                                match.VisitingTeam = htmlNodeCollection4.ElementAt(5).InnerText;
                                match.Result = htmlNodeCollection4.ElementAt(6).InnerText;
                                if (htmlNodeCollection4.Count >= 17)
                                {
                                    match.Position = htmlNodeCollection4.ElementAt(7).InnerText.Replace("&nbsp;", "");
                                    match.Goals = htmlNodeCollection4.ElementAt(8).InnerText.Replace("&npbsp;", "");
                                    match.Assists = htmlNodeCollection4.ElementAt(9).InnerText.Replace("&npbsp;", "");
                                    match.OwnGoals = htmlNodeCollection4.ElementAt(10).InnerText.Replace("&npbsp;", "");
                                    match.YellowCards =
                                      htmlNodeCollection4.ElementAt(11)
                                        .InnerText.Replace("&#10004;", "*")
                                        .Replace("&npbsp;", "").Replace("'", "");
                                    match.YellowRedCards =
                                      htmlNodeCollection4.ElementAt(12)
                                        .InnerText.Replace("&#10004;", "*")
                                        .Replace("&npbsp;", "").Replace("'", "");
                                    match.RedCards =
                                      htmlNodeCollection4.ElementAt(13)
                                        .InnerText.Replace("&#10004;", "*")
                                        .Replace("&npbsp;", "").Replace("'", "");
                                    match.SubstitutedOn =
                                      htmlNodeCollection4.ElementAt(14).InnerText.Replace("&#10004;", "*").Replace("&nbsp;", "").Replace("'", "");
                                    match.SubstitutedOff =
                                      htmlNodeCollection4.ElementAt(15)
                                        .InnerText.Replace("&#10004;", "*")
                                        .Replace("&npbsp;", "").Replace("'", "");
                                    if (htmlNodeCollection4.Count > 17)
                                    {
                                        match.MinutesPlayed =
                                          htmlNodeCollection4.ElementAt(17)
                                            .InnerText.Replace("&#10004;", "*")
                                            .Replace("&npbsp;", "").Replace("'", "");
                                    }
                                    else
                                    {
                                        match.MinutesPlayed =
                                          htmlNodeCollection4.ElementAt(16)
                                            .InnerText.Replace("&#10004;", "*")
                                            .Replace("&npbsp;", "").Replace("'", "");
                                    }
                                }
                                else
                                {
                                    match.Description = htmlNodeCollection4.ElementAt(7).InnerText.Replace("&nbsp;", "");
                                }
                                matches.Add(match);
                            }
                        }
                    }
                    statistics.Contract = contract;
                    statistics.Matches = matches;
                }

                result = statistics;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                result = null;
            }
            return result;
        }

        public async static Task<AdditionalPlayerData> LoadAdditionPlayerDataAsync(string playerProfileDataUrl)
        {
            AdditionalPlayerData adp = new AdditionalPlayerData();
            try
            {

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/" + playerProfileDataUrl);
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                HtmlDocument htmlDocument = new HtmlDocument();
                string ss = await response.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(ss);

                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[@class='spielerdaten']/table/tr");
                if (htmlNodeCollection != null)
                {
                    foreach (HtmlNode current in htmlNodeCollection)
                    {
                        if (current.InnerText.Contains("Foot:"))
                            adp.PrefferedFoot = current.InnerText.Trim().Substring(5).Trim();
                    }
                }
                HtmlNodeCollection htmlNodeCollection2 = htmlDocument.DocumentNode.SelectNodes("//div[@class='detailpositionen']/div/div[2]/table/tr[2]/td/a");
                if (htmlNodeCollection2 != null)
                {
                    foreach (HtmlNode current2 in htmlNodeCollection2)
                        adp.SecondaryPositions.Add(Helper.PositionShortName(current2.InnerText));
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return adp;
        }
    }
}
