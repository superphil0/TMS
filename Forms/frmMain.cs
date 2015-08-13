using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMS.Properties;
namespace TMS
{
    public partial class frmMain : Form
    {
        #region Attributes

        private const int COMMON_CHARACTER_MATCH_PERCENTAGE = 90;
        private Player CurrentPlayer;
        private bool _generatingInProgress = false;
        private bool _playerLoadingInProgress = false;
        private List<Game> games = new List<Game>();
        private List<Game> gamesUpcoming = new List<Game>();
        private List<Game> gamesLive = new List<Game>();
        private List<Game> gamesCompleted = new List<Game>();

        public GameLineups SelectedGameLinups;
        public Game SelectedGame;
        public List<string> LineupPlayers = new List<string>();
        private List<Country> _countries = new List<Country>();
        private List<Country> _cachedCountries = new List<Country>();
        private List<Competition> _cachedCompetitions = new List<Competition>();
        private List<Team> _cachedTeams = new List<Team>();
        private List<Player> _cachedStats = new List<Player>();
        private int _counter;
        private Team _selectedTeam;
        private Competition _selectedCompetition;
        private Country _selectedCountry;

        private Task _geTask;
        #endregion Attributes

        #region Init

        public frmMain()
        {
            this.InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this._cachedCompetitions = this.GetCachedCompetitions();
            this._cachedTeams = this.GetCachedTeams();
            this._cachedCountries = this.GetCachedCountries();
            this.cbTeams.DataSource = this._cachedTeams;
            this.cbTeams.DisplayMember = "TeamName";
            this.cbTeams.ValueMember = "TeamId";
            this.cbTeams.SelectedIndex = -1;
            LoadCountriesAsync();
            LoadDocuments();
            StartLivescoreFeedAsync();
            tmrReload.Start();

        }

        private List<Team> GetCachedTeams()
        {
            List<Team> list = new List<Team>();

            try
            {
                if (File.Exists("cache\\teams.txt"))
                {
                    StreamReader streamReader = new StreamReader("cache\\teams.txt");
                    while (!streamReader.EndOfStream)
                    {
                        string text = streamReader.ReadLine();
                        string[] array = text.Split(new char[]
                {
                  '|'
                });

                        string alternativeName = null;
                        if (array.Length > 4)
                            alternativeName = array[4];

                        list.Add(new Team
                        {
                            TeamId = int.Parse(array[2]),
                            TeamName = array[3],
                            CompetitionId = array[0],
                            CompetitionName = array[1],
                            AlternativeName = alternativeName
                        });
                    }
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            return list;
        }

        private List<Country> GetCachedCountries()
        {
            List<Country> list = new List<Country>();

            try
            {
                if (File.Exists("cache\\countries.txt"))
                {
                    StreamReader streamReader = new StreamReader("cache\\countries.txt");
                    while (!streamReader.EndOfStream)
                    {
                        string text = streamReader.ReadLine();
                        string[] array = text.Split(new char[]
                {
                  '|'
                });
                        list.Add(new Country
                        {
                            CountryId = int.Parse(array[0]),
                            CountryName = array[1]
                        });
                    }
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            return list;
        }

        private List<Competition> GetCachedCompetitions()
        {
            List<Competition> list = new List<Competition>();
            try
            {

                if (File.Exists("cache\\competitions.txt"))
                {
                    StreamReader streamReader = new StreamReader("cache\\competitions.txt");
                    while (!streamReader.EndOfStream)
                    {
                        string text = streamReader.ReadLine();
                        string[] array = text.Split(new char[]
            {
              '|'
            });
                        list.Add(new Competition
                        {
                            CompetitionId = array[2],
                            CompetitionName = array[3],
                            CompetitionCountryId = int.Parse(array[0])
                        });
                    }
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }


            return list;
        }

        #endregion Init

        #region Countries

        private void lbCountries_Click(object sender, EventArgs e)
        {
            LoadCompetitionsAsync();
        }

        private async Task LoadCountriesAsync()
        {
            try
            {
                this.pbLoadingCountries.Visible = true;
                this.lbCountries.DataSource = null;
                this.cbCountries.DataSource = null;
                this.lbTeams.DataSource = null;
                this.lbCompetition.DataSource = null;
                this.lbTeams.DataSource = null;
                this.dgvPlayers.DataSource = null;
                lbCountries.Enabled = false;


                List<Country> cachedCountries = this._cachedCountries;
                if (cachedCountries.Count == 0)
                {
                    this._countries = await DataLoader.LoadCountriesAsync();
                    DirectoryInfo directoryInfo = new DirectoryInfo("cache");
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    StreamWriter streamWriter = new StreamWriter("cache\\countries.txt", true);
                    foreach (Country c in _countries)
                    {
                        if ((
                          from cc in cachedCountries
                          where cc.CountryId == c.CountryId
                          select cc).ToList<Country>().Count == 0)
                        {
                            this._cachedCountries.Add(c);
                            streamWriter.WriteLine(c.CountryId.ToString() + "|" + c.CountryName);
                        }
                    }
                    streamWriter.Close();
                }
                this._countries = cachedCountries;

                this.pbLoadingCountries.Visible = false;
                this.lbCountries.DataSource = this._countries;
                this.lbCountries.DisplayMember = "CountryName";
                this.lbCountries.ValueMember = "CountryId";
                this.cbCountries.DataSource = this._countries;
                this.cbCountries.DisplayMember = "CountryName";
                this.cbCountries.ValueMember = "CountryId";
                this.cbCountries.Text = "";
                this.cbCountries.Focus();
                this.cbCountries.Select();

                lbCountries.Enabled = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void cbCountries_TextUpdate(object sender, EventArgs e)
        {
            string text = this.cbCountries.Text;
            int selectedIndex = this.cbCountries.FindString(this.cbCountries.Text);
            this.lbCountries.SelectedIndex = selectedIndex;
            this.cbCountries.Text = text;
            this.cbCountries.Select(this.cbCountries.Text.Length, 0);
        }

        private void cbCountries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                LoadCompetitionsAsync();
        }

        #endregion Countries

        #region Competitions

        private async Task LoadCompetitionsAsync()
        {
            try
            {
                this.pbLoadingCompetitions.Visible = true;
                this._selectedCountry = (Country)this.lbCountries.SelectedItem;
                this.lbTeams.DataSource = null;
                this.lbCompetition.DataSource = null;
                this.lbCompetition.DisplayMember = "CompetitionName";
                this.lbCompetition.ValueMember = "CompetitionId";
                this.lbTeams.DataSource = null;
                this.lbTeams.SelectedIndex = -1;
                this.cbCountries.Text = "";
                lbCountries.Enabled = false;

                List<Competition> list = (
                  from cc in this._cachedCompetitions
                  where cc.CompetitionCountryId == this._selectedCountry.CountryId
                  select cc).ToList<Competition>();

                if (list.Count == 0)
                {
                    this._selectedCountry.Competitions =
                      await DataLoader.LoadCompetitionsAsync(this._selectedCountry.CountryId);
                    DirectoryInfo directoryInfo = new DirectoryInfo("cache");
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    StreamWriter streamWriter = new StreamWriter("cache\\competitions.txt", true);
                    foreach (Competition c in _selectedCountry.Competitions)
                    {

                        if (c.CompetitionName.Equals("NATIONAL"))
                        {
                            StreamWriter sw = new StreamWriter("cache\\teams.txt", true);
                            foreach (Team t in c.Teams)
                            {
                                t.CompetitionName = c.CompetitionName;
                                if ((
                                  from cc in this._cachedTeams
                                  where cc.CompetitionId == t.CompetitionId && cc.TeamId == t.TeamId
                                  select cc).ToList<Team>().Count == 0)
                                {

                                    this._cachedTeams.Add(t);
                                    sw.WriteLine(string.Concat(new string[]
                        {
                         t.CompetitionId.ToString(),
                          "|",
                         t.CompetitionName,
                          "|",
                          t.TeamId.ToString(),
                          "|",
                          t.TeamName
                        }));

                                }
                            }
                            sw.Close();
                        }

                        if ((
                          from cc in this._cachedCompetitions
                          where cc.CompetitionId == c.CompetitionId
                          select cc).ToList<Competition>().Count == 0)
                        {
                            list.Add(c);
                            this._cachedCompetitions.Add(c);
                            streamWriter.WriteLine(string.Concat(new string[]
                    {
                      this._selectedCountry.CountryId.ToString(),
                      "|",
                      this._selectedCountry.CountryName,
                      "|",
                      c.CompetitionId.ToString(),
                      "|",
                      c.CompetitionName
                    }));
                        }
                    }
                    streamWriter.Close();
                }

                this._selectedCountry.Competitions = list;

                this.pbLoadingCompetitions.Visible = false;
                this.lbCompetition.DataSource = this._selectedCountry.Competitions;
                this.lbCompetition.Select();
                lbCountries.Enabled = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void lbCompetition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && lbCompetition.SelectedItem != null)
                LoadTeamsAsync();
        }

        private void lbCompetition_Click(object sender, EventArgs e)
        {
            if (lbCompetition.SelectedItem != null)
                LoadTeamsAsync();
        }

        #endregion Competitions

        #region Teams

        private async Task LoadTeamsAsync()
        {
            try
            {
                this._selectedCompetition = (Competition)this.lbCompetition.SelectedItem;
                this.lbTeams.DataSource = null;
                this.lbTeams.DisplayMember = "TeamName";
                this.lbTeams.ValueMember = "TeamId";
                this.pbLoadingTeams.Visible = true;
                lbCompetition.Enabled = false;

                List<Team> list = (
                  from cc in this._cachedTeams
                  where cc.CompetitionId == this._selectedCompetition.CompetitionId
                  select cc).ToList<Team>();
                if (list.Count == 0)
                {
                    this._selectedCompetition.Teams = await DataLoader.LoadTeamsAsync(this._selectedCompetition.CompetitionId);
                    DirectoryInfo directoryInfo = new DirectoryInfo("cache");
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    StreamWriter streamWriter = new StreamWriter("cache\\teams.txt", true);

                    foreach (Team t in _selectedCompetition.Teams)
                    {
                        t.CompetitionName = this._selectedCompetition.CompetitionName;
                        if ((
                          from cc in this._cachedTeams
                          where cc.CompetitionId == t.CompetitionId && cc.TeamId == t.TeamId
                          select cc).ToList<Team>().Count == 0)
                        {
                            list.Add(t);
                            this._cachedTeams.Add(t);
                            streamWriter.WriteLine(string.Concat(new string[]
                    {
                      this._selectedCompetition.CompetitionId.ToString(),
                      "|",
                      this._selectedCompetition.CompetitionName,
                      "|",
                      t.TeamId.ToString(),
                      "|",
                      t.TeamName
                    }));
                        }
                    }
                    streamWriter.Close();
                }
                this._selectedCompetition.Teams = list;
                lbCompetition.Enabled = true;
                this.pbLoadingTeams.Visible = false;
                this.lbTeams.DataSource = this._selectedCompetition.Teams;
                if (this._selectedCompetition.Teams.Count > 0)
                {
                    this.lbTeams.SelectedIndex = 0;
                    this.lbTeams.Focus();
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                MessageBox.Show(e.Message);
            }
        }

        private void lbTeams_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && lbTeams.SelectedItem != null)
                this.LoadPlayers();
        }

        private void lbTeams_Click(object sender, EventArgs e)
        {
            if (_generatingInProgress == false && lbTeams.SelectedItem != null)
                this.LoadPlayers();
        }

        private void cbTeams_TextUpdate(object sender, EventArgs e)
        {
            string text = this.cbTeams.Text;
            List<Team> dataSource = (
                from ts in this._cachedTeams
                where Helper.RemoveDiacritics(ts.TeamName, false).ToUpper().Contains(this.cbTeams.Text.ToUpper())
                select ts).ToList<Team>();
            this.lbTeams.ValueMember = "TeamId";
            this.lbTeams.DisplayMember = "TeamFullName";
            this.lbTeams.DataSource = dataSource;
            this.cbTeams.Text = text;
            this.cbTeams.Select(this.cbTeams.Text.Length, 0);
        }

        private void cbTeams_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                this.LoadPlayers();
            }
        }

        private void lbTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_generatingInProgress == false && _playerLoadingInProgress == false)
                this._selectedTeam = (Team)this.lbTeams.SelectedItem;
        }

        private void UpdateTeamAlternativeName(Team t)
        {
            try
            {
                StreamReader sr = new StreamReader(Application.StartupPath + "/cache/teams.txt");
                List<string> lines = new List<string>();
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    if (line.Contains(t.TeamId + "|" + t.TeamName))
                    {
                        line += "|" + t.AlternativeName;
                    }
                    lines.Add(line);
                }
                sr.Close();

                StreamWriter sw = new StreamWriter(Application.StartupPath + "/cache/teams.txt");
                foreach (string s in lines)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                MessageBox.Show(e.Message);
            }
        }

        private List<Team> FindTeamsByName(string teamName)
        {
            // return _cachedTeams.Where(tt => tt.TeamName.Contains(teamName) || (tt.AlternativeName != null && tt.AlternativeName.Equals(teamName))).ToList();
            return _cachedTeams.Where(tt => Helper.RemoveDiacritics(tt.TeamName, false).Contains(teamName) || (tt.AlternativeName != null && tt.AlternativeName.Equals(teamName))).ToList();
        }

        private void mapirajTimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Team t = (Team)lbTeams.SelectedItem;
            if (t.CompetitionId == null)
            {
                frmTeamMapping frm = new frmTeamMapping();
                frm.CachedTeams = this._cachedTeams;
                frm.TeamToMap = t.TeamName;
                DialogResult dr = frm.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    Team mappedTeam = frm.MappedTeam;
                    mappedTeam.AlternativeName = t.TeamName;
                    UpdateTeamAlternativeName(mappedTeam);
                    this._cachedTeams = this.GetCachedTeams();
                    cbTeams.DataSource = _cachedTeams;
                    LoadLineups();
                }
            }
        }

        #endregion Teams

        #region Players

        private async void LoadPlayers()
        {
            try
            {
                _playerLoadingInProgress = true;
                lbTeams.Enabled = false;
                cbTeams.Enabled = false;
                lbMatches.Enabled = false;
                lbUnamapped.Visible = false;
                lblUnmapped.Visible = false;
                this.dgvPlayers.DataSource = null;
                this.tbStatus.Clear();
                this._selectedTeam = (Team)this.lbTeams.SelectedItem;
                Team arg_75_0 = (Team)this.lbTeams.SelectedItem;
                this.btnGenerateExcel.Enabled = false;
                this.pbLoadingPlayers.Visible = true;

                this._selectedTeam.Players = await DataLoader.LoadPlayersAsync(this._selectedTeam.TeamId);

                if (_selectedTeam.GameLineups != null)
                {
                    List<string> matchedPlayers = SetLineupStatus();
                    List<string> unmatchedPlayers = new List<string>();
                    foreach (string s in _selectedTeam.GameLineups)
                    {
                        if (matchedPlayers.Contains(s.ToLower()) == false)
                            unmatchedPlayers.Add(s);
                    }

                    lbUnamapped.DataSource = unmatchedPlayers;
                    if (unmatchedPlayers.Count > 0)
                    {
                        lbUnamapped.Visible = true;
                        lblUnmapped.Visible = true;
                    }
                    else
                    {
                        lbUnamapped.Visible = false;
                        lblUnmapped.Visible = false;
                    }
                }
                else
                {
                    lbUnamapped.Visible = false;
                    lblUnmapped.Visible = false;
                }

                lbTeams.Enabled = true;
                cbTeams.Enabled = true;
                lbMatches.Enabled = true;

                this.pbLoadingPlayers.Visible = false;
                this.btnGenerateExcel.Enabled = true;
                dgvPlayers.AutoGenerateColumns = false;
                dgvPlayers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvPlayers.Columns.Clear();
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.Width = 30;
                c.DataPropertyName = "PlayerNr";
                dgvPlayers.Columns.Add(c);


                c = new DataGridViewTextBoxColumn();
                c.DataPropertyName = "Name";
                c.Width = 125;
                dgvPlayers.Columns.Add(c);

                c = new DataGridViewTextBoxColumn();
                c.DataPropertyName = "MainPosition";
                c.Width = 30;
                dgvPlayers.Columns.Add(c);


                DataGridViewButtonColumn bc = new DataGridViewButtonColumn()
                {
                    UseColumnTextForButtonValue = true,
                    Text = "TMS",
                    Width = 40
                };
                dgvPlayers.Columns.Add(bc);

                c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                c.DataPropertyName = "Injury";
                dgvPlayers.Columns.Add(c);


                dgvPlayers.DataSource = this._selectedTeam.Players;
                foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
                {
                    dgvr.Cells[4].Style = new DataGridViewCellStyle()
                    {
                        Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Italic)
                    };
                    Player player = (Player)dgvr.DataBoundItem;
                    if (player.Lineup == Player.LineUpStatus.YES)
                    {
                        dgvr.Cells[0].Style.BackColor = Color.LightGreen;
                        dgvr.Cells[1].Style.BackColor = Color.LightGreen;
                    }
                    else if (player.Lineup == Player.LineUpStatus.MAYBE)
                    {
                        dgvr.Cells[0].Style.BackColor = Color.Orange;
                        dgvr.Cells[1].Style.BackColor = Color.Orange;
                    }

                    if (player.MainPosition == "GK")
                    {
                        dgvr.Cells[2].Style.BackColor = (Color.LightGreen);
                    }
                    else
                    {
                        if (player.MainPosition == "CB" || player.MainPosition == "LB" || player.MainPosition == "RB")
                        {
                            dgvr.Cells[2].Style.BackColor = Color.Aqua;
                        }
                        else
                        {
                            if (player.MainPosition == "DM" || player.MainPosition == "CM" || player.MainPosition == "LM" ||
                                player.MainPosition == "RM" || player.MainPosition == "AM")
                            {
                                dgvr.Cells[2].Style.BackColor = Color.Orange;
                            }
                            else
                            {
                                if (player.MainPosition == "LW" || player.MainPosition == "RW" || player.MainPosition == "CF" ||
                                    player.MainPosition == "SS")
                                {
                                    dgvr.Cells[2].Style.BackColor = Color.Red;
                                }
                            }
                        }
                    }
                }
                if (dgvPlayers.Rows.Count > 0)
                    dgvPlayers.Rows[0].Selected = false;

                string text;
                if (DateTime.Now.Month >= 8)
                {
                    text = DateTime.Now.Year.ToString();
                }
                else
                {
                    text = (DateTime.Now.Year - 1).ToString();
                }
                for (int i = 0; i < this._selectedTeam.Players.Count; i++)
                {
                    string text2 = this._selectedTeam.Players[i].TmUrl.Substring(1);
                    string text3 = text2.Substring(0, text2.IndexOf("/"));
                    this._selectedTeam.Players[i].PlayerStatsUrl = string.Concat(new object[]
              {
                text3,
                "/leistungsdaten/spieler/",
                this._selectedTeam.Players[i].PlayerId,
                "/saison/",
                text,
                "/plus/1"
              });
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            _playerLoadingInProgress = false;
        }

        private async Task GetPlayersData(Player p)
        {
            StreamWriter streamWriter = null;
            try
            {
                if (p.Lineup == Player.LineUpStatus.YES)
                {
                    Statistics statistics = new Statistics();

                    p.Statistics = new List<Statistics>();
                    p.Matches = new List<Match>();
                    streamWriter =
                      new StreamWriter("cache\\" + this._selectedTeam.TeamId.ToString() + ".txt", true);
                    bool flag = true;
                    bool flag2 = false;
                    for (int i = 0; i < 4; i++)
                    {
                        string year;
                        if (DateTime.Now.Month < 7)
                        {
                            year = (DateTime.Now.Year - i - 1).ToString();
                        }
                        else
                        {
                            year = (DateTime.Now.Year - i).ToString();
                        }
                        bool flag3 = false;
                        Statistics statistics2 = new Statistics();
                        if (!flag)
                        {
                            List<Player> list = (
                              from cs in this._cachedStats
                              where cs.PlayerId.Equals(p.PlayerId)
                              select cs).ToList<Player>();
                            if (list.Count > 0)
                            {
                                List<Statistics> list2 = (
                                  from ss in list.First<Player>().Statistics
                                  where ss.Year.Equals(year)
                                  select ss).ToList<Statistics>();
                                if (list2.Count > 0)
                                {
                                    flag3 = true;
                                    statistics2 = list2.ElementAt(0);
                                    p.Statistics.Add(statistics2);
                                    p.SecondaryPositions = list.First<Player>().SecondaryPositions.ToList<string>();
                                    p.PrefferedFoot = list.First<Player>().PrefferedFoot;
                                }
                            }
                        }
                        if (!flag3 || flag)
                        {
                            DateTime arg_265_0 = DateTime.Now;
                            if (DateTime.Now.Month < 8)
                            {
                                statistics2.Year = (DateTime.Now.Year - i - 1).ToString();
                            }
                            else
                            {
                                statistics2.Year = (DateTime.Now.Year - i).ToString();
                            }
                            DateTime arg_2C2_0 = DateTime.Now;
                            string text = p.TmUrl.Substring(1);
                            string text2 = text.Substring(0, text.IndexOf("/"));
                            string playerStatsUrl = string.Concat(new object[]
                  {
                    text2,
                    "/leistungsdaten/spieler/",
                    p.PlayerId,
                    "/saison/",
                    year,
                    "/plus/1"
                  });
                            string text3 = null;
                            statistics = await DataLoader.LoadPlayerStatisticsAsync(playerStatsUrl, flag, year,
                              p.MainPosition);
                            if (statistics.Contract != null)
                            {
                                p.Contract = statistics.Contract;
                            }
                            if (statistics.NationalPlayer != null)
                                p.NationalPlayer = statistics.NationalPlayer;

                            if (statistics.NationalPlayerUrl != null)
                                p.NationalPlayerUrl = statistics.NationalPlayerUrl;

                            if (!flag && !flag2)
                            {
                                flag2 = true;
                                List<string> secondaryPositions = new List<string>();
                                AdditionalPlayerData additionalPlayerData =
                                  await DataLoader.LoadAdditionPlayerDataAsync(text2 + "/profil/spieler/" + p.PlayerId);
                                p.PrefferedFoot = additionalPlayerData.PrefferedFoot;
                                p.SecondaryPositions = additionalPlayerData.SecondaryPositions;
                            }
                            if (statistics != null && !flag && statistics.GamesPlayed != null)
                            {
                                streamWriter.Write(string.Concat(new object[]
                    {
                      p.PlayerId,
                      "|",
                      year,
                      "-",
                      statistics.GamesPlayed.Replace("-", "0"),
                      "-",
                      statistics.GoalsScored.Replace("-", "0"),
                      "-",
                      statistics.Assists.Replace("-", "0"),
                      "-",
                      statistics.MinutesPlayed.Replace(".", "").Replace("-", "0.000001"),
                      "-",
                      statistics.MinutesPerGoal.Replace(".", "").Replace("-", "0.000001"),
                      "-",
                      p.PrefferedFoot
                    }));
                                string text4 = "";
                                foreach (string current in p.SecondaryPositions)
                                {
                                    text4 = text4 + "," + current;
                                }
                                if (text4 != "")
                                {
                                    streamWriter.Write("-" + text4.Substring(1));
                                }
                                streamWriter.Write(Environment.NewLine);
                            }
                            p.Statistics.Add(statistics);
                            if (flag && statistics != null)
                            {
                                p.Matches = statistics.Matches.OrderByDescending(m => m.Date).Take(4).ToList();
                                flag = false;
                            }
                        }
                    }

                    this._counter++;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Dispose();
            }

        }

        private void dgvPlayers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.ColumnIndex >= 0 && senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                Player p = (Player)dgvPlayers.Rows[e.RowIndex].DataBoundItem;
                Process.Start("http://www.transfermarkt.com/" + p.TmUrl);
            }
        }

        private List<Player> LoadCachedData(int teamId)
        {
            this._cachedStats = new List<Player>();

            try
            {
                if (File.Exists("cache\\" + teamId + ".txt"))
                {
                    StreamReader streamReader = new StreamReader("cache\\" + teamId + ".txt");
                    string a = "";
                    Player player = new Player();
                    player.Statistics = new List<Statistics>();
                    while (!streamReader.EndOfStream)
                    {
                        string input = streamReader.ReadLine();
                        string[] array = Regex.Split(input, "\\|");
                        string text = array[0];
                        if (a != text)
                        {
                            player = new Player();
                            player.PlayerId = int.Parse(text);
                            player.Statistics = new List<Statistics>();
                            this._cachedStats.Add(player);
                        }
                        Statistics statistics = new Statistics();
                        string[] array2 = Regex.Split(array[1], "-");
                        statistics.Year = array2[0];
                        statistics.GamesPlayed = array2[1];
                        statistics.GoalsScored = array2[2];
                        statistics.Assists = array2[3];
                        statistics.MinutesPlayed = array2[4];
                        statistics.MinutesPerGoal = array2[5];
                        player.Statistics.Add(statistics);
                        if (array2.Length > 6)
                        {
                            player.PrefferedFoot = array2[6];
                        }
                        player.SecondaryPositions = new List<string>();
                        if (array2.Length > 7)
                        {
                            string input2 = array2[7];
                            string[] array3 = Regex.Split(input2, ",");
                            string[] array4 = array3;
                            for (int i = 0; i < array4.Length; i++)
                            {
                                string item = array4[i];
                                player.SecondaryPositions.Add(item);
                            }
                        }
                        a = text;
                    }
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            return this._cachedStats;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _generatingInProgress = false;
            btnGenerateExcel.Enabled = true;
            btnStop.Visible = false;
        }

        #endregion Players

        #region Livescore

        private async Task StartLivescoreFeedAsync()
        {
            pbLivescore.Visible = true;
            try
            {
                List<Game> previousData = new List<Game>();
                if (rbCompleted.Checked == true)
                {
                    previousData = gamesCompleted;
                    gamesCompleted = await DataLoader.LoadGamesWithAnnouncedLineUpAsync(dtpDatum.Value);
                    gamesCompleted = gamesCompleted.Where(g => g.GameStatus == Game.GameStatusEnum.FINISHED)
                      .OrderBy(g => g.Time)
                      .ToList();

                }
                else if (rbLive.Checked == true)
                {
                    previousData = gamesLive;
                    gamesLive = await DataLoader.LoadGamesWithAnnouncedLineUpAsync(dtpDatum.Value);
                    gamesLive = gamesLive.Where(g => g.GameStatus == Game.GameStatusEnum.LIVE)
                      .OrderBy(g => g.ProgressMinute)
                      .ToList();
                }
                else if (rbSlijedi.Checked == true)
                {
                    previousData = gamesUpcoming;
                    gamesUpcoming = await DataLoader.LoadGamesWithAnnouncedLineUpAsync(dtpDatum.Value);
                    gamesUpcoming = gamesUpcoming.Where(g => g.GameStatus == Game.GameStatusEnum.UPCOMING)
                      .OrderBy(g => g.Time)
                      .ToList();
                }
                if (rbCompleted.Checked == true)
                {
                    foreach (Game g in gamesCompleted)
                    {
                        if (previousData.Where(gg => gg.LineupUrl == g.LineupUrl).Count() == 0)
                            g.LastChange = DateTime.Now;
                        else
                            g.LastChange = previousData.Where(gg => gg.LineupUrl == g.LineupUrl).ElementAt(0).LastChange;
                    }
                    games = gamesCompleted.OrderByDescending(gg => gg.LastChange).ToList();
                }
                else if (rbSlijedi.Checked == true)
                {
                    foreach (Game g in gamesUpcoming)
                    {
                        if (previousData.Where(gg => gg.LineupUrl == g.LineupUrl).Count() == 0)
                            g.LastChange = DateTime.Now;
                        else
                            g.LastChange = previousData.Where(gg => gg.LineupUrl == g.LineupUrl).ElementAt(0).LastChange;
                    }
                    games = gamesUpcoming.OrderByDescending(gg => gg.LastChange).OrderBy(gg => gg.Time).ToList();
                }
                else
                {
                    foreach (Game g in gamesLive)
                    {
                        List<Game> oldData = previousData.Where(gg => gg.LineupUrl == g.LineupUrl).ToList();
                        if (oldData.Count > 0)
                        {
                            Game old = oldData.First();
                            if (old.Result != g.Result)
                                g.LastChange = DateTime.Now;
                            else
                                g.LastChange = old.LastChange;
                        }
                        else
                            g.LastChange = DateTime.Now;
                    }
                    games = gamesLive;
                }
            }
            catch (Exception ee)
            {
                Logger.Exception(ee);
            }

            lbMatches.AutoGenerateColumns = false;
            lbMatches.Columns.Clear();
            lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Progress", Width = 40 });
            lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Home", Width = 120 });
            lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Away", Width = 120 });
            lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Result", Width = 40 });
            lbMatches.Columns.Add(new DataGridViewButtonColumn() { UseColumnTextForButtonValue = true, Text = "LS", Width = 30 });


            lbMatches.DataSource = games;

            if (lbMatches.Rows.Count > 0)
                lbMatches.Rows[0].Cells[0].Selected = false;

            foreach (DataGridViewRow r in lbMatches.Rows)
            {
                Game g = (Game)r.DataBoundItem;
                if (DateTime.Now.Subtract(g.LastChange).TotalSeconds < 60)
                    r.Cells[0].Style.BackColor = Color.LightGreen;
            }

            pbLivescore.Visible = false;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            StartLivescoreFeedAsync();
        }

        private void rbSlijedi_Click(object sender, EventArgs e)
        {
            StartLivescoreFeedAsync();
        }

        private void rbLive_Click(object sender, EventArgs e)
        {
            dtpDatum.Value = DateTime.Now;
            StartLivescoreFeedAsync();
        }

        private void rbCompleted_Click(object sender, EventArgs e)
        {
            StartLivescoreFeedAsync();
        }

        private void tmrReload_Tick_1(object sender, EventArgs e)
        {
            StartLivescoreFeedAsync();
        }

        private void lbMatches_Click(object sender, EventArgs e)
        {
            LoadLineups();
        }

        private void lbMatches_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                Game g = (Game)lbMatches.Rows[e.RowIndex].DataBoundItem;
                Process.Start(g.LineupUrl);
            }
            else
            {
                //if (_generatingInProgress == false)
                LoadLineups();
            }
        }

        private async Task LoadLineups()
        {
            try
            {
                if (lbMatches.SelectedRows.Count == 1)
                {
                    SelectedGame = (Game)lbMatches.SelectedRows[0].DataBoundItem;
                    lbMatches.Enabled = false;
                    lbTeams.DataSource = null;
                    lbTeams.Items.Clear();

                    pbLoadingTeams.Visible = true;

                    SelectedGameLinups = await DataLoader.LoadLineupsAsync(SelectedGame.LineupUrl);

                    pbLoadingTeams.Visible = false;
                    lbMatches.Enabled = true;
                    List<Team> listBoxSource = new List<Team>();

                    List<Team> teams = FindTeamsByName(SelectedGame.Home);
                    if (teams.Count() == 0)
                    {
                        Team t = new Team();
                        t.TeamName = SelectedGame.Home;
                        t.CompetitionName = "NEMAPIRANO";

                        teams.Add(t);
                    }
                    foreach (Team t in teams)
                    {
                        t.GameLineups = SelectedGameLinups.HomeLineup;
                        listBoxSource.Add(t);
                    }

                    teams = FindTeamsByName(SelectedGame.Away);
                    if (teams.Count() == 0)
                    {
                        Team t = new Team();
                        t.TeamName = SelectedGame.Away;
                        t.CompetitionName = "NEMAPIRANO";
                        teams.Add(t);
                    }
                    foreach (Team t in teams)
                    {
                        t.GameLineups = SelectedGameLinups.AwayLineup;
                        listBoxSource.Add(t);
                    }

                    lbTeams.DataSource = listBoxSource;
                    this.lbTeams.DisplayMember = "TeamFullName";
                    this.lbTeams.ValueMember = "TeamId";
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void tsmiYES_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dgvPlayers.SelectedRows)
            {
                Player p = (Player)r.DataBoundItem;
                p.Lineup = Player.LineUpStatus.YES;
                r.Cells[0].Style.BackColor = Color.LightGreen;
                r.Cells[1].Style.BackColor = Color.LightGreen;
            }
        }

        private void tsmiNO_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dgvPlayers.SelectedRows)
            {
                Player p = (Player)r.DataBoundItem;
                p.Lineup = Player.LineUpStatus.NO;
                r.Cells[0].Style.BackColor = Color.White;
                r.Cells[1].Style.BackColor = Color.White;
            }
        }

        private List<string> SetLineupStatus()
        {
            List<string> matchedPlayers = new List<string>();
            try
            {
                List<string> gameLineups;

                if (_selectedTeam.GameLineups == null)
                    return matchedPlayers;
                else
                    gameLineups = _selectedTeam.GameLineups.ConvertAll(d => d.ToLower());
                foreach (Player p in _selectedTeam.Players)
                {
                    p.Lineup = Player.LineUpStatus.NO;
                    string name = p.Name;
                    name = Helper.RemoveDiacritics(name.ToLower(), false);
                    if (gameLineups.Contains(name.ToLower()))
                    {
                        if (!matchedPlayers.Contains(name))
                            matchedPlayers.Add(name);
                        p.Lineup = Player.LineUpStatus.YES;
                    }
                    else
                    {
                        name = p.Name;
                        name = Helper.RemoveDiacritics(name.ToLower(), true);
                        if (gameLineups.Contains(name.ToLower()))
                        {
                            if (!matchedPlayers.Contains(name))
                                matchedPlayers.Add(name);
                            p.Lineup = Player.LineUpStatus.YES;
                        }
                        else
                        {
                            int matchCount = 0;
                            string[] nameparts = name.Replace('-', ' ').Split(' ');
                            foreach (string lineupPlayer in gameLineups)
                            {
                                matchCount = 0;
                                string[] lpNameParts = lineupPlayer.Replace('-', ' ').Split(' ');
                                foreach (string lpnp in lpNameParts)
                                {
                                    foreach (string np in nameparts)
                                    {
                                        if (np == lpnp)
                                        {
                                            matchCount++;
                                        }
                                    }

                                    if (matchCount > 1)
                                    {
                                        if (!matchedPlayers.Contains(lineupPlayer))
                                            matchedPlayers.Add(lineupPlayer);
                                        p.Lineup = Player.LineUpStatus.YES;
                                        break;
                                    }
                                    else if (matchCount == 1)
                                    {
                                        int cc = Helper.CommonCharacters(name, lineupPlayer);
                                        decimal pc = (cc * 2m) / (name.Length + lineupPlayer.Length);
                                        if ((pc * 100) > COMMON_CHARACTER_MATCH_PERCENTAGE)
                                        {
                                            if (!matchedPlayers.Contains(lineupPlayer))
                                                matchedPlayers.Add(lineupPlayer);
                                            p.Lineup = Player.LineUpStatus.YES;
                                        }
                                        else
                                            p.Lineup = Player.LineUpStatus.MAYBE;
                                    }
                                }
                            }

                            name = Helper.RemoveDiacritics(name, false);
                            nameparts = name.Replace('-', ' ').Split(' ');
                            foreach (string lineupPlayer in gameLineups)
                            {
                                matchCount = 0;
                                string[] lpNameParts = lineupPlayer.Replace('-', ' ').Split(' ');
                                foreach (string lpnp in lpNameParts)
                                {
                                    foreach (string np in nameparts)
                                    {
                                        if (np == lpnp)
                                        {
                                            matchCount++;
                                        }
                                    }

                                    if (matchCount > 1)
                                    {
                                        if (!matchedPlayers.Contains(lineupPlayer))
                                            matchedPlayers.Add(lineupPlayer);
                                        p.Lineup = Player.LineUpStatus.YES;
                                    }
                                    else if (matchCount == 1)
                                    {
                                        int cc = Helper.CommonCharacters(name, lineupPlayer);
                                        decimal pc = (cc * 2m) / (name.Length + lineupPlayer.Length);
                                        if ((pc * 100) > COMMON_CHARACTER_MATCH_PERCENTAGE)
                                        {
                                            if (!matchedPlayers.Contains(lineupPlayer))
                                                matchedPlayers.Add(lineupPlayer);
                                            p.Lineup = Player.LineUpStatus.YES;
                                        }
                                        else
                                            p.Lineup = Player.LineUpStatus.MAYBE;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            return matchedPlayers;
        }


        #endregion Livescore

        #region Documents

        private void btnGenerateExcel_Click(object sender, EventArgs e)
        {
            GenerateExcel();
        }

        private async Task GenerateExcel()
        {
            try
            {
                _generatingInProgress = true;
                if (_selectedTeam.Players == null)
                    return;
                foreach (DataGridViewRow r in dgvPlayers.Rows)
                {
                    Player p = (Player)r.DataBoundItem;
                    _selectedTeam.Players.Where(pp => pp.PlayerId == p.PlayerId).FirstOrDefault().Lineup = p.Lineup;
                }
                if (_selectedTeam.GameLineups != null &&
                    _selectedTeam.Players.Where(p => p.Lineup == Player.LineUpStatus.YES).Count() < 11)
                {
                    DialogResult dr =
                      MessageBox.Show(
                        "Neki od igraca iz najavljenog sastava nisu mapirani. Nastaviti generisanje dokumenta bez njih ?",
                        "Pitanje", MessageBoxButtons.YesNo);
                    if (dr != DialogResult.Yes)
                        return;
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath + "\\cache\\xls\\");
                if (!directoryInfo.Exists)
                    directoryInfo.Create();

                this.btnGenerateExcel.Enabled = false;
                btnStop.Visible = true;
                this._counter = 0;
                this.tbStatus.Visible = true;
                this._cachedStats = this.LoadCachedData(this._selectedTeam.TeamId);

                foreach (Player p in _selectedTeam.Players)
                {
                    this.tbStatus.Text = string.Concat(new object[]
              {
                (this._counter + 1).ToString(),
                " od ",
                this._selectedTeam.Players.Where(pl => pl.Lineup == TMS.Player.LineUpStatus.YES).Count(),
                " - ",
                p.Name
              });
                    foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
                    {
                        CurrentPlayer = p;
                        if (dgvr.DataBoundItem == CurrentPlayer)
                            dgvPlayers.CurrentCell = dgvr.Cells[0];
                    }
                    await GetPlayersData(p);
                    if (_generatingInProgress == false)
                        return;
                }
                lbMatches.Enabled = true;
                dtpDatum.Enabled = true;
                this.lbTeams.Enabled = true;
                this.lbCompetition.Enabled = true;
                this.btnGenerateExcel.Enabled = true;
                btnStop.Visible = false;
                this.tbStatus.Visible = false;
                this.cbTeams.Enabled = true;
                this.cbCountries.Enabled = true;
                this.lbCountries.Enabled = true;
                Guid.NewGuid();
                string text = string.Concat(new string[]
			{
				"cache\\xls\\",
				this._selectedTeam.TeamName,
				" [",
				this._selectedTeam.TeamId.ToString(),
				"]\\",
				this._selectedTeam.TeamName,
				"_",
				DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss"),
				".xlsx"
			});
                DocumentBuilder.CreateCXMLDocument(text, this._selectedTeam);
                if (File.Exists(text))
                {
                    //this.BindDocuments();
                    LoadDocuments();
                    Process.Start(text);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }
            _generatingInProgress = false;
        }

        private void LoadDocuments()
        {
            try
            {
                List<FileInfo> allFiles = new List<FileInfo>();

                DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "//cache//xls//");

                foreach (DirectoryInfo diTeam in di.GetDirectories())
                {
                    FileInfo[] files = diTeam.GetFiles();
                    foreach (FileInfo fi in files)
                        allFiles.Add(fi);
                }

                lbLatest.DataSource = allFiles.OrderByDescending(f => f.LastWriteTime).ToList();
                lbLatest.ValueMember = "Name";
                lbLatest.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void lbLatest_DoubleClick(object sender, EventArgs e)
        {
            if (lbLatest.SelectedItem != null)
            {
                var a = lbLatest.SelectedItem;
                Process.Start(((FileInfo)a).FullName);
            }
        }

        #endregion Documents
    }
}
