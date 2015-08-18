using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

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

      XLWorkbook xLWorkbook = new XLWorkbook();
      IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add("test");

      iXLWorksheet.Cell("A1").Value = (t.TeamName);
      iXLWorksheet.Cell("A1").Style.Font.FontSize = (16.0);
      iXLWorksheet.Cell("A1").Style.Font.FontColor = (XLColor.DarkBlue);
      iXLWorksheet.Cell("A1").Hyperlink = (new XLHyperlink("http://www.transfermarkt.com/" + t.Url));

      List<Player> lineupPlayers = t.Players.Where(p => p.Lineup == Player.LineUpStatus.YES).ToList();

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
          if (player.Statistics[0] != null)
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
              iXLWorksheet.Cell("O" + (i * 5 + l + 2).ToString()).Value = statistics.Assists.Replace("-", "1");
              iXLWorksheet.Cell("P" + (i * 5 + l + 2).ToString()).Value = statistics.MinutesPlayed.Replace("-",
                "0.000001");
              iXLWorksheet.Cell("Q" + (i * 5 + l + 2).ToString()).Value = statistics.MinutesPerGoal.Replace("-",
                "0.000001");
            }
          }
        }
        xLWorkbook.SaveAs(excelFilename);
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
      }
    }

    public static void CreateCompetitionArchive(string excelFileName, Competition c)
    {
      XLWorkbook xLWorkbook = new XLWorkbook();
      CreateDbStylesheet(c, xLWorkbook);
      CreateScheduleStylesheet(c, xLWorkbook);
      xLWorkbook.SaveAs(excelFileName);
      return;
    }

    private static void CreateScheduleStylesheet(Competition c, XLWorkbook xLWorkbook)
    {
      IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add("Schedule");

      iXLWorksheet.ColumnWidth = 2;
      iXLWorksheet.Style.Font.SetFontSize(10);

      //Dictionary<string, string> ranges = new Dictionary<string, string>();
      //ranges.Add("B", "K");
      //ranges.Add("M", "V");
      //ranges.Add("X", "AG");
      //ranges.Add("AI", "AR");
      //ranges.Add("AT", "BC");
      //ranges.Add("BE", "BN");

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

      //int numberOfRows = ranges.Count;



      for (int i = 0; i < c.Teams.Count; i++)
      {
        List<Match> teamSchedule = c.Teams[i].Schedule;
        int row = i / 6;
        int column = i % 6;

        //iXLWorksheet.Cell(1, (i * 26) + 3).Value = c.Teams[i].TeamName;
        //KeyValuePair<string, string> kvp = ranges.ElementAt(column);
        var range = iXLWorksheet.Range(1, (i * 26) + 3, 1, (i * 26) + 26).Merge();
        range.Value = c.Teams[i].TeamName;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        iXLWorksheet.Row(1 + row * 8).Height = 20;
        iXLWorksheet.Cell(1, (i * 26) + 3).Hyperlink = new XLHyperlink(c.Teams[i].Url);
        iXLWorksheet.Cell(1, (i * 26) + 3).Style.Font.SetFontSize(12);
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);





        for (int m = 0; m < teamSchedule.Count; m++)
        {

          var mdCell = iXLWorksheet.Cell(m * 8 + 6, 1);
          mdCell.Style.Font.SetFontSize(16);
          mdCell.Value = (m + 1).ToString();

          var sc = iXLWorksheet.Cell((m * 8 + 3), 3 + i * 26);

          //HOME
          var scr = sc.CellBelow();
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
              scr = scr.CellRight();
            }
          }

          sc = sc.CellBelow();

          //var range = iXLWorksheet.Range(sc.Address.RowNumber, sc.Address.ColumnNumber, sc.Address.RowNumber, sc.Address.ColumnNumber + 9).Merge();
          //range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
          //range.Value = teamSchedule[m].HomeTeam.TeamName;
          //sc.Hyperlink = new XLHyperlink(teamSchedule[m].HomeTeam.Url);

          
       
          var cellRange = iXLWorksheet.Cell(sc.Address.RowNumber, sc.Address.ColumnNumber);
          sc.Hyperlink = new XLHyperlink(teamSchedule[m].HomeTeam.Url);
          if (teamSchedule[m].Date.HasValue)
            sc.Value = teamSchedule[m].Date.Value.ToString("dd.MM.yyyy") + " - " + teamSchedule[m].HomeTeam.TeamName;
          else
            sc.Value = "" + " - " + teamSchedule[m].HomeTeam.TeamName;



          //AWAY
          sc = iXLWorksheet.Cell((m * 8 + 3), 3 + i * 26 + 11);
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
              scr = scr.CellRight();
            }
          }

          sc = sc.CellBelow();

          //range = iXLWorksheet.Range(sc.Address.RowNumber, sc.Address.ColumnNumber, sc.Address.RowNumber, sc.Address.ColumnNumber + 9).Merge();
          //range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
          //range.Value = teamSchedule[m].VisitingTeam.TeamName;
          //sc.Hyperlink = new XLHyperlink(teamSchedule[m].VisitingTeam.Url);

          cellRange = iXLWorksheet.Cell(sc.Address.RowNumber, sc.Address.ColumnNumber);
          cellRange.Value = teamSchedule[m].VisitingTeam.TeamName;
          sc.Hyperlink = new XLHyperlink(teamSchedule[m].VisitingTeam.Url);
        }
      }
    }

    private static void CreateDbStylesheet(Competition c, XLWorkbook xLWorkbook)
    {
      IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add("DB");

      iXLWorksheet.ColumnWidth = 2;
      iXLWorksheet.Style.Font.SetFontSize(10);

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



      for (int i = 0; i < c.Teams.Count; i++)
      {
        int row = i / 6;
        int column = i % 6;
        KeyValuePair<string, string> kvp = ranges.ElementAt(column);
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Merge().Value = c.Teams[i].TeamName;
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        iXLWorksheet.Row(1 + row * 8).Height = 20;
        iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString()).Hyperlink = new XLHyperlink(c.Teams[i].Url);
        iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString()).Style.Font.SetFontSize(12);
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Font.Bold = true;
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);

        var sc = iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString());

        var scr = sc.CellBelow();
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
            scr = scr.CellRight();
          }
        }
      }
    }
  }
}
