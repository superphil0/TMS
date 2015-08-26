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
            XLWorkbook xLWorkbook = new XLWorkbook(excelFilename);
            var existing = xLWorkbook.Worksheets.Where(ws => ws.Name == t.TeamName).FirstOrDefault();
            int index = existing.Position;
            if (existing != null)
                existing.Delete();
            IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add(t.TeamName);
            iXLWorksheet.Position = index;

            iXLWorksheet.Cell("A1").Value = (t.TeamName);
            iXLWorksheet.Cell("A1").Style.Font.FontSize = (16.0);
            iXLWorksheet.Cell("A1").Style.Font.FontColor = (XLColor.DarkBlue);
            iXLWorksheet.Cell("A1").Hyperlink = (new XLHyperlink("http://www.transfermarkt.com/" + t.Url));

            List<Player> lineupPlayers = t.Players.Where(p => p.Lineup == Player.LineUpStatus.YES).ToList();

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

                CreateSummaryTable(iXLWorksheet.Cell("H237"));
                //xLWorkbook.SaveAs(excelFilename);
                xLWorkbook.Save();
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
            CreateScheduleStylesheet(c, xLWorkbook, false);
            xLWorkbook.SaveAs(excelFileName);
        }

        public static void UpdateCompetitionArchive(string excelFileName, Competition c)
        {
            try
            {
                XLWorkbook xLWorkbook = new XLWorkbook(excelFileName);
                xLWorkbook.Worksheet("Schedule").Delete();
                CreateScheduleStylesheet(c, xLWorkbook, true);
                xLWorkbook.Save();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("being used"))
                {
                    MessageBox.Show("Zatvori arhivu pa pokusaj opet!");
                }
                else
                {
                    MessageBox.Show("Greska!");
                }
            }
        }

        private static void CreateSummaryTable(IXLCell scr)
        {
            XLWorkbook template = new XLWorkbook("cache\\arhiva\\Template.xlsx");
            var sheet = template.Worksheet("Template");

            var summaryTableRange = sheet.Range(237, 8, 242, 17);

            summaryTableRange.CopyTo(scr);

            //    string[] headers = new string[] { "UU", "GU", "AU", "MU", "MPG", "0", "GU", "AU", "GU", "AU" };

            //    XLColor[][] colormatrix = new XLColor[10][];

            //    colormatrix[0] = new XLColor[] {
            //XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0),
            //XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0), XLColor.FromArgb(255, 192, 0) };

            //    colormatrix[1] = new XLColor[] {
            //XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(146, 208, 80), XLColor.FromArgb(146, 208, 80), XLColor.FromArgb(146, 208, 80), XLColor.FromArgb(146, 208, 80),
            //XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(137, 207, 240) };

            //    colormatrix[2] = new XLColor[] {
            //XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105),
            //XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(144, 238, 144), XLColor.FromArgb(144, 238, 144) };

            //    colormatrix[3] = new XLColor[] {
            //XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0),
            //XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(137, 207, 240) };

            //    colormatrix[4] = new XLColor[] {
            //XLColor.FromArgb(144, 238, 144), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255),
            //XLColor.FromArgb(0, 176, 80), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 201, 105) };

            //    colormatrix[5] = new XLColor[] {
            //XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255),
            //XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(255, 201, 105), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(137, 207, 240), XLColor.FromArgb(137, 207, 240) };

            //    colormatrix[6] = new XLColor[] {
            //XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(169, 208, 142), XLColor.FromArgb(255, 255, 255),
            //XLColor.FromArgb(189, 146, 222), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(255, 255, 255), XLColor.FromArgb(191, 143, 0), XLColor.FromArgb(191, 143, 0) };

            //    var sc = scr;

            //    for (int l = 0; l < headers.Length; l++)
            //    {
            //        scr.Value = headers[l];
            //        scr.Style.Fill.BackgroundColor = colormatrix[0][l];
            //        scr.Style.Font.SetFontSize(8);
            //        scr = scr.CellRight();
            //    }

            //    for (int j = 0; j < 6; j++)
            //    {
            //        scr = sc.CellBelow();
            //        scr.WorksheetRow().Height = 15.075 * 0.70;
            //        sc = sc.CellBelow();
            //        sc.WorksheetRow().Height = 15.075 * 0.70;
            //        for (int k = 0; k < 10; k++)
            //        {
            //            scr.Style.Fill.BackgroundColor = colormatrix[j + 1][k];
            //            scr.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            //            scr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            if (j == 2 && (k == 6 || k == 7))
            //                scr.Style.Font.FontColor = XLColor.Red;

            //            scr.Style.Font.SetFontSize(8);
            //            scr.Value = 0;
            //            scr.SetFormulaA1("M4");
            //            scr = scr.CellRight();
            //        }
            //    }
        }

        private static void CreateScheduleStylesheet(Competition c, XLWorkbook xLWorkbook, bool updateMode)
        {
            var dbWorksheet = xLWorkbook.Worksheet("DB");
            try
            {
                IXLWorksheet iXLWorksheet = xLWorkbook.Worksheets.Add("Schedule");
                iXLWorksheet.Position = 2;
                iXLWorksheet.ColumnWidth = 3.57 * 0.7;
                iXLWorksheet.Columns(1, 1).Width = 5.14 * 0.7;
                //iXLWorksheet.Style.Font.SetFontSize(8);

                iXLWorksheet.Style.Font.Bold = true;
                iXLWorksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                iXLWorksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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


                #region formulas

                iXLWorksheet.Range("X3", "Z9").Style.Font.Bold = true;
                iXLWorksheet.Range("X3", "Z9").Style.Font.SetFontSize(8.5);
                iXLWorksheet.Range("X3", "Z9").Style.Font.FontColor = XLColor.FromArgb(0, 32, 96);
                iXLWorksheet.Range("X3", "Z9").Style.Font.SetFontName("Segoe UI Black");
                //Segoe UI Black

                //0, 32, 96
                #region headers
                //headers
                var cellX3 = iXLWorksheet.Cell(3, 24);
                cellX3.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 201, 105);
                cellX3.SetFormulaA1("SUM((((Q5 * 100) / ((F5 / E5) * 100)) - P5))");

                var cellY3 = iXLWorksheet.Cell(3, 25);
                cellY3.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 255);
                cellY3.SetFormulaA1("SUM((((((D5+I4)/F5)+((D4/F4)*K5)+(D4+I4)/F4)+(I5+G4)))/5)/S4");

                var cellZ3 = iXLWorksheet.Cell(3, 26);
                cellZ3.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 255);
                cellZ3.SetFormulaA1("SUM((((((O5+T4)/Q5)+((O4/Q4)*V5)+(O4+T4)/Q4)+(T5+R4)))/5)/H4");
                #endregion headers

                #region row1

                var cellX4 = iXLWorksheet.Cell(4, 24);
                cellX4.Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                cellX4.SetFormulaA1("SUM((((Q4*100)/((F4/E4)*100))-P4))");

                var cellY4 = iXLWorksheet.Cell(4, 25);
                cellY4.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellY4.SetFormulaA1("SUM((G7+H8+G4+K5+K5+K5))/6");

                var cellZ4 = iXLWorksheet.Cell(4, 26);
                cellZ4.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellZ4.SetFormulaA1("SUM((R7+S8+R4+V5+V5+V5))/6");

                var cellAA4 = iXLWorksheet.Cell(4, 27);
                cellAA4.SetFormulaA1("SUM(Y4-Z4)");
                cellAA4.Style.Font.Bold = false;
                cellAA4.Style.Font.SetFontSize(8);


                #endregion row1

                #region row2     

                var cellY5 = iXLWorksheet.Cell(5, 25);
                cellY5.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellY5.SetFormulaA1("SUM(((G9+G9+H8+G7+G4+K5+K5+(L5/2)+I5))/7)/S4");

                var cellZ5 = iXLWorksheet.Cell(5, 26);
                cellZ5.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellZ5.SetFormulaA1("SUM(((R9+R9+S8+R7+R4+V5+V5+(W5/2)+T5))/7)/H4");

                var cellAA5 = iXLWorksheet.Cell(5, 27);
                cellAA5.SetFormulaA1("SUM(Y5-Z5)");
                cellAA5.Style.Font.Bold = false;
                cellAA5.Style.Font.SetFontSize(8);

                #endregion row2

                #region row3     

                var cellY6 = iXLWorksheet.Cell(6, 25);
                cellY6.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellY6.SetFormulaA1("SUM((K5+K5+E7+G8+H8)/2)/(S4+S4)");

                var cellZ6 = iXLWorksheet.Cell(6, 26);
                cellZ6.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellZ6.SetFormulaA1("SUM((V5+V5+P7+R8+S8)/2)/(H4+H4)");

                var cellAA6 = iXLWorksheet.Cell(6, 27);
                cellAA6.SetFormulaA1("SUM(Y6-Z6)");
                cellAA6.Style.Font.Bold = false;
                cellAA6.Style.Font.SetFontSize(8);
                #endregion row3

                #region row4     

                var cellY7 = iXLWorksheet.Cell(7, 25);
                cellY7.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellY7.SetFormulaA1("SUM((I5+I7+H8+G9+K5+K5)/3)/(S4+S4)");

                var cellZ7 = iXLWorksheet.Cell(7, 26);
                cellZ7.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellZ7.SetFormulaA1("SUM((T5+T7+S8+R9+V5+V5)/3)/(H4+H4)");

                var cellAA7 = iXLWorksheet.Cell(7, 27);
                cellAA7.SetFormulaA1("SUM(Y7-Z7)");
                cellAA7.Style.Font.Bold = false;
                cellAA7.Style.Font.SetFontSize(8);
                #endregion row4

                #region row5 

                var cellX8 = iXLWorksheet.Cell(8, 24);
                cellX8.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 201, 105);
                cellX8.SetFormulaA1("SUM((((Q5*100)/((F5/D5)*100))-O5))");

                var cellY8 = iXLWorksheet.Cell(8, 25);
                cellY8.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellY8.SetFormulaA1("SUM((G4+H8+I8)/2)/S4");

                var cellZ8 = iXLWorksheet.Cell(8, 26);
                cellZ8.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                cellZ8.SetFormulaA1("SUM((R4+S8+T8)/2)/H4");

                var cellAA8 = iXLWorksheet.Cell(8, 27);
                cellAA8.SetFormulaA1("SUM(Y8-Z8)");
                cellAA8.Style.Font.Bold = false;
                cellAA8.Style.Font.SetFontSize(8);

                #endregion row5

                #region row6 

                var cellX9 = iXLWorksheet.Cell(9, 24);
                cellX9.Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                cellX9.SetFormulaA1("SUM((((Q4*100)/((F4/D4)*100))-O4))");

                var cellY9 = iXLWorksheet.Cell(9, 25);
                cellY9.Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
                cellY9.SetFormulaA1("SUM(Y4:Y8)/5");

                var cellZ9 = iXLWorksheet.Cell(9, 26);
                cellZ9.Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
                cellZ9.SetFormulaA1("SUM(Z4:Z8)/5");

                var cellAA9 = iXLWorksheet.Cell(9, 27);
                cellAA9.SetFormulaA1("SUM(Y9-Z9)");
                cellAA9.Style.Font.Bold = false;
                cellAA9.Style.Font.SetFontSize(8);

                #endregion row6

                #region row7

                var cellZ10 = iXLWorksheet.Cell(10, 26);
                cellZ10.SetFormulaA1("SUM(I7+H9)-(T7+S9)");

                #endregion row7

                #endregion formulas

                for (int i = 0; i < c.Teams.Count; i++)
                {
                    List<Match> teamSchedule = c.Teams[i].Schedule;
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

                    for (int m = 0; m < teamSchedule.Count; m++)
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
                            mdatecell.WorksheetColumn().Width = 8;
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
                            mtimecell.WorksheetColumn().Width = 8;
                            mtimecell.Value = mtime;
                            mtimecell.Hyperlink = new XLHyperlink(teamSchedule[m].MatchReportUrl);
                        }



                        var sc = iXLWorksheet.Cell((m * 8 + 2), 3 + i * 26);

                        //HOME
                        var scr = sc.CellBelow();

                        scr.WorksheetRow().Height = 15.075 * 0.70;

                        sc = sc.CellBelow();

                        sc.WorksheetRow().Height = 15.075 * 0.70;

                        for (int l = 0; l < headers.Length; l++)
                        {
                            scr.Value = headers[l];
                            scr.Style.Fill.BackgroundColor = colormatrix[0][l];
                            scr.Style.Font.SetFontSize(8);
                            scr = scr.CellRight();
                        }

                        for (int j = 0; j < 6; j++)
                        {
                            scr = sc.CellBelow();
                            scr.WorksheetRow().Height = 15.075 * 0.70;
                            sc = sc.CellBelow();
                            sc.WorksheetRow().Height = 15.075 * 0.70;
                            for (int k = 0; k < 10; k++)
                            {
                                scr.Style.Fill.BackgroundColor = colormatrix[j + 1][k];
                                scr.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
                                scr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                if (j == 2 && (k == 6 || k == 7))
                                    scr.Style.Font.FontColor = XLColor.Red;

                                var linkedTeam = c.Teams.Where(t => t.TeamId == teamSchedule[m].HomeTeam.TeamId).FirstOrDefault();
                                int linkedTeamIndex = c.Teams.IndexOf(linkedTeam);

                                int dbrow = linkedTeamIndex / 6;
                                int dbcolumn = linkedTeamIndex % 6;

                                if (updateMode == true)
                                {
                                    if (DateTime.Now <= teamSchedule[m].Date)
                                        scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                                    else
                                    {
                                        scr.Value = dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Value;
                                        scr.FormulaA1 = null;
                                    }
                                }
                                else
                                    scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                                scr.Style.Font.SetFontSize(8);
                                scr = scr.CellRight();
                            }
                        }

                        sc = sc.CellBelow();

                        sc.WorksheetRow().Height = 15.075 * 0.70;

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
                        for (int l = 0; l < headers.Length; l++)
                        {
                            scr.Value = headers[l];
                            scr.Style.Fill.BackgroundColor = colormatrix[0][l];
                            scr.Style.Font.SetFontSize(8);
                            scr = scr.CellRight();
                        }

                        for (int j = 0; j < 6; j++)
                        {
                            scr = sc.CellBelow();
                            scr.WorksheetRow().Height = 15.075 * 0.70;
                            sc = sc.CellBelow();
                            sc.WorksheetRow().Height = 15.075 * 0.70;
                            for (int k = 0; k < 10; k++)
                            {
                                scr.Style.Fill.BackgroundColor = colormatrix[j + 1][k];
                                scr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                if (j == 2 && (k == 6 || k == 7))
                                    scr.Style.Font.FontColor = XLColor.Red;

                                var linkedTeam = c.Teams.Where(t => t.TeamId == teamSchedule[m].VisitingTeam.TeamId).FirstOrDefault();
                                int linkedTeamIndex = c.Teams.IndexOf(linkedTeam);

                                int dbrow = linkedTeamIndex / 6;
                                int dbcolumn = linkedTeamIndex % 6;

                                if (updateMode == true)
                                {
                                    if (DateTime.Now <= teamSchedule[m].Date)
                                        scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();
                                    else
                                    {
                                        scr.Value = dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Value;
                                        scr.FormulaA1 = null;
                                    }
                                }
                                else
                                    scr.FormulaA1 = "DB!" + dbWorksheet.Cell(dbrow * 8 + 3 + j, dbcolumn * 11 + 2 + k).Address.ToString();

                                scr.Style.Font.SetFontSize(8);

                                scr = scr.CellRight();
                            }
                        }

                        sc = sc.CellBelow();
                        sc.WorksheetRow().Height = 15.075 * 0.70;
                        sc.Style.Font.SetFontSize(10);
                        cellRange = iXLWorksheet.Cell(sc.Address.RowNumber, sc.Address.ColumnNumber);
                        cellRange.Value = teamSchedule[m].VisitingTeam.TeamName;
                        if (teamSchedule[m].VisitingTeam.Url != null)
                            sc.Hyperlink = new XLHyperlink(teamSchedule[m].VisitingTeam.Url);

                        sc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //RESULT
                        string res = teamSchedule[m].Result;
                        string hg = res.Split(':')[0];
                        string ag = res.Split(':')[1].Trim();
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


                        hgCell.Style.Font.Bold = true;
                        agCell.Style.Font.Bold = true;

                        #region forumulas

                        //header
                        var cell = iXLWorksheet.Cell((8 * m) + 3, 26 * (i + 1) - 2);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellX3.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 3, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY3.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 3, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ3.CopyTo(cell);

                        //red 1
                        cell = iXLWorksheet.Cell((8 * m) + 4, 26 * (i + 1) - 2);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellX4.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 4, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY4.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 4, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ4.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 4, 26 * (i + 1) + 1);
                        cellAA4.CopyTo(cell);

                        //red 2                        
                        cell = iXLWorksheet.Cell((8 * m) + 5, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY5.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 5, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ5.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 5, 26 * (i + 1) + 1);
                        cellAA5.CopyTo(cell);

                        //red 3                        
                        cell = iXLWorksheet.Cell((8 * m) + 6, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY6.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 6, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ6.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 6, 26 * (i + 1) + 1);
                        cellAA6.CopyTo(cell);

                        //red 4
                        cell = iXLWorksheet.Cell((8 * m) + 7, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY7.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 7, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ7.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 7, 26 * (i + 1) + 1);
                        cellAA7.CopyTo(cell);

                        //red 5
                        cell = iXLWorksheet.Cell((8 * m) + 8, 26 * (i + 1) - 2);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellX8.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 8, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY8.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 8, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ8.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 8, 26 * (i + 1) + 1);
                        cellAA8.CopyTo(cell);

                        //red 6
                        cell = iXLWorksheet.Cell((8 * m) + 9, 26 * (i + 1) - 2);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellX9.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 9, 26 * (i + 1) - 1);
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellY9.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 9, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ9.CopyTo(cell);

                        cell = iXLWorksheet.Cell((8 * m) + 9, 26 * (i + 1) + 1);
                        cellAA9.CopyTo(cell);

                        //red 7
                        cell = iXLWorksheet.Cell((8 * m) + 10, 26 * (i + 1));
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        cellZ10.CopyTo(cell);


                        #endregion formulas
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
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
                xLWorkbook.Worksheets.Add(c.Teams[i].TeamName.ToString());
                int row = i / 6;
                int column = i % 6;
                KeyValuePair<string, string> kvp = ranges.ElementAt(column);
                iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Merge().Value = c.Teams[i].TeamName;
                iXLWorksheet.Range(kvp.Key + (1 + row * 8).ToString(), kvp.Value + (1 + row * 8).ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                iXLWorksheet.Row(1 + row * 8).Height = 24.75 * 0.75;
                iXLWorksheet.Cell(kvp.Key + (1 + row * 8).ToString()).Hyperlink = new XLHyperlink(c.Teams[i].Url);
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
                        scr.FormulaA1 = "'" + c.Teams[i].TeamName + "'!" + iXLWorksheet.Cell(237 + j, 8 + k).Address.ToString();
                        scr = scr.CellRight();
                    }
                }
            }
        }
    }
}
