using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
              catch (Exception ex)
              {
                Logger.Exception(ex);
                //MessageBox.Show(ex.Message);
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
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
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

        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@data-id='substitutions']");
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
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
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
        //MessageBox.Show(ex.Message);
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
            t.UrlName = t.Url.Split('/')[1];
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
        //MessageBox.Show(ex.Message);
        result = null;
      }
      return result;
    }

    public async static Task<List<Competition>> LoadCupCompetitionsAsync()
    {
      var result = new List<Competition>();
      try
      {
        List<Competition> list = new List<Competition>();

        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/");
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);

        var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'wettbewerbe-auflistung')]").ElementAt(0).SelectNodes("li/a");

        if (htmlNodeCollection != null)
        {
          foreach (HtmlNode node in htmlNodeCollection)
          {
            Competition c = new Competition();
            c.CompetitionName = node.GetAttributeValue("title", "");
            c.CompetitionId = node.GetAttributeValue("href", "").Split('/').LastOrDefault();
            c.CompetitionCountryId = -2;
            c.CompetitionCountry = "CUP COMPETITIONS";
            result.Insert(0, c);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
        result = null;
      }
      return result;
    }

    public async static Task<List<Competition>> LoadInternationalCupCompetitionsAsync()
    {
      var result = new List<Competition>();
      try
      {
        List<Competition> list = new List<Competition>();

        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/");
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);

        var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'wettbewerbe-auflistung')]").ElementAt(1).SelectNodes("li/a");

        if (htmlNodeCollection != null)
        {
          Competition c;
          foreach (HtmlNode node in htmlNodeCollection)
          {
            c = new Competition();
            c.CompetitionName = node.GetAttributeValue("title", "");
            c.CompetitionId = node.GetAttributeValue("href", "").Split('/').LastOrDefault();
            c.CompetitionCountryId = -3;
            c.CompetitionCountry = "INT. CUP COMPETITIONS";
            result.Insert(0, c);
          }

          c = new Competition();
          c.CompetitionName = "EUROPEAN QUALIFIERS";
          c.CompetitionId = "EMQ";
          c.CompetitionCountryId = -3;
          c.CompetitionCountry = "INT. CUP COMPETITIONS";
          result.Insert(0, c);
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
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
              TeamName = array2[2].Substring(1).Replace("/", "-"),
              UrlName = array2[2].Substring(1).ToLower().Replace(" ", "-").Replace("/", "-"),
              CompetitionId = competitionid
            });
          }
        }
        result = list;
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
        result = null;
      }
      return result;
    }

    public async static Task<List<Team>> LoadTeamsAlternativeAsync(string competitionid)
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


        var fnc = htmlDocument.DocumentNode.SelectNodes("//div[@class='flagge']/a/img");

        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[@id='yw1']");
        if (htmlNodeCollection != null)
        {
          var nodes = htmlNodeCollection.ElementAt(0).SelectNodes("table/tbody/tr");
          foreach (var row in nodes)
          {
            var href = row.SelectNodes("td").ElementAt(0).SelectNodes("a").ElementAt(0).Attributes["href"].Value;
            var urlname = href.Split('/')[1];
            var teamid = href.Split('/').ElementAt(4);
            var title = row.SelectNodes("td").ElementAt(1).SelectNodes("a").ElementAt(0).InnerHtml;
            list.Add(new Team
            {
              TeamId = int.Parse(teamid),
              TeamName = title,
              CompetitionId = competitionid,
              UrlName = urlname
            });
          }
        }
        result = list;
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
        result = null;
      }
      return result;
    }

    public async static Task<List<Team>> LoadTeamsSearchPageAsync(string competitionid)
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
        
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//div[@class='box']/table/tbody/tr/td/a");
        if (htmlNodeCollection != null)
        {       
          foreach (var a in htmlNodeCollection)
          {
            var href = a.GetAttributeValue("href", "");
            if (href.Split('/').Length>0&& href.Contains("verein"))
            {
              var urlname = href.Split('/')[1];
              var teamid = href.Split('/').ElementAt(4);
              if (a.InnerText != "")
              {
                var title = a.InnerHtml;
                list.Add(new Team
                {
                  TeamId = int.Parse(teamid),
                  TeamName = title,
                  CompetitionId = competitionid,
                  UrlName = urlname
                });
              }
            }
          }
        }
        result = list;
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
        result = null;
      }
      return result;
    }

    public async static Task<List<Player>> LoadPlayersAsync(int teamid)
    {
      List<Player> result = new List<Player>();
      try
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/jumplist/startseite/verein/" + teamid);
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value. 
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);



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
                if (data.Count > 3)
                {
                  age = data.ElementAt(3).InnerText;
                  if (age.IndexOf("(") != -1)
                  {
                    age = age.Substring(age.IndexOf("(") + 1);
                    age = age.Replace(")", "").Trim();
                  }
                }
                if (data.Count > 5)
                  marketValue = data.ElementAt(5).InnerText;

                data = current.SelectNodes(".//td[@class='hauptlink']/span");
                if (data != null)
                {
                  injury = data.First().GetAttributeValue("title", null);
                }
              }

              var mainPosition = Helper.PositionShortName(position);
              //ADD PLAYER TO LISTs
              result.Add(new Player
              {
                Name = WebUtility.HtmlDecode(name),
                PlayerId = int.Parse(playerId),
                MainPosition = mainPosition,
                TmUrl = tmurl,
                PlayerNr = playernr,
                Age = age,
                MarketValue = marketValue,
                Injury = injury,
                Lineup = Player.LineUpStatus.UNKNOWN
              });
            }
          }
        }

      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
      }
      return result;
    }

    public async static Task<Statistics> LoadPlayerStatisticsAsync(string playerStatsUrl, bool loadMatches, string year, string playerPosition)
    {
      string contract = null;
      Statistics statistics = new Statistics()
      {
        GamesPlayed = "-",
        GoalsScored = "-",
        MinutesPlayed = "-",
        Assists = "0",
        Year = year,
        MinutesPerGoal = "-"
      };
      Statistics result;
      try
      {
        new List<Team>();

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/" + playerStatsUrl);
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);


        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value. 
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);



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
            if (htmlNodeCollection3.Count > 5)
              text = htmlNodeCollection3.ElementAt(5).InnerText;
            else if (htmlNodeCollection3.Count > 4)
              text = htmlNodeCollection3.ElementAt(4).InnerText;

            text = text.Replace("\r\n", "").Replace("\t", "");
          }
          HtmlNode htmlNode2 = htmlDocument.DocumentNode.SelectNodes("//table[@class='profilheader']").ElementAt(1);
          htmlNodeCollection3 = htmlNode2.SelectNodes("tr/td");
          if (htmlNodeCollection3 != null)
          {
            text2 = htmlNodeCollection3.ElementAt(0).InnerText;
            text2 = text2.Replace("\r\n", "").Replace("\t", "");
            if (htmlNodeCollection3.Count > 4)
            {
              var np = htmlNodeCollection3.ElementAt(4).InnerText.Replace("&nbsp;", "").Replace("\r\n", "").Trim();
              var npurl = htmlNodeCollection3.ElementAt(4).SelectNodes("a").ElementAt(0).Attributes["href"].Value;
              statistics.NationalPlayer = np;
              statistics.NationalPlayerUrl = npurl;
            }
          }
          contract = text2 + "-" + text;
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
                match.HomeTeamName = htmlNodeCollection4.ElementAt(3).InnerText;
                match.VisitingTeamName = htmlNodeCollection4.ElementAt(5).InnerText;
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
        //MessageBox.Show(ex.Message);
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

    public async static Task<List<Match>> LoadSchedule(List<Team> cachedTeams, Team team, string year)
    {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

      List<Match> schedule = new List<Match>();
      try
      {
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.transfermarkt.com/" + team.UrlName + "/spielplan/verein/" + team.TeamId.ToString() + "/saison_id/" + year);
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);


        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value. 
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);

        HtmlNode theBox = null;

        var boxes = htmlDocument.DocumentNode.SelectNodes("//div[@class='box']");
        foreach (var box in boxes)
        {
          var links = box.SelectNodes("div/a");
          if (links != null)
          {
            foreach (var link in links)
            {
              string refer = link.GetAttributeValue("href", null);
              if (refer != null && refer.Contains("#" + team.CompetitionId) == true)
              {
                theBox = box;
                break;
              }
            }
            if (theBox != null)
              break;
          }
        }
        if (theBox == null)
          return schedule;

        var st = theBox.SelectNodes("div[@class='responsive-table']");
        if (st.Count > 0)
        {
          var scheduleTable = st[0];

          var rows = scheduleTable.SelectNodes("table/tbody/tr");

          foreach (var row in rows)
          {
            var match = new Match();
            DateTime mdate;

            string date = row.SelectNodes("td").ElementAt(1).InnerHtml;
            string time = row.SelectNodes("td").ElementAt(2).InnerHtml;
            string dt = date + " " + time;


            match.Date = null;

            if (time != "")
            {
              bool s = DateTime.TryParseExact(dt, "ddd MMM d, yyyy h:mm tt", null, DateTimeStyles.None, out mdate);
              if (s == true)
                match.Date = mdate;
            }
            else
            {
              bool s = DateTime.TryParseExact(dt, "ddd MMM d, yyyy", null, DateTimeStyles.None, out mdate);
              if (s == true)
                match.Date = mdate;
            }



            string homeaway = row.SelectNodes("td").ElementAt(3).InnerHtml;


            var opponent = row.SelectNodes("td").ElementAt(6);

            string href = opponent.SelectNodes("a").FirstOrDefault().GetAttributeValue("href", null);

            string id = href.Split('/')[4];

            var opponentTeam = cachedTeams.Where(ct => ct.TeamId.ToString() == id).FirstOrDefault();
            if (opponentTeam == null)
            {
              opponentTeam = new Team() { TeamId = int.Parse(id), CompetitionId = team.CompetitionId };
            }

            string opponentName = opponent.SelectNodes("a").FirstOrDefault().InnerHtml;

            if (homeaway == "H")
            {
              match.HomeTeam = team;
              match.VisitingTeam = opponentTeam;
            }
            else
            {
              match.VisitingTeam = team;
              match.HomeTeam = opponentTeam;
            }

            string formation = row.SelectNodes("td").ElementAt(7).InnerHtml;
            string spectators = row.SelectNodes("td").ElementAt(8).InnerHtml;
            string result = row.SelectNodes("td").ElementAt(9).InnerText;
            //var matchReportUrl = 
            var mrn = row.SelectNodes("td").ElementAt(9).SelectNodes("a");
            if (mrn != null)
            {
              var mrurl = mrn.FirstOrDefault().GetAttributeValue("href", null);
              match.MatchReportUrl = "http://www.transfermarkt.com/" + mrurl;
            }

            match.Result = result;

            schedule.Add(match);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
      }
      return schedule;
    }

    public static async Task<string> GetFootage(string home, string away, DateTime date)
    {
      string url = null;
      try
      {
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://footyroom.com/do/search.php?q=" + home);
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);

        dynamic data = JsonConvert.DeserializeObject(ss);
        dynamic results = data.matches;
        JArray arr = JArray.FromObject(results);
        if (arr.Count > 0)
        {
          foreach (dynamic el in arr)
          {
            string dt = el.datetime;
            DateTime dtr;
            bool s = DateTime.TryParse(dt, out dtr);
            if (s == true)
              if (dtr.Date == date.Date)
              {
                url = "http://www.footyroom.com?p=" + el.postId;
                break;
              }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
      }
      return url;
    }


    public static async Task<string> GetStream(string home, string away)
    {
      string url = null;
      try
      {
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.realstreamunited.com/search.php?option=com_search&tmpl=raw&type=json&ordering=&searchphrase=all&Itemid=207&areas[]=event&searchword=" + home+" "+away + "&sef=1&limitstart=0");
        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        HtmlDocument htmlDocument = new HtmlDocument();
        string ss = await response.Content.ReadAsStringAsync();
        htmlDocument.LoadHtml(ss);

        dynamic data = JsonConvert.DeserializeObject(ss);
        dynamic results = data.results;
        JArray arr = JArray.FromObject(results);
        if (arr.Count > 0)
        {
          dynamic link = arr[0];
          url = "http://www.realstreamunited.com/" + link.url;
          httpClient = new HttpClient();
          requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
          requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

          response = await httpClient.SendAsync(requestMessage);
          htmlDocument = new HtmlDocument();
          ss = await response.Content.ReadAsStringAsync();
          htmlDocument.LoadHtml(ss);

          var recom = htmlDocument.DocumentNode.SelectNodes("//table/tbody/tr/td");
          foreach (var td in recom)
          {
            if (td.InnerText.Contains("Recommended"))
            {
              url = td.SelectNodes("a")[0].GetAttributeValue("href", "");
              break;
            }
          }

        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        //MessageBox.Show(ex.Message);
      }
      return url;
    }
  }
}
