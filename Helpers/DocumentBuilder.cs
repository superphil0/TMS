using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TMS
{
  internal class DocumentBuilder
  {
    private static List<XLColor> seasonColors = new List<XLColor>
        {
      XLColor.LightGreen,
      XLColor.Yellow,
      XLColor.Orange,
      XLColor.LightBrown
    };
    public static void CreateCXMLDocument(string excelFilename, Team t)
    {
      if (File.Exists(excelFilename))
      {
        File.Delete(excelFilename);
      }
      XLWorkbook xLWorkbook = new XLWorkbook();
      IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add(t.TeamName.NormalizeString().Left(31));

      iXLWorksheet.Cell("A1").Value = (t.TeamName);
      iXLWorksheet.Cell("A1").Style.Font.FontSize = (16.0);
      iXLWorksheet.Cell("A1").Style.Font.FontColor = (XLColor.DarkBlue);
      iXLWorksheet.Cell("A1").Hyperlink = (new XLHyperlink("http://www.transfermarkt.com/" + t.Url));

      List<Player> lineupPlayers = new List<Player>();
      if (t.Players != null)
        lineupPlayers = t.Players.Where(p => p.Lineup == Player.LineUpStatus.YES).ToList();


      Player emptyGoalkeeper = new Player() { MainPosition = "GK", Statistics = new List<Statistics>() { new Statistics() { }, new Statistics() { }, new Statistics() { }, new Statistics() { } } };
      Player emptyDefender = new Player() { MainPosition = "CB", Statistics = new List<Statistics>() { new Statistics() { }, new Statistics() { }, new Statistics() { }, new Statistics() { } } };
      Player emptyMidfielder = new Player() { MainPosition = "DM", Statistics = new List<Statistics>() { new Statistics() { }, new Statistics() { }, new Statistics() { }, new Statistics() { } } };
      Player emptyAttacker = new Player() { MainPosition = "SS", Statistics = new List<Statistics>() { new Statistics() { }, new Statistics() { }, new Statistics() { }, new Statistics() { } } };

      int gkCount = 5;
      int dfCount = 14;
      int mfCount = 14;
      int atCount = 14;

      var goalkeepers = lineupPlayers.Where(lp => lp.MainPosition == "GK");
      var defenders = lineupPlayers.Where(lp => lp.MainPosition == "CB" || lp.MainPosition == "LB" || lp.MainPosition == "RB");
      var midfielders = lineupPlayers.Where(lp => lp.MainPosition == "DM" || lp.MainPosition == "CM" || lp.MainPosition == "LM" ||
                          lp.MainPosition == "RM" || lp.MainPosition == "AM");
      var attackers = lineupPlayers.Where(lp => lp.MainPosition == "LW" || lp.MainPosition == "RW" || lp.MainPosition == "CF" ||
                              lp.MainPosition == "SS");

      lineupPlayers = new List<Player>();

      for (int i = 0; i < gkCount; i++)
        if (i < goalkeepers.Count())
          lineupPlayers.Add(goalkeepers.ElementAt(i));
        else
          lineupPlayers.Add(emptyGoalkeeper);

      for (int i = 0; i < dfCount; i++)
        if (i < defenders.Count())
          lineupPlayers.Add(defenders.ElementAt(i));
        else
          lineupPlayers.Add(emptyDefender);

      for (int i = 0; i < mfCount; i++)
        if (i < midfielders.Count())
          lineupPlayers.Add(midfielders.ElementAt(i));
        else
          lineupPlayers.Add(emptyMidfielder);

      for (int i = 0; i < atCount; i++)
        if (i < attackers.Count())
          lineupPlayers.Add(attackers.ElementAt(i));
        else
          lineupPlayers.Add(emptyAttacker);

      try
      {
        int playersCount = lineupPlayers.Count();
        for (int i = 0; i < playersCount; i++)
        {
          Player player = lineupPlayers.ElementAt(i);

          iXLWorksheet.Row(i * 5 + 1).Style.Fill.BackgroundColor = (XLColor.AshGrey);
          iXLWorksheet.Column("A").Width = (12.0);
          iXLWorksheet.Column("B").Width = (30.0);
          iXLWorksheet.Column("D").Width = (3.0);
          iXLWorksheet.Column("E").Width = (3.0);
          iXLWorksheet.Column("F").Width = (3.0);
          iXLWorksheet.Column("G").Width = (3.0);
          iXLWorksheet.Column("H").Width = (3.0);
          iXLWorksheet.Column("I").Width = (3.0);
          iXLWorksheet.Column("J").Width = (3.0);
          iXLWorksheet.Column("K").Width = (3.0);
          iXLWorksheet.Column("L").Width = (3.0);
          if (player.MainPosition == "GK")
          {
            iXLWorksheet.Cell("A" + (i * 5 + 2).ToString()).Style.Fill.BackgroundColor = (XLColor.LightGreen);
            iXLWorksheet.Cell("A" + (i * 5 + 3).ToString()).Style.Fill.BackgroundColor = (XLColor.LightGreen);
            iXLWorksheet.Cell("A" + (i * 5 + 4).ToString()).Style.Fill.BackgroundColor = (XLColor.LightGreen);
            iXLWorksheet.Cell("A" + (i * 5 + 5).ToString()).Style.Fill.BackgroundColor = (XLColor.LightGreen);
          }
          else
          {
            if (player.MainPosition == "CB" || player.MainPosition == "LB" || player.MainPosition == "RB")
            {
              iXLWorksheet.Cell("A" + (i * 5 + 2).ToString()).Style.Fill.BackgroundColor = (XLColor.BabyBlue);
              iXLWorksheet.Cell("A" + (i * 5 + 3).ToString()).Style.Fill.BackgroundColor = (XLColor.BabyBlue);
              iXLWorksheet.Cell("A" + (i * 5 + 4).ToString()).Style.Fill.BackgroundColor = (XLColor.BabyBlue);
              iXLWorksheet.Cell("A" + (i * 5 + 5).ToString()).Style.Fill.BackgroundColor = (XLColor.BabyBlue);
            }
            else
            {
              if (player.MainPosition == "DM" || player.MainPosition == "CM" || player.MainPosition == "LM" ||
                  player.MainPosition == "RM" || player.MainPosition == "AM")
              {
                iXLWorksheet.Cell("A" + (i * 5 + 2).ToString()).Style.Fill.BackgroundColor = (XLColor.Orange);
                iXLWorksheet.Cell("A" + (i * 5 + 3).ToString()).Style.Fill.BackgroundColor = (XLColor.Orange);
                iXLWorksheet.Cell("A" + (i * 5 + 4).ToString()).Style.Fill.BackgroundColor = (XLColor.Orange);
                iXLWorksheet.Cell("A" + (i * 5 + 5).ToString()).Style.Fill.BackgroundColor = (XLColor.Orange);
              }
              else
              {
                if (player.MainPosition == "LW" || player.MainPosition == "RW" || player.MainPosition == "CF" ||
                    player.MainPosition == "SS")
                {
                  iXLWorksheet.Cell("A" + (i * 5 + 2).ToString()).Style.Fill.BackgroundColor = (XLColor.Red);
                  iXLWorksheet.Cell("A" + (i * 5 + 3).ToString()).Style.Fill.BackgroundColor = (XLColor.Red);
                  iXLWorksheet.Cell("A" + (i * 5 + 4).ToString()).Style.Fill.BackgroundColor = (XLColor.Red);
                  iXLWorksheet.Cell("A" + (i * 5 + 5).ToString()).Style.Fill.BackgroundColor = (XLColor.Red);
                }
              }
            }
          }
          iXLWorksheet.Cell("C" + (i * 5 + 1).ToString()).Value = ("PO");
          iXLWorksheet.Cell("D" + (i * 5 + 1).ToString()).Value = ("GO");
          iXLWorksheet.Cell("E" + (i * 5 + 1).ToString()).Value = ("AG");
          iXLWorksheet.Cell("F" + (i * 5 + 1).ToString()).Value = ("AS");
          iXLWorksheet.Cell("G" + (i * 5 + 1).ToString()).Value = ("ZK");
          iXLWorksheet.Cell("H" + (i * 5 + 1).ToString()).Value = ("ZC");
          iXLWorksheet.Cell("I" + (i * 5 + 1).ToString()).Value = ("CK");
          iXLWorksheet.Cell("J" + (i * 5 + 1).ToString()).Value = ("SN");
          iXLWorksheet.Cell("K" + (i * 5 + 1).ToString()).Value = ("SF");
          iXLWorksheet.Cell("L" + (i * 5 + 1).ToString()).Value = ("MI");
          iXLWorksheet.Cell("M" + (i * 5 + 1).ToString()).Value = ("UU");
          iXLWorksheet.Cell("N" + (i * 5 + 1).ToString()).Value = ("GU");
          iXLWorksheet.Cell("O" + (i * 5 + 1).ToString()).Value = ("AU");
          iXLWorksheet.Cell("P" + (i * 5 + 1).ToString()).Value = ("MU");
          iXLWorksheet.Cell("Q" + (i * 5 + 1).ToString()).Value = ("MPG");
          iXLWorksheet.Cell("M" + (i * 5 + 1).ToString()).Style.Alignment.Horizontal = (0);
          iXLWorksheet.Cell("N" + (i * 5 + 1).ToString()).Style.Alignment.Horizontal = (0);
          iXLWorksheet.Cell("O" + (i * 5 + 1).ToString()).Style.Alignment.Horizontal = (0);
          iXLWorksheet.Cell("P" + (i * 5 + 1).ToString()).Style.Alignment.Horizontal = (0);
          iXLWorksheet.Cell("Q" + (i * 5 + 1).ToString()).Style.Alignment.Horizontal = (0);
          string text = player.MainPosition;
          if (player.SecondaryPositions != null && player.SecondaryPositions.Count > 0)
          {
            text += " (";
            for (int j = 0; j < player.SecondaryPositions.Count; j++)
            {
              text += player.SecondaryPositions[j];
              if (j < player.SecondaryPositions.Count - 1)
              {
                text += ",";
              }
            }
            text += ")";
          }
          iXLWorksheet.Cell("A" + (i * 5 + 2).ToString()).Value = (text);
          iXLWorksheet.Cell("A" + (i * 5 + 3).ToString()).Value = ("[" + player.PlayerNr + "] " +
                                                                 player.PrefferedFoot);
          if (player.Statistics !=null && player.Statistics[0] != null)
          {
            if (player.Statistics[0].GoalsScoredTopLeague == "0")
            {
              iXLWorksheet.Cell("A" + (i * 5 + 4).ToString()).Value = ("0.000001");
            }
            else
            {
              iXLWorksheet.Cell("A" + (i * 5 + 4).ToString()).Value = (player.Statistics[0].GoalsScoredTopLeague);
            }
            if (player.Statistics[0].MinutesPerGoalTopLeague == "0")
            {
              iXLWorksheet.Cell("A" + (i * 5 + 5).ToString()).Value = ("0.000001");
            }
            else
            {
              iXLWorksheet.Cell("A" + (i * 5 + 5).ToString()).Value = (player.Statistics[0].MinutesPerGoalTopLeague);
            }
          }
          iXLWorksheet.Cell("A" + (i * 5 + 3).ToString()).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Left;
          iXLWorksheet.Cell("B" + (i * 5 + 2).ToString()).Hyperlink =
            new XLHyperlink("http://www.transfermarkt.com/" + player.TmUrl);
          iXLWorksheet.Cell("B" + (i * 5 + 2).ToString()).Value = (player.Name + ", " + player.Age);
          iXLWorksheet.Cell("B" + (i * 5 + 2).ToString()).Style.Font.Bold = (true);
          iXLWorksheet.Cell("B" + (i * 5 + 3).ToString()).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Left;
          iXLWorksheet.Cell("B" + (i * 5 + 3).ToString()).Value = (player.Injury);
          iXLWorksheet.Cell("B" + (i * 5 + 3).ToString()).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Left;
          iXLWorksheet.Cell("B" + (i * 5 + 4).ToString()).Value = player.Contract;
          iXLWorksheet.Cell("B" + (i * 5 + 4).ToString()).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Left;

          var cv = (player.MarketValue);
          if (player.NationalPlayer != null)
            cv += " / " + player.NationalPlayer;

          iXLWorksheet.Cell("B" + (i * 5 + 5).ToString()).Value = cv;
          iXLWorksheet.Cell("B" + (i * 5 + 5).ToString()).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Left;
          if (player.NationalPlayerUrl != null)
            iXLWorksheet.Cell("B" + (i * 5 + 5).ToString()).Hyperlink =
                new XLHyperlink("http://www.transfermarkt.com/" + player.NationalPlayerUrl);


          if (player.Matches != null)
          {
            for (int k = 0; k < player.Matches.Count; k++)
            {
              Match match = player.Matches[k];
              if (match.Description != null)
              {
                iXLWorksheet.Columns("C").Width = 20.0;
                iXLWorksheet.Cell("C" + (i * 5 + k + 2).ToString()).Value = match.Description;
              }
              else
              {
                iXLWorksheet.Columns("C").Width = (3.0);
                iXLWorksheet.Cell("C" + (i * 5 + k + 2).ToString()).Value = match.Position;
                iXLWorksheet.Cell("D" + (i * 5 + k + 2).ToString()).Value = match.Goals;
                iXLWorksheet.Cell("E" + (i * 5 + k + 2).ToString()).Value = match.OwnGoals;
                iXLWorksheet.Cell("F" + (i * 5 + k + 2).ToString()).Value = match.Assists;
                iXLWorksheet.Cell("G" + (i * 5 + k + 2).ToString()).Value = match.YellowCards;
                iXLWorksheet.Cell("H" + (i * 5 + k + 2).ToString()).Value = match.YellowRedCards;
                iXLWorksheet.Cell("I" + (i * 5 + k + 2).ToString()).Value = match.RedCards;
                iXLWorksheet.Cell("J" + (i * 5 + k + 2).ToString()).Value = match.SubstitutedOn;
                iXLWorksheet.Cell("K" + (i * 5 + k + 2).ToString()).Value = match.SubstitutedOff;
                iXLWorksheet.Cell("L" + (i * 5 + k + 2).ToString()).Value = match.MinutesPlayed;
              }
            }
          }
          if (player.Statistics != null)
          {
            for (int l = 0; l < player.Statistics.Count; l++)
            {
              Statistics statistics = player.Statistics[l];
              if (statistics == null)
              {
                statistics = new Statistics();
              }
              iXLWorksheet.Cell("M" + (i * 5 + l + 2).ToString()).Style.Alignment.Horizontal = (0);
              iXLWorksheet.Cell("N" + (i * 5 + l + 2).ToString()).Style.Alignment.Horizontal = (0);
              iXLWorksheet.Cell("O" + (i * 5 + l + 2).ToString()).Style.Alignment.Horizontal = (0);
              iXLWorksheet.Cell("P" + (i * 5 + l + 2).ToString()).Style.Alignment.Horizontal = (0);
              iXLWorksheet.Cell("Q" + (i * 5 + l + 2).ToString()).Style.Alignment.Horizontal = (0);
              iXLWorksheet.Cell("M" + (i * 5 + l + 2).ToString()).Style.Font.Bold = (true);
              iXLWorksheet.Cell("N" + (i * 5 + l + 2).ToString()).Style.Font.Bold = (true);
              iXLWorksheet.Cell("O" + (i * 5 + l + 2).ToString()).Style.Font.Bold = (true);
              iXLWorksheet.Cell("P" + (i * 5 + l + 2).ToString()).Style.Font.Bold = (true);
              iXLWorksheet.Cell("Q" + (i * 5 + l + 2).ToString()).Style.Font.Bold = (true);
              iXLWorksheet.Cell("M" + (i * 5 + l + 2).ToString()).Style.Fill.BackgroundColor =
                DocumentBuilder.seasonColors[l];
              iXLWorksheet.Cell("N" + (i * 5 + l + 2).ToString()).Style.Fill.BackgroundColor =
                DocumentBuilder.seasonColors[l];
              iXLWorksheet.Cell("O" + (i * 5 + l + 2).ToString()).Style.Fill.BackgroundColor =
                DocumentBuilder.seasonColors[l];
              iXLWorksheet.Cell("P" + (i * 5 + l + 2).ToString()).Style.Fill.BackgroundColor =
                DocumentBuilder.seasonColors[l];
              iXLWorksheet.Cell("Q" + (i * 5 + l + 2).ToString()).Style.Fill.BackgroundColor =
                DocumentBuilder.seasonColors[l];
              if (statistics.GamesPlayed == null)
              {
                statistics.GamesPlayed = "0.000001";
              }
              else
              {
                if (statistics.GamesPlayed == "0")
                {
                  statistics.GamesPlayed = "0.000001";
                }
              }
              if (statistics.GoalsScored == null)
              {
                statistics.GoalsScored = "0.000001";
              }
              else
              {
                if (statistics.GoalsScored == "0")
                {
                  statistics.GoalsScored = "0.000001";
                }
              }
              if (statistics.Assists == null)
              {
                statistics.Assists = "0.000001";
              }
              else
              {
                if (statistics.Assists == "0")
                {
                  statistics.Assists = "0.000001";
                }
              }
              if (statistics.MinutesPlayed == null)
              {
                statistics.MinutesPlayed = "1";
              }
              else
              {
                if (statistics.MinutesPlayed == "0")
                {
                  statistics.MinutesPlayed = "1";
                }
              }
              if (statistics.MinutesPerGoal == null)
              {
                statistics.MinutesPerGoal = "0.000001";
              }
              else
              {
                if (statistics.MinutesPerGoal == "0")
                {
                  statistics.MinutesPerGoal = "0.000001";
                }
              }
              iXLWorksheet.Cell("M" + (i * 5 + l + 2).ToString()).Value = statistics.GamesPlayed.Replace("-",
                "0.000001");
              iXLWorksheet.Cell("N" + (i * 5 + l + 2).ToString()).Value = statistics.GoalsScored.Replace("-",
                "0.000001");
              iXLWorksheet.Cell("O" + (i * 5 + l + 2).ToString()).Value = statistics.Assists.Replace("-", "0.000001");
              iXLWorksheet.Cell("P" + (i * 5 + l + 2).ToString()).Value = statistics.MinutesPlayed.Replace("-",
                "0.000001");
              iXLWorksheet.Cell("Q" + (i * 5 + l + 2).ToString()).Value = statistics.MinutesPerGoal.Replace("-",
                "0.000001");
            }
          }
        }

        CreateSummaryTable(iXLWorksheet.Cell("H237"));

        MemoryStream ms = new MemoryStream();
        xLWorkbook.SaveAs(ms);
        FileStream file = new FileStream(excelFilename, FileMode.Create, FileAccess.Write);
        ms.WriteTo(file);
        file.Close();
        ms.Close();
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
    }

    public static void CreateCompetitionArchive(string excelFileName, Competition c)
    {
      foreach (Team t in c.Teams)
      {
        string fileName = Application.StartupPath + Helper.GetTeamArchiveFileName(c, t);
        if (File.Exists(fileName) == false)
          CreateCXMLDocument(fileName, t);
      }

      XLWorkbook xLWorkbook = new XLWorkbook();
      CreateDbStylesheet(c, xLWorkbook);
      CreateScheduleStylesheet(c, xLWorkbook, false);
      MemoryStream ms = new MemoryStream();
      xLWorkbook.SaveAs(ms);
      FileStream file = new FileStream(excelFileName, FileMode.Create, FileAccess.Write);
      ms.WriteTo(file);
      file.Close();
      ms.Close();
    }

    public static void UpdateCompetitionArchive(string excelFileName, Competition c)
    {
      try
      {
        XLWorkbook xLWorkbook = new XLWorkbook(excelFileName);
        CreateScheduleStylesheet(c, xLWorkbook, true);
        xLWorkbook.Save();
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("being used"))
          MessageBox.Show("Zatvori arhivu pa pokusaj opet!");
        else
          MessageBox.Show("Greska!");
      }
    }

    private static void CreateSummaryTable(IXLCell scr)
    {
      XLWorkbook template = new XLWorkbook(Application.StartupPath + Helper.GetTemplateFileName());
      var sheet = template.Worksheet("Template");
      var summaryTableRange = sheet.Range(237, 8, 242, 17);
      summaryTableRange.CopyTo(scr);
      template.Dispose();
    }

    private static void CreateScheduleStylesheet(Competition c, XLWorkbook xLWorkbook, bool updateMode)
    {
      var dbWorksheet = xLWorkbook.Worksheet("DB");
      int i = 0, m = 0;
      int dbrow, dbcolumn, linkedTeamIndex;
      Team linkedTeam;
      List<Match> teamSchedule;
      try
      {
        var templateWorksheet = new XLWorkbook(Application.StartupPath + Helper.GetTemplateFileName());
        var matchWorksheet = templateWorksheet.Worksheet("Match");
        var matchHomeRange = matchWorksheet.Range("A1", "Y8");
        IXLWorksheet iXLWorksheet;
        if (updateMode == false)
        {
          iXLWorksheet = xLWorkbook.Worksheets.Add("Schedule");
          iXLWorksheet.Position = 2;
          iXLWorksheet.ColumnWidth = 2.57;
          iXLWorksheet.Columns(1, 1).Width = 5.14 * 0.7;

          iXLWorksheet.Style.Font.Bold = true;
          iXLWorksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
          iXLWorksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
        else
        {
          iXLWorksheet = xLWorkbook.Worksheet("Schedule");
        }


        for (i = 0; i < c.Teams.Count; i++)
        {
          teamSchedule = c.Teams[i].Schedule;
          int row = i / 6;
          int column = i % 6;

          var range = iXLWorksheet.Range(1, (i * 26) + 3, 1, (i * 26) + 26).Merge();
          range.Value = c.Teams[i].TeamName;
          range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
          iXLWorksheet.Row(1 + row * 8).Height = 31.5 * 0.70;
          iXLWorksheet.Cell(1, (i * 26) + 3).Hyperlink = new XLHyperlink(c.Teams[i].Url);
          iXLWorksheet.Cell(1, (i * 26) + 3).Style.Font.SetFontSize(25);
          range.Style.Font.Bold = true;
          range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
          range.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);

          for (m = 0; m < teamSchedule.Count; m++)
          {
            iXLWorksheet.Row((m + 1) * 8 + 2).Style.Fill.BackgroundColor = XLColor.LightGray;

            //MATCH DAY
            var mdCell = iXLWorksheet.Cell(m * 8 + 6, 1);
            mdCell.Style.Font.SetFontSize(14);
            mdCell.Value = (m + 1).ToString();
            mdCell.Style.Font.Bold = true;
            mdCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            mdCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            mdCell.Style.Fill.BackgroundColor = XLColor.FromArgb(193, 239, 255);
            //MATCH DATE
            if (teamSchedule[m].Date.HasValue)
            {
              var mdate = teamSchedule[m].Date.Value.ToString("dd.MM.yyyy");
              var mdatecell = iXLWorksheet.Cell(m * 8 + 9, i * 26 + 2);
              mdatecell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
              mdatecell.Style.Font.SetFontColor(XLColor.FromArgb(15, 26, 62));
              mdatecell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
              mdatecell.Style.Font.Bold = true;
              mdatecell.Style.Font.SetFontSize(8);
              mdatecell.Style.Font.Italic = true;
              mdatecell.WorksheetColumn().Width = 8.71;
              mdatecell.Value = mdate;
              mdatecell.Hyperlink = new XLHyperlink(teamSchedule[m].MatchReportUrl);

              var mtime = teamSchedule[m].Date.Value.ToString("HH:mm");
              var mtimecell = iXLWorksheet.Cell(m * 8 + 10, i * 26 + 2);
              mtimecell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
              mtimecell.Style.Font.SetFontColor(XLColor.FromArgb(15, 26, 62));
              mtimecell.Style.Font.Bold = true;
              mtimecell.Style.Font.SetFontSize(8);
              mtimecell.Style.Font.Italic = true;
              mtimecell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
              mtimecell.WorksheetColumn().Width = 8.71;
              mtimecell.Value = mtime;
              mtimecell.Hyperlink = new XLHyperlink(teamSchedule[m].MatchReportUrl);
            }

            var sc = iXLWorksheet.Cell((m * 8 + 2), 3 + i * 26);

            //HOME
            var scr = sc.CellBelow();

            scr.WorksheetRow().Height = 15.075 * 0.70;
            sc = sc.CellBelow();
            sc.WorksheetRow().Height = 15.075 * 0.70;

            if (updateMode == false)
            {
              scr = iXLWorksheet.Cell(8 * m + 3, 26 * i + 3);
              matchHomeRange.CopyTo(scr);
            }

            for (int j = 0; j < 6; j++)
            {
              for (int k = 0; k < 10; k++)
              {
                #region home
                linkedTeam = c.Teams.Where(t => t.TeamId == teamSchedule[m].HomeTeam.TeamId).FirstOrDefault();
                linkedTeamIndex = c.Teams.IndexOf(linkedTeam);

                if (linkedTeam != null)
                {
                  dbrow = linkedTeamIndex / 6;
                  dbcolumn = linkedTeamIndex % 6;
                  scr = iXLWorksheet.Cell(8 * m + 4 + j, 26 * i + 3 + k);
                  scr.WorksheetRow().Height = 15.75 * 0.70;
                  if (updateMode == true)
                  {
                    if (DateTime.Now <= teamSchedule[m].Date)
                      scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                    else
                    {
                      if (scr.FormulaA1 != "")
                      {
                        scr.Value = scr.ValueCached;
                        scr.FormulaA1 = null;
                      }
                    }
                  }
                  else
                    scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                }

                #endregion

                #region away
                linkedTeam = c.Teams.Where(t => t.TeamId == teamSchedule[m].VisitingTeam.TeamId).FirstOrDefault();
                linkedTeamIndex = c.Teams.IndexOf(linkedTeam);

                if (linkedTeam != null)
                {
                  dbrow = linkedTeamIndex / 6;
                  dbcolumn = linkedTeamIndex % 6;
                  scr = iXLWorksheet.Cell(8 * m + 4 + j, 26 * i + 14 + k);
                  scr.WorksheetRow().Height = 15.75 * 0.70;
                  if (updateMode == true)
                  {
                    if (DateTime.Now <= teamSchedule[m].Date)
                      scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                    else
                    {
                      if (scr.FormulaA1 != "")
                      {
                        scr.Value = scr.ValueCached;
                        scr.FormulaA1 = null;
                      }

                    }
                  }
                  else
                    scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                }
                #endregion
              }
            }

            sc = sc.CellBelow();
            sc = iXLWorksheet.Cell(8 * m + 10, 26 * i + 3);

            sc.Style.Font.SetFontSize(10);
            var cellRange = iXLWorksheet.Cell(sc.Address.RowNumber, sc.Address.ColumnNumber);
            if (teamSchedule[m].HomeTeam.Url != null)
              sc.Hyperlink = new XLHyperlink(teamSchedule[m].HomeTeam.Url);
            if (teamSchedule[m].Date.HasValue)
              sc.Value = teamSchedule[m].HomeTeam.TeamName;
            else
              sc.Value = "" + " - " + teamSchedule[m].HomeTeam.TeamName;

            sc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            var val = iXLWorksheet.Cell(8 * m + 5, 13).Value;

            //AWAY
            sc = iXLWorksheet.Cell((m * 8 + 2), 3 + i * 26 + 11);
            scr = sc.CellBelow();
            scr.WorksheetRow().Height = 15.075 * 0.70;
            sc = sc.CellBelow();
            sc.WorksheetRow().Height = 15.075 * 0.70;


            sc = iXLWorksheet.Cell(8 * m + 10, 26 * i + 14);
            sc.WorksheetRow().Height = 15.075 * 0.70;
            sc.Style.Font.SetFontSize(10);
            cellRange = iXLWorksheet.Cell(sc.Address.RowNumber, sc.Address.ColumnNumber);
            cellRange.Value = teamSchedule[m].VisitingTeam.TeamName;
            if (teamSchedule[m].VisitingTeam.Url != null)
              sc.Hyperlink = new XLHyperlink(teamSchedule[m].VisitingTeam.Url);

            sc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            #region RESULT
            //RESULT
            string res = teamSchedule[m].Result;
            string hg = res.Split(':')[0];
            string ag = res.Split(':')[1].Trim();
            if (ag.Contains(" "))
              ag = ag.Substring(0, ag.IndexOf(" ")).Trim();
            var hgCell = iXLWorksheet.Cell((8 * m) + 6, 26 * (i + 1) - 13);
            var agCell = iXLWorksheet.Cell((8 * m) + 6, 26 * (i + 1) - 2);
            hgCell.Value = hg;
            hgCell.Hyperlink = new XLHyperlink(teamSchedule[m].MatchReportUrl);
            agCell.Value = ag;
            agCell.Hyperlink = new XLHyperlink(teamSchedule[m].MatchReportUrl);

            hgCell.Style.Font.SetFontSize(14);
            hgCell.Style.Font.SetFontColor(XLColor.Black);

            agCell.Style.Font.SetFontSize(14);
            agCell.Style.Font.SetFontColor(XLColor.Black);
            if (hg != "-" & ag != "-")
            {
              if (teamSchedule[m].HomeTeam.TeamId == c.Teams[i].TeamId)
              {
                if (int.Parse(hg) > int.Parse(ag))
                {
                  hgCell.Style.Fill.BackgroundColor = XLColor.Green;
                  agCell.Style.Fill.BackgroundColor = XLColor.Green;
                  hgCell.Style.Font.FontColor = XLColor.White;
                  agCell.Style.Font.FontColor = XLColor.White;
                }
                else if (int.Parse(hg) == int.Parse(ag))
                {
                  hgCell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 240);
                  agCell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 240);
                }
                else
                {
                  hgCell.Style.Fill.BackgroundColor = XLColor.Red;
                  agCell.Style.Fill.BackgroundColor = XLColor.Red;
                  hgCell.Style.Font.FontColor = XLColor.White;
                  agCell.Style.Font.FontColor = XLColor.White;
                }
              }
              else
              {
                if (int.Parse(hg) > int.Parse(ag))
                {
                  hgCell.Style.Fill.BackgroundColor = XLColor.Red;
                  agCell.Style.Fill.BackgroundColor = XLColor.Red;
                  hgCell.Style.Font.FontColor = XLColor.White;
                  agCell.Style.Font.FontColor = XLColor.White;
                }
                else if (int.Parse(hg) == int.Parse(ag))
                {
                  hgCell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 240);
                  agCell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 240);
                }
                else
                {
                  hgCell.Style.Fill.BackgroundColor = XLColor.Green;
                  agCell.Style.Fill.BackgroundColor = XLColor.Green;
                  hgCell.Style.Font.FontColor = XLColor.White;
                  agCell.Style.Font.FontColor = XLColor.White;
                }
              }
            }
            else
            {
              hgCell.Style.Fill.BackgroundColor = XLColor.LightGray;
              agCell.Style.Fill.BackgroundColor = XLColor.LightGray;
              hgCell.Style.Font.FontColor = XLColor.Black;
              agCell.Style.Font.FontColor = XLColor.Black;
            }


            hgCell.Style.Font.Bold = true;
            agCell.Style.Font.Bold = true;

            #endregion
          }
        }

      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }

    }

    private static void CreateDbStylesheet(Competition c, XLWorkbook xLWorkbook)
    {
      IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add("DB");

      iXLWorksheet.ColumnWidth = 2;
      iXLWorksheet.Style.Font.SetFontSize(8);

      Dictionary<string, string> ranges = new Dictionary<string, string>();
      ranges.Add("B", "K");
      ranges.Add("M", "V");
      ranges.Add("X", "AG");
      ranges.Add("AI", "AR");
      ranges.Add("AT", "BC");
      ranges.Add("BE", "BN");

      string[] headers = new string[] { "UU", "GU", "AU", "MU", "MPG", "0", "GU", "AU", "GU", "AU" };

      XLColor[][] colormatrix = new XLColor[10][];

      colormatrix[0] = new XLColor[] {
        XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0),
        XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0) };

      colormatrix[1] = new XLColor[] {
        XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(146, 208, 80), XLColor.FromArgb(146, 208, 80), XLColor.FromArgb(146, 208, 80), XLColor.FromArgb(146, 208, 80),
        XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(137, 207, 240) };

      colormatrix[2] = new XLColor[] {
        XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105),
        XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(144, 238, 144), XLColor.FromArgb(144, 238, 144) };

      colormatrix[3] = new XLColor[] {
        XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0),
        XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(137, 207, 240) };

      colormatrix[4] = new XLColor[] {
        XLColor.FromArgb(144, 238, 144), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255),
        XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105) };

      colormatrix[5] = new XLColor[] {
        XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255),
        XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(137, 207, 240) };

      colormatrix[6] = new XLColor[] {
        XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(255, 255, 255),
        XLColor.FromArgb(189, 146, 222), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0) };

      int numberOfRows = ranges.Count;

      IXLCell scr = null;

      for (int i = 0; i < c.Teams.Count; i++)
      {
        int row = i / 6;
        int column = i % 6;
        KeyValuePair<string, string> kvp = ranges.ElementAt(column);
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Merge().Value = c.Teams[i].TeamName;
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        iXLWorksheet.Row(1 + row * 8).Height = 24.75 * 0.75;
        iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString()).Hyperlink = new XLHyperlink(c.Teams[i].Url);// URL??
        iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString()).Style.Font.SetFontSize(12);
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Font.Bold = true;
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);

        var sc = iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString());

        scr = sc.CellBelow();
        sc = sc.CellBelow();
        for (int l = 0; l < headers.Length; l++)
        {
          scr.Value = headers[l];
          scr.Style.Fill.BackgroundColor = colormatrix[0][l];
          scr = scr.CellRight();
        }

        for (int j = 0; j < 6; j++)
        {
          scr = sc.CellBelow();
          sc = sc.CellBelow();
          for (int k = 0; k < 10; k++)
          {
            scr.Style.Fill.BackgroundColor = colormatrix[j + 1][k];
            scr.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            scr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            scr.Value = 0;
            scr.FormulaA1 = "'" + Application.StartupPath + Helper.GetCompetitionArchiveDirectoryName(c) + "\\[" + c.Teams[i].TeamName.NormalizeString() + ".xlsx]" + c.Teams[i].TeamName.NormalizeString().Left(31) + "'!" + iXLWorksheet.Cell(237 + j, 8 + k).Address.ToString();
            scr = scr.CellRight();
          }
        }
      }
    }
  }
}

