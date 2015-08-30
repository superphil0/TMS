using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
    private bool _scheduleUpdateInProgress = false;
    private bool _competitionUpdateInProgress = false;
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
      this._cachedCountries = this.GetCachedCountries();

      this._cachedCompetitions = this.GetCachedCompetitions();
      this._cachedTeams = this.GetCachedTeams();

      this.cbTeams.DataSource = this._cachedTeams;
      this.cbTeams.DisplayMember = "TeamName";
      this.cbTeams.ValueMember = "TeamId";
      this.cbTeams.SelectedIndex = -1;
      LoadCountriesAsync();
      LoadDocuments();

      StartLivescoreFeedAsync();
      tmrReload.Start();

      cbCurrentSeason.Items.Add(DateTime.Now.Year);
      cbCurrentSeason.Items.Add(DateTime.Now.Year - 1);

      if (DateTime.Now.Month < 7)
        cbCurrentSeason.SelectedIndex = 1;
      else
        cbCurrentSeason.SelectedIndex = 0;
    }

    private List<Team> GetCachedTeams()
    {
      List<Team> list = new List<Team>();
      string text = "";
      try
      {
        if (File.Exists("cache\\teams.txt"))
        {
          StreamReader streamReader = new StreamReader("cache\\teams.txt");
          while (!streamReader.EndOfStream)
          {
            text = streamReader.ReadLine();
            string[] array = text.Split(new char[]
    {
                  '|'
    });

            string alternativeName = null;
            if (array.Length > 5)
              alternativeName = array[5];

            list.Add(new Team
            {
              TeamId = int.Parse(array[2]),
              TeamName = array[3],
              UrlName = array[4],
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
              CountryName = array[1],
              Top = bool.Parse(array[2])
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

            var c = new Competition
            {
              CompetitionId = array[2],
              CompetitionName = array[3],
              CompetitionCountryId = int.Parse(array[0])
            };

            if (array.Length > 4)
              c.AlternativeName = array[4];

            var country = _cachedCountries.Where(cc => cc.CountryId == c.CompetitionCountryId).FirstOrDefault();
            c.CompetitionCountry = country.CountryName;

            list.Add(c);
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
      if (((Country)lbCountries.SelectedItem).CountryId != -1)
      {
        LoadCompetitionsAsync();
        if (_generatingInProgress == false)
          btnArhiva.Enabled = true;
      }
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
          _countries.Insert(0, new Country() { CountryName = "CUP COMPETITIONS", CountryId = -2 });
          _countries.Insert(1, new Country() { CountryName = "INT. CUP COMPETITIONS", CountryId = -3 });
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
              streamWriter.WriteLine(c.CountryId.ToString() + "|" + c.CountryName + "|" + c.Top);
            }
          }
          streamWriter.Close();
        }

        this._countries = cachedCountries.Where(cc => cc.Top == false).ToList();
        var topCountries = cachedCountries.Where(cc => cc.Top == true).ToList();

        foreach (var c in topCountries)
        {
          c.CountryName = c.CountryName;
          _countries.Insert(0, c);
        }

        _countries.Insert(topCountries.Count, new Country() { CountryId = -1, CountryName = "----------------------" });

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
      {
        LoadCompetitionsAsync();
        if (_generatingInProgress == false)
          btnArhiva.Enabled = true;
      }
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
          //KLUPSKA TAKMICENJA
          if (_selectedCountry.CountryId == -2)
            this._selectedCountry.Competitions =
              await DataLoader.LoadCupCompetitionsAsync();

          //MEDJUNARODNA TAKMICENJA
          else if (_selectedCountry.CountryId == -3)
            this._selectedCountry.Competitions =
              await DataLoader.LoadInternationalCupCompetitionsAsync();

          //OSTALO
          else
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
                          t.TeamName,
                          "|",
                          t.UrlName
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


        if (_generatingInProgress == false)
        {
          btnAzurirajArhivu.Enabled = true;
        }
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

    private async Task LoadTeamsAsync(bool changeCompetition=true)
    {
      try
      {
        if (_competitionUpdateInProgress == true)
          return;
        if(changeCompetition==true)
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
          List<Team> competitions = null;
          if (_selectedCompetition.CompetitionCountryId < 0)//KUP TAKMICENJA
            competitions = await DataLoader.LoadTeamsAsync(this._selectedCompetition.CompetitionId);
          else
            competitions = await DataLoader.LoadTeamsAlternativeAsync(this._selectedCompetition.CompetitionId);
          this._selectedCompetition.Teams = competitions;

          DirectoryInfo directoryInfo = new DirectoryInfo("cache");
          if (!directoryInfo.Exists)
          {
            directoryInfo.Create();
          }
          StreamWriter streamWriter = new StreamWriter("cache\\teams.txt", true);
          if (_selectedCompetition.Teams != null)
          {
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
                      t.TeamName,
                      "|",
                      t.UrlName
        }));
              }
            }
          }
          streamWriter.Close();
        }
        foreach (var el in list)
        {
          el.Url = "http://www.transfermarkt.de/jumplist/startseite/verein/" + el.TeamId;
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

        LoadArchive(_selectedCompetition);
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
      {
        cbCurrentSeason.SelectedIndex = 0;

        this._selectedTeam = (Team)this.lbTeams.SelectedItem;

        if (_selectedTeam != null)
        {
          var competition = _cachedCompetitions.Where(cc => cc.CompetitionId == _selectedTeam.CompetitionId).FirstOrDefault();
          this._selectedCompetition = competition;
          LoadArchive(competition);
        }
      }
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


    private void UpdateCompetitionAlternativeName(Competition c)
    {
      try
      {
        StreamReader sr = new StreamReader(Application.StartupPath + "/cache/competitions.txt");
        List<string> lines = new List<string>();
        while (sr.EndOfStream == false)
        {
          string line = sr.ReadLine();
          if (line.Contains(c.CompetitionId + "|" + c.CompetitionName))
          {
            var parts = line.Split('|');
            if (parts.Count() == 4)
              line += "|" + c.AlternativeName;
            else
              line = line.Substring(0, line.LastIndexOf("|") + 1) + c.AlternativeName;
          }
          lines.Add(line);
        }
        sr.Close();

        StreamWriter sw = new StreamWriter(Application.StartupPath + "/cache/competitions.txt");
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



    private void UpdateCountryTop(Country c)
    {
      try
      {
        StreamReader sr = new StreamReader(Application.StartupPath + "/cache/countries.txt");
        List<string> lines = new List<string>();
        while (sr.EndOfStream == false)
        {
          string line = sr.ReadLine();
          if (line.Contains(c.CountryId + "|" + c.CountryName))
          {
            line = line.Substring(0, line.LastIndexOf("|")) + "|" + c.Top;
          }
          lines.Add(line);
        }
        sr.Close();

        StreamWriter sw = new StreamWriter(Application.StartupPath + "/cache/countries.txt");
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

    private List<Team> FindTeamsByName(string teamName, string competitionId = null)
    {
      // return _cachedTeams.Where(tt => tt.TeamName.Contains(teamName) || (tt.AlternativeName != null && tt.AlternativeName.Equals(teamName))).ToList();
      var teams = _cachedTeams.Where(tt => tt.AlternativeName != null && tt.AlternativeName.Equals(teamName) && (tt.CompetitionId == competitionId || competitionId == null));
      if (teams.Count() != 0)
        return teams.ToList();
      else
        return _cachedTeams.Where(tt => Helper.RemoveDiacritics(tt.TeamName, false).Contains(teamName) && (tt.CompetitionId == competitionId || competitionId == null)).ToList();
    }

    private void mapirajTimToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Team t = (Team)lbTeams.SelectedItem;
      if (t.CompetitionId == null)
      {
        frmTeamMapping frm = new frmTeamMapping();
        frm.CachedTeams = this._cachedTeams;
        frm.TeamToMap = t.TeamName;
        frm.CompetitionId = SelectedGame.CompetitionId;
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
      else
      {
        if (t.Tag == "H")
          t.AlternativeName = SelectedGame.Home;
        else
          t.AlternativeName = SelectedGame.Away;
        UpdateTeamAlternativeName(t);
        this._cachedTeams = GetCachedTeams();
        cbTeams.DataSource = _cachedTeams;
        LoadLineups();
      }
    }

    #endregion Teams

    #region Players

    private async Task LoadPlayers()
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
        this.btnGenerateExcel.Enabled = false;
        this.pbLoadingPlayers.Visible = true;

        this._selectedTeam.Players = await DataLoader.LoadPlayersAsync(this._selectedTeam.TeamId);

        _cachedStats = LoadCachedData(_selectedTeam.TeamId);

        foreach (var p in _selectedTeam.Players)
        {
          var pl = _cachedStats.Where(cs => cs.PlayerId == p.PlayerId).FirstOrDefault();
          if (pl != null)
            p.CurrentSeasonStatistics = pl.CurrentSeasonStatistics;
        }

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


        c = new DataGridViewTextBoxColumn();
        c.DataPropertyName = "UU";
        c.Width = 30;
        dgvPlayers.Columns.Add(c);

        c = new DataGridViewTextBoxColumn();
        c.DataPropertyName = "MU";
        c.Width = 30;
        dgvPlayers.Columns.Add(c);

        c = new DataGridViewTextBoxColumn();
        c.DataPropertyName = "UG";
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
          dgvr.Cells[7].Style = new DataGridViewCellStyle()
          {
            Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Italic)
          };
          Player player = (Player)dgvr.DataBoundItem;

          if (player.CurrentSeasonStatistics != null)
          {
            dgvr.Cells[3].Value = player.CurrentSeasonStatistics.GamesPlayed;
            dgvr.Cells[4].Value = player.CurrentSeasonStatistics.MinutesPlayed;
            dgvr.Cells[5].Value = player.CurrentSeasonStatistics.GoalsScored;
          }

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

        text = cbCurrentSeason.SelectedItem.ToString();

        for (int i = 0; i < this._selectedTeam.Players.Count; i++)
        {
          string text2 = this._selectedTeam.Players[i].TmUrl.Substring(1);
          string text3 = text2.Substring(0, text2.IndexOf("/"));
          this._selectedTeam.Players[i].PlayerStatsUrl = text3 + "/leistungsdaten/spieler/" + this._selectedTeam.Players[i].PlayerId + "/saison/" + text + "/plus/1";
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
      StreamWriter streamWriter = null, currentSeasonData = null;
      try
      {
        if (p.Lineup == Player.LineUpStatus.YES)
        {
          Statistics statistics = new Statistics();

          p.Statistics = new List<Statistics>();
          p.Matches = new List<Match>();
          streamWriter = new StreamWriter("cache\\" + this._selectedTeam.TeamId.ToString() + ".txt", true);

          currentSeasonData = new StreamWriter("cache\\" + this._selectedTeam.TeamId.ToString() + "_tmp.txt", true);

          bool currentSeason = true;
          bool flag2 = false;
          for (int i = 0; i < 4; i++)
          {
            string year;

            year = ((int)cbCurrentSeason.SelectedItem - i).ToString();
            bool flag3 = false;
            Statistics statistics2 = new Statistics();
            if (!currentSeason)
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
            if (!flag3 || currentSeason)
            {
              statistics2.Year = ((int)cbCurrentSeason.SelectedItem - i).ToString();

              string text = p.TmUrl.Substring(1);
              string text2 = text.Substring(0, text.IndexOf("/"));
              string playerStatsUrl = text2 + "/leistungsdaten/spieler/" + p.PlayerId + "/saison/" + year + "/plus/1";

              statistics = await DataLoader.LoadPlayerStatisticsAsync(playerStatsUrl, currentSeason, year, p.MainPosition);

              if (statistics != null && statistics.Contract != null)
                p.Contract = statistics.Contract;

              if (statistics != null && statistics.NationalPlayer != null)
                p.NationalPlayer = statistics.NationalPlayer;

              if (statistics != null && statistics.NationalPlayerUrl != null)
                p.NationalPlayerUrl = statistics.NationalPlayerUrl;

              if (!currentSeason && !flag2)
              {
                flag2 = true;
                List<string> secondaryPositions = new List<string>();
                AdditionalPlayerData additionalPlayerData =
                  await DataLoader.LoadAdditionPlayerDataAsync(text2 + "/profil/spieler/" + p.PlayerId);
                p.PrefferedFoot = additionalPlayerData.PrefferedFoot;
                p.SecondaryPositions = additionalPlayerData.SecondaryPositions;
              }

              var playerData = p.PlayerId + "|" + year + "-" + statistics.GamesPlayed.Replace("-", "0") + "-" + statistics.GoalsScored.Replace("-", "0") +
                      "-" + statistics.Assists.Replace("-", "0") + "-" + statistics.MinutesPlayed.Replace(".", "").Replace("-", "0.000001") +
                      "-" + statistics.MinutesPerGoal.Replace(".", "").Replace("-", "0.000001") + "-" + p.PrefferedFoot;

              if (statistics != null && !currentSeason && statistics.GamesPlayed != null)
              {
                streamWriter.Write(playerData);

                string text4 = "";
                foreach (string current in p.SecondaryPositions)
                  text4 = text4 + "," + current;

                if (text4 != "")
                  streamWriter.Write("-" + text4.Substring(1));

                streamWriter.Write(Environment.NewLine);
              }

              p.Statistics.Add(statistics);
              if (currentSeason && statistics != null)
              {
                p.Matches = statistics.Matches.OrderByDescending(m => m.Date).Take(4).ToList();
                currentSeason = false;

                currentSeasonData.WriteLine(playerData);
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

        if (currentSeasonData != null)
          currentSeasonData.Dispose();
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
        //LOADING PREVIOUS SEASON DATA
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

        //LOADING CURRENT SEASON DATA
        if (File.Exists("cache\\" + teamId + "_tmp.txt"))
        {
          StreamReader streamReader = new StreamReader("cache\\" + teamId + "_tmp.txt");
          while (!streamReader.EndOfStream)
          {
            string input = streamReader.ReadLine();
            string[] array = Regex.Split(input, "\\|");
            string text = array[0];

            int playerId = int.Parse(text);
            Statistics statistics = new Statistics();
            string[] array2 = Regex.Split(array[1], "-");
            statistics.Year = array2[0];
            statistics.GamesPlayed = array2[1];
            statistics.GoalsScored = array2[2];
            statistics.Assists = array2[3];
            statistics.MinutesPlayed = array2[4];
            statistics.MinutesPerGoal = array2[5];

            var existing = _cachedStats.Where(cs => cs.PlayerId == playerId).FirstOrDefault();
            if (existing != null)
              existing.CurrentSeasonStatistics = statistics;
            else
            {
              Player player = new Player();
              player.PlayerId = int.Parse(text);
              player.CurrentSeasonStatistics = statistics;
              _cachedStats.Add(player);
            }
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
      _playerLoadingInProgress = false;
      btnGenerateExcel.Enabled = true;
      btnStop.Visible = false;
      btnAzurirajArhivu.Enabled = true;
      btnArhiva.Enabled = true;
      btnAzurirajLigu.Enabled = true;
      lbTeams.Enabled = true;
      _competitionUpdateInProgress = false;
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
      lbMatches.Columns.Add(new DataGridViewImageColumn() { Width = 20 });
      lbMatches.Columns.Add(new DataGridViewImageColumn() { Width = 30 });
      lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Progress", Width = 35 });
      lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Home", Width = 105 });
      lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Away", Width = 105 });
      lbMatches.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Result", Width = 30 });
      lbMatches.Columns.Add(new DataGridViewButtonColumn() { UseColumnTextForButtonValue = true, Text = "LS", Width = 30 });



      foreach (var g in games)
      {
        var competition = _cachedCompetitions.Where(cc => cc.AlternativeName != null && cc.AlternativeName.Equals(g.LineupUrl.Split('/')[4] + "/" + g.LineupUrl.Split('/')[5])).FirstOrDefault();
        if (competition != null)
          g.CompetitionId = competition.CompetitionId;
      }

      if (rbIzabranaLiga.Checked == true)
      {
        if (lbCompetition.SelectedItem != null)
        {
          var competitionId = ((Competition)lbCompetition.SelectedItem).CompetitionId;
          games.RemoveAll(ga => ga.CompetitionId != competitionId);
        }
      }

      lbMatches.DataSource = games;

      if (lbMatches.Rows.Count > 0)
        lbMatches.Rows[0].Cells[0].Selected = false;

      foreach (DataGridViewRow r in lbMatches.Rows)
      {
        Game g = (Game)r.DataBoundItem;

        var competition = _cachedCompetitions.Where(cc => cc.CompetitionId == g.CompetitionId).FirstOrDefault();
        if (DateTime.Now.Subtract(g.LastChange).TotalSeconds < 60)
          r.Cells[2].Style.BackColor = Color.LightGreen;

        var tt = g.LineupUrl.Split('/')[4].Replace("-", " ").ToUpper() + " - " + g.LineupUrl.Split('/')[5].Replace("-", " ").ToUpper();

        r.Cells[1].ToolTipText = tt;
        r.Cells[2].ToolTipText = tt;
        r.Cells[3].ToolTipText = tt;
        r.Cells[4].ToolTipText = tt;

        if (g.CompetitionId != null)
        {
          DataGridViewImageCell cell = (DataGridViewImageCell)r.Cells[0];
          if (File.Exists(Application.StartupPath + "/cache/img/competitions/" + competition.CompetitionId + ".png"))
          {
            cell.Value = Image.FromFile("cache/img/competitions/" + competition.CompetitionId + ".png");
            cell.ToolTipText = competition.CompetitionName;
          }
          else
          {
            using (WebClient webClient = new WebClient())
            {
              try
              {
                DirectoryInfo d = new DirectoryInfo(Application.StartupPath + "/cache/img/competitions");
                if (d.Exists == false)
                  d.Create();

                webClient.DownloadFile("http://www.transfermarkt.com/images/logo/tiny/" + competition.CompetitionId.ToLower() + ".png", Application.StartupPath + "/cache/img/competitions/" + competition.CompetitionId + ".png");
                cell.Value = Image.FromFile("cache/img/competitions/" + competition.CompetitionId + ".png");
                cell.ToolTipText = competition.CompetitionName;
              }
              catch (Exception e)
              {
                Logger.Exception(e);
              }
            }
          }

          var cou = _cachedCompetitions.Where(co => co.CompetitionId == g.CompetitionId).FirstOrDefault();
          DataGridViewImageCell c = (DataGridViewImageCell)r.Cells[1];

          if (competition.CompetitionCountryId < 0)
            c.Value = Image.FromFile("cache/img/competitions/" + competition.CompetitionId + ".png");
          else
          {
            if (File.Exists("cache/img/" + cou.CompetitionCountryId + ".png"))
            {
              c.Value = Image.FromFile("cache/img/" + cou.CompetitionCountryId + ".png");
              var country = _cachedCountries.Where(cc => cc.CountryId == cou.CompetitionCountryId).FirstOrDefault();
              if (country != null)
                c.ToolTipText = country.CountryName;
            }
            else
            {
              using (WebClient webClient = new WebClient())
              {
                try
                {

                  webClient.DownloadFile("http://www.transfermarkt.com/images/flagge/small/" + cou.CompetitionCountryId + ".png", Application.StartupPath + "/cache/img/" + cou.CompetitionCountryId + ".png");

                  c.Value = Image.FromFile("cache/img/" + cou.CompetitionCountryId + ".png");
                  var country = _cachedCountries.Where(cc => cc.CountryId == cou.CompetitionCountryId).FirstOrDefault();
                  if (country != null)
                    c.ToolTipText = country.CountryName;
                }
                catch (Exception e)
                {
                  Logger.Exception(e);
                }
              }
            }
          }
        }
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
        if (_scheduleUpdateInProgress == true || _competitionUpdateInProgress == true)
          return;
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

          List<Team> teams = FindTeamsByName(SelectedGame.Home, SelectedGame.CompetitionId);
          if (teams.Count() == 0)
          {
            Team t = new Team();
            t.TeamName = SelectedGame.Home;
            t.CompetitionName = "NEMAPIRANO";

            teams.Add(t);
          }
          foreach (Team t in teams)
          {
            if (SelectedGameLinups != null)
              t.GameLineups = SelectedGameLinups.HomeLineup;
            t.Tag = "H";
            listBoxSource.Add(t);

          }

          teams = FindTeamsByName(SelectedGame.Away, SelectedGame.CompetitionId);
          if (teams.Count() == 0)
          {
            Team t = new Team();
            t.TeamName = SelectedGame.Away;
            t.CompetitionName = "NEMAPIRANO";
            teams.Add(t);
          }
          foreach (Team t in teams)
          {
            if (SelectedGameLinups != null)
              t.GameLineups = SelectedGameLinups.AwayLineup;
            t.Tag = "A";
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

        //if (lbArhiva.SelectedItem == null)
        //{
        //  MessageBox.Show("Nije izabrana arhiva lige u kojoj se klub takmiči!");
        //  return;
        //}

        var comp = _cachedCompetitions.Where(cc => cc.CompetitionId == _selectedTeam.CompetitionId).FirstOrDefault();

        if (comp == null)
        {
          MessageBox.Show("Izabrani tim nije mapiran. Mapiraj tim pa pokušaj opet!");
          return;
        }


        string archiveFileName = "cache\\arhiva\\" + comp.CompetitionCountry + "\\" + comp.CompetitionName + "\\" + lbArhiva.SelectedItem;

        //if (IsFileLocked(new FileInfo(archiveFileName)))
        //{
        //  MessageBox.Show("Arhiva '" + archiveFileName + "' je otvorena. Zatvori je pa pokusaj opet.");
        //  return;
        //}


        if (_selectedTeam == null)
          return;
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

        var pcd = _cachedStats.FirstOrDefault();
        if (pcd != null && cbCurrentSeason.SelectedIndex == 0)
        {
          int maxYear = int.Parse(pcd.Statistics.Max(s => s.Year));
          cbCurrentSeason.SelectedItem = maxYear + 1;
        }

        var currentSeasonData = new FileInfo("cache\\" + this._selectedTeam.TeamId.ToString() + "_tmp.txt");
        if (currentSeasonData.Exists == true)
          currentSeasonData.Delete();

        foreach (Player p in _selectedTeam.Players)
        {
          this.tbStatus.Text = (this._counter + 1).ToString() + " od " + this._selectedTeam.Players.Where(pl => pl.Lineup == TMS.Player.LineUpStatus.YES).Count() + " - " + p.Name;
          foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
          {
            CurrentPlayer = p;
            if (dgvr.DataBoundItem == CurrentPlayer)
              dgvPlayers.CurrentCell = dgvr.Cells[0];
          }

          await GetPlayersData(p);
          if (_generatingInProgress == false)
            return;
          UpdatePlayersGrid(p.PlayerId);
        }

        lbMatches.Enabled = true;
        dtpDatum.Enabled = true;
        this.lbTeams.Enabled = true;
        this.lbCompetition.Enabled = true;
        this.btnGenerateExcel.Enabled = true;
        btnAzurirajLigu.Enabled = true;
        btnStop.Visible = false;
        this.tbStatus.Visible = false;
        this.cbTeams.Enabled = true;
        this.cbCountries.Enabled = true;
        this.lbCountries.Enabled = true;
        btnArhiva.Enabled = true;
        btnAzurirajArhivu.Enabled = true;
        Guid.NewGuid();

        var fileName = archiveFileName;
        fileName = "cache\\arhiva\\" + comp.CompetitionCountry + "\\" + comp.CompetitionName + "\\" + _selectedTeam.TeamName + ".xlsx";
        DocumentBuilder.CreateCXMLDocument(fileName, this._selectedTeam);

        Process.Start(fileName);
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
      _generatingInProgress = false;


    }

    private void UpdatePlayersGrid(int playerId)
    {
      foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
      {
        Player player = (Player)dgvr.DataBoundItem;

        if (player.Statistics != null && player.PlayerId == playerId)
        {
          dgvr.Cells[3].Value = player.Statistics[0].GamesPlayed;
          dgvr.Cells[4].Value = player.Statistics[0].MinutesPlayed;
          dgvr.Cells[5].Value = player.Statistics[0].GoalsScored;
        }
      }
    }

    private void LoadDocuments()
    {
      try
      {
        List<FileInfo> allFiles = new List<FileInfo>();

        DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "//cache//xls//");
        if (di.Exists == false)
          di.Create();

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

    private void LoadArchive(Competition c)
    {
      try
      {
        if (c == null)
          return;
        List<FileInfo> allFiles = new List<FileInfo>();

        DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "//cache//arhiva//" + c.CompetitionCountry + "//" + c.CompetitionName);
        if (di.Exists == false)
          di.Create();

        foreach (FileInfo fi in di.GetFiles().Where(f => f.Name.StartsWith("~") == false && f.Name.Contains("[" + c.CompetitionId + "]")))
          allFiles.Add(fi);

        lbArhiva.DataSource = allFiles.OrderByDescending(f => f.LastWriteTime).ToList();
        lbArhiva.ValueMember = "Name";
        lbArhiva.DisplayMember = "Name";
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

    private async void btnArhiva_Click(object sender, EventArgs e)
    {
      try
      {
        btnGenerateExcel.Enabled = false;
        cbTeams.Enabled = false;
        lbTeams.Enabled = false;
        lbCompetition.Enabled = false;
        cbCountries.Enabled = false;
        lbCountries.Enabled = false;
        btnArhiva.Enabled = false;
        btnAzurirajArhivu.Enabled = false;
        _scheduleUpdateInProgress = true;
        btnDeleteFile.Enabled = false;

        string text = "cache\\arhiva\\" + _selectedCountry.CountryName + "\\" + _selectedCompetition.CompetitionName + "\\" + "[" + this._selectedCompetition.CompetitionId.ToString() + "] " + this._selectedCompetition.CompetitionName + " " + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx";

        foreach (Team t in _selectedCompetition.Teams)
        {
          List<Match> schedule = await DataLoader.LoadSchedule(_cachedTeams, t, cbCurrentSeason.SelectedItem.ToString());
          var season = cbCurrentSeason.SelectedItem;
          lbTeams.SelectedItem = t;
          cbCurrentSeason.SelectedItem = season;
          t.Schedule = schedule;
        }

        DocumentBuilder.CreateCompetitionArchive(text, _selectedCompetition);
        if (File.Exists(text))
        {
          Process.Start(text);
          LoadArchive(_selectedCompetition);
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
      }

      btnDeleteFile.Enabled = true;
      _scheduleUpdateInProgress = false;
      cbTeams.Enabled = true;
      lbTeams.Enabled = true;
      lbCompetition.Enabled = true;
      cbCountries.Enabled = true;
      lbCountries.Enabled = true;
      btnArhiva.Enabled = true;
      btnAzurirajArhivu.Enabled = true;
      _scheduleUpdateInProgress = false;
      btnGenerateExcel.Enabled = true;
      btnAzurirajLigu.Enabled = true;
    }

    private void lbArhiva_DoubleClick(object sender, EventArgs e)
    {
      if (lbArhiva.SelectedItem != null)
      {
        btnDeleteFile.Enabled = false;
        lbArhiva.Enabled = false;
        var a = lbArhiva.SelectedItem;
        Process.Start(((FileInfo)a).FullName);
        lbArhiva.Enabled = true;
        btnDeleteFile.Enabled = true;
      }
    }

    protected bool IsFileLocked(FileInfo file)
    {
      FileStream stream = null;

      try
      {
        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch (IOException)
      {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)
        return true;
      }
      finally
      {
        if (stream != null)
          stream.Close();
      }

      //file is not locked
      return false;
    }

    private async void btnAzurirajArhivu_Click(object sender, EventArgs e)
    {
      await AzurirajArhivu();
    }

    private async Task<bool> AzurirajArhivu()
    {
      try
      {
        string templateFileName = "cache\\arhiva\\Template.xlsx";

        if (IsFileLocked(new FileInfo(templateFileName)))
        {
          MessageBox.Show("Template.xlsx je otvoren. Zatvori ga pa pokusaj opet.");
          return false;
        }

       

    

        if (lbArhiva.SelectedItem != null)
        {
          string a = lbArhiva.SelectedItem.ToString();
          string competitionId = a.Substring(a.IndexOf("[") + 1);
          competitionId = competitionId.Substring(0, competitionId.IndexOf("]"));
          var competition = _cachedCompetitions.Where(cc => cc.CompetitionId == competitionId).FirstOrDefault();

          string archiveFileName = "cache\\arhiva\\" + competition.CompetitionCountry + "\\" + competition.CompetitionName + "\\" + lbArhiva.SelectedItem;

          if (IsFileLocked(new FileInfo(archiveFileName)))
          {
            MessageBox.Show("Arhiva '" + archiveFileName + "' je otvorena. Zatvori je pa pokusaj opet.");
            return false;
          }

          btnGenerateExcel.Enabled = false;
          btnAzurirajLigu.Enabled = false;
          _scheduleUpdateInProgress = true;
          cbTeams.Enabled = false;
          lbTeams.Enabled = false;
          lbCompetition.Enabled = false;
          cbCountries.Enabled = false;
          lbCountries.Enabled = false;
          btnArhiva.Enabled = false;
          btnAzurirajArhivu.Enabled = false;
          btnDeleteFile.Enabled = false;

          if (competition != null)
          {
            lbCompetition.SelectedItem = competition;
            //await LoadTeamsAsync(false);
            lbCompetition.Enabled = false;
            //var teams = _cachedTeams.Where(ct => ct.CompetitionId == competitionId).ToList();
            var teams = (List<Team>)lbTeams.DataSource;
            foreach (var t in teams)
            {
              List<Match> schedule = await DataLoader.LoadSchedule(_cachedTeams, t, cbCurrentSeason.SelectedItem.ToString());
              t.Schedule = schedule;
              lbTeams.SelectedItem = t;
            }
            competition.Teams = teams;

            DocumentBuilder.UpdateCompetitionArchive("cache\\arhiva\\" + competition.CompetitionCountry + "\\" + competition.CompetitionName + "\\" + a, competition);

            Process.Start("cache\\arhiva\\" + competition.CompetitionCountry + "\\" + competition.CompetitionName + "\\" + a);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
      }

      btnGenerateExcel.Enabled = true;
      btnAzurirajLigu.Enabled = true;
      btnDeleteFile.Enabled = true;
      cbTeams.Enabled = true;
      lbTeams.Enabled = true;
      lbCompetition.Enabled = true;
      cbCountries.Enabled = true;
      lbCountries.Enabled = true;
      btnArhiva.Enabled = true;
      btnAzurirajArhivu.Enabled = true;
      _scheduleUpdateInProgress = false;
      return true;
    }

    private void btnDeleteFile_Click(object sender, EventArgs e)
    {
      try
      {
        if (lbArhiva.SelectedItem != null)
        {
          string a = lbArhiva.SelectedItem.ToString();
          File.Delete("cache\\arhiva\\" + _selectedCountry.CountryName + "\\" + _selectedCompetition.CompetitionName + "\\" + a);
          LoadArchive(_selectedCompetition);
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void topToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (((Country)lbCountries.SelectedItem).CountryId != -1)
      {
        var c = lbCountries.SelectedItem;
        if (c != null)
        {
          var country = (Country)c;
          country.Top = !country.Top;
          UpdateCountryTop(country);
        }
      }
    }

    private void cmsTop_Opening(object sender, CancelEventArgs e)
    {
      if (((Country)lbCountries.SelectedItem).CountryId != -1)
      {
        var c = lbCountries.SelectedItem;
        if (c != null)
        {
          var country = (Country)c;
          topToolStripMenuItem.Checked = country.Top;
        }
      }
    }

    private void mapirajToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (SelectedGame == null)
        return;
      var competitionShortName = SelectedGame.LineupUrl.Split('/')[4] + "/" + SelectedGame.LineupUrl.Split('/')[5];

      frmCompetitionMapping frm = new frmCompetitionMapping();
      frm.CachedCompetitions = this._cachedCompetitions;
      frm.CompetitionToMap = competitionShortName;
      DialogResult dr = frm.ShowDialog();
      if (dr == System.Windows.Forms.DialogResult.OK)
      {
        Competition mappedCompetition = frm.MappedComeptition;
        mappedCompetition.AlternativeName = competitionShortName;
        UpdateCompetitionAlternativeName(mappedCompetition);
        //this._cachedCompetitions = this.GetCachedCompetitions();
        //lbCompetition.DataSource = _cachedCompetitions;
        //this.lbCompetition.DisplayMember = "CompetitionName";
        //this.lbCompetition.ValueMember = "CompetitionId";
      }

    }



    private void rbIzabranaLiga_Click(object sender, EventArgs e)
    {
      StartLivescoreFeedAsync();
    }

    private void rbSve_Click(object sender, EventArgs e)
    {
      StartLivescoreFeedAsync();
    }

    private void cbCurrentSeason_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (lbTeams.SelectedItem != null)
      {
        var selectedTeam = (Team)lbTeams.SelectedItem;
        if (cbCurrentSeason.SelectedIndex == 1)
        {
          this._cachedStats = this.LoadCachedData(this._selectedTeam.TeamId);
          var pcd = _cachedStats.FirstOrDefault();
          if (pcd != null)
          {
            int maxYear = int.Parse(pcd.Statistics.Max(s => s.Year));
            if (maxYear == (int)cbCurrentSeason.SelectedItem)
              File.Delete("cache\\" + this._selectedTeam.TeamId.ToString() + ".txt");
          }
        }
      }
    }

    private async void btnAzurirajLigu_Click(object sender, EventArgs e)
    {
      bool status = await AzurirajArhivu();
      if (status == false)
        return;
      btnAzurirajArhivu.Enabled = false;
      btnArhiva.Enabled = false;
      btnAzurirajLigu.Enabled = false;
      lbTeams.Enabled = false;
      _competitionUpdateInProgress = true;

      for (int i = 0; i < lbTeams.Items.Count; i++)
      {
        lbTeams.SelectedIndex = i;
        await LoadPlayers();
        if (_competitionUpdateInProgress == false)
          break;
        await GenerateExcel();
        if (_competitionUpdateInProgress == false)
          break;
      }

      btnAzurirajArhivu.Enabled = true;
      btnArhiva.Enabled = true;
      btnAzurirajLigu.Enabled = true;
      lbTeams.Enabled = true;
      _competitionUpdateInProgress = false;

    }

  

    private async void gledajToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var url = await DataLoader.GetStream(SelectedGame.Home, SelectedGame.Away);
      if (url != null)
        Process.Start(url);
      else
        MessageBox.Show("Nije pronadjen stream!");
    }
  }
}
