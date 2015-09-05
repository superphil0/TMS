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

    //private Task _geTask;
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
        if (File.Exists(Application.StartupPath + Helper.GetTeamListFileName()))
        {
          StreamReader streamReader = new StreamReader(Application.StartupPath + Helper.GetTeamListFileName());
          while (!streamReader.EndOfStream)
          {
            text = streamReader.ReadLine();
            string[] array = text.Split('|');

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
        if (File.Exists(Application.StartupPath + Helper.GetCountryListFileName()))
        {
          StreamReader streamReader = new StreamReader(Application.StartupPath + Helper.GetCountryListFileName());
          while (!streamReader.EndOfStream)
          {
            string text = streamReader.ReadLine();
            string[] array = text.Split('|');
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

        if (File.Exists(Application.StartupPath + Helper.GetCompetitionListFileName()))
        {
          StreamReader streamReader = new StreamReader(Application.StartupPath + Helper.GetCompetitionListFileName());
          while (!streamReader.EndOfStream)
          {
            string text = streamReader.ReadLine();
            string[] array = text.Split('|');

            var c = new Competition
            {
              CompetitionId = array[2],
              CompetitionName = array[3],
              CompetitionCountryId = int.Parse(array[0])
            };

            if (array.Length > 4)
            {
              var anames = array[4].Split(',');
              foreach (var an in anames)
                c.AlternativeName.Add(an);
            }

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

    private async void lbCountries_Click(object sender, EventArgs e)
    {
      if (((Country)lbCountries.SelectedItem).CountryId != -1)
      {
        await LoadCompetitionsAsync();
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
          StreamWriter streamWriter = new StreamWriter(Application.StartupPath + Helper.GetCountryListFileName(), true);
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
          StreamWriter streamWriter = new StreamWriter(Application.StartupPath + Helper.GetCompetitionListFileName(), true);
          foreach (Competition c in _selectedCountry.Competitions)
          {

            if (c.CompetitionName.Equals("NATIONAL"))
            {
              StreamWriter sw = new StreamWriter(Application.StartupPath + Helper.GetTeamListFileName(), true);
              foreach (Team t in c.Teams)
              {
                t.CompetitionName = c.CompetitionName;
                if ((
                  from cc in this._cachedTeams
                  where cc.CompetitionId == t.CompetitionId && cc.TeamId == t.TeamId
                  select cc).ToList<Team>().Count == 0)
                {
                  this._cachedTeams.Add(t);
                  sw.WriteLine(t.CompetitionId.ToString() + "|" + t.CompetitionName + "|" + t.TeamId.ToString() + "|" + t.TeamName + "|" + t.UrlName);
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
              streamWriter.WriteLine(_selectedCountry.CountryId.ToString() + "|" + _selectedCountry.CountryName + "|" + c.CompetitionId.ToString() + "|" + c.CompetitionName);
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

  

    private async void lbCompetition_Click(object sender, EventArgs e)
    {
      if (lbCompetition.SelectedItem != null)
        await LoadTeamsAsync();
    }

    #endregion Competitions

    #region Teams

    private async Task<List<Team>> LoadTeamsAsync(bool changeCompetition = true)
    {
      List<Team> teamList = new List<Team>();
      try
      {
        //if (_competitionUpdateInProgress == true)
        //  return teamList;
        if (changeCompetition == true)
          this._selectedCompetition = (Competition)this.lbCompetition.SelectedItem;
        this.lbTeams.DataSource = null;
        this.lbTeams.DisplayMember = "TeamName";
        this.lbTeams.ValueMember = "TeamId";
        this.pbLoadingTeams.Visible = true;
        //lbCompetition.Enabled = false;

        teamList = (
          from cc in this._cachedTeams
          where cc.CompetitionId == this._selectedCompetition.CompetitionId
          select cc).ToList<Team>();
        if (teamList.Count == 0)
        {
          List<Team> teams = null;
          if (_selectedCompetition.CompetitionCountryId < 0)//KUP TAKMICENJA
          {
            teams = await DataLoader.LoadTeamsAsync(this._selectedCompetition.CompetitionId);
            if (teams.Count == 0)
              teams = await DataLoader.LoadTeamsSearchPageAsync(this._selectedCompetition.CompetitionId);
          }
          else
            teams = await DataLoader.LoadTeamsAlternativeAsync(this._selectedCompetition.CompetitionId);
          this._selectedCompetition.Teams = teams;

          DirectoryInfo directoryInfo = new DirectoryInfo("cache");
          if (!directoryInfo.Exists)
          {
            directoryInfo.Create();
          }
          StreamWriter streamWriter = new StreamWriter(Application.StartupPath + Helper.GetTeamListFileName(), true);
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
                teamList.Add(t);
                this._cachedTeams.Add(t);
                streamWriter.WriteLine(_selectedCompetition.CompetitionId.ToString() + "|" + _selectedCompetition.CompetitionName + "|" + t.TeamId.ToString() + "|" + t.TeamName + "|" + t.UrlName);
              }
            }
          }
          streamWriter.Close();
        }
        foreach (var el in teamList)
        {
          el.Url = "http://www.transfermarkt.de/jumplist/startseite/verein/" + el.TeamId;
        }
        this._selectedCompetition.Teams = teamList;
        //lbCompetition.Enabled = true;
        this.pbLoadingTeams.Visible = false;
        this.lbTeams.DataSource = this._selectedCompetition.Teams;
        if (this._selectedCompetition.Teams.Count > 0)
        {
          this.lbTeams.SelectedIndex = 0;
          this.lbTeams.Focus();
        }

        LoadArchive(_selectedCompetition);
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
      return teamList;
    }

    private async void lbTeams_Click(object sender, EventArgs e)
    {
      if (_generatingInProgress == false && lbTeams.SelectedItem != null)
      {
        AllowControls(false);
        _selectedTeam = (Team)this.lbTeams.SelectedItem;
        if (_selectedTeam.Schedule == null)
        {
          _scheduleUpdateInProgress = true;

          List<Match> schedule = await DataLoader.LoadSchedule(_cachedTeams, _selectedTeam, cbCurrentSeason.SelectedItem.ToString());
          _selectedTeam.Schedule = schedule;

          _scheduleUpdateInProgress = false;
        }
        await LoadPlayers(_selectedTeam);
        AllowControls(true);
      }
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


    private void lbTeams_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_generatingInProgress == false && _playerLoadingInProgress == false && _scheduleUpdateInProgress == false && _competitionUpdateInProgress == false)
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
        StreamReader sr = new StreamReader(Application.StartupPath + Helper.GetTeamListFileName());
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

        StreamWriter sw = new StreamWriter(Application.StartupPath + Helper.GetTeamListFileName());
        foreach (string s in lines)
        {
          sw.WriteLine(s);
        }
        sw.Close();
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
    }


    private void UpdateCompetitionAlternativeName(Competition c)
    {
      try
      {
        StreamReader sr = new StreamReader(Application.StartupPath + Helper.GetCompetitionListFileName());
        List<string> lines = new List<string>();
        while (sr.EndOfStream == false)
        {
          string line = sr.ReadLine();
          if (line.Contains(c.CompetitionId + "|" + c.CompetitionName))
          {
            var parts = line.Split('|');

            var anames = "";
            foreach (var an in c.AlternativeName)
              anames += "," + an;
            anames = anames.Substring(1);

            if (parts.Count() == 4)
              line += "|" + anames;
            else
              line = line.Substring(0, line.LastIndexOf("|") + 1) + anames;
          }
          lines.Add(line);
        }
        sr.Close();

        StreamWriter sw = new StreamWriter(Application.StartupPath + Helper.GetCompetitionListFileName());
        foreach (string s in lines)
        {
          sw.WriteLine(s);
        }
        sw.Close();
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
    }



    private void UpdateCountryTop(Country c)
    {
      try
      {
        StreamReader sr = new StreamReader(Application.StartupPath + Helper.GetCountryListFileName());
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

        StreamWriter sw = new StreamWriter(Application.StartupPath + Helper.GetCountryListFileName());
        foreach (string s in lines)
          sw.WriteLine(s);
        sw.Close();
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
    }

    private List<Team> FindTeamsByName(string teamName, string competitionId = null)
    {
      var teams = _cachedTeams.Where(tt => tt.AlternativeName != null && tt.AlternativeName.Equals(teamName) && (tt.CompetitionId == competitionId || competitionId == null));
      if (teams.Count() != 0)
        return teams.ToList();
      else
        return _cachedTeams.Where(tt => Helper.RemoveDiacritics(tt.TeamName, false).Contains(teamName) && (tt.CompetitionId == competitionId || competitionId == null)).ToList();
    }

    private async void mapirajTimToolStripMenuItem_Click(object sender, EventArgs e)
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
          await LoadLineups();
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
        await LoadLineups();
      }
    }

    #endregion Teams

    #region Players

    private async Task LoadPlayers(Team team)
    {
      try
      {
        _playerLoadingInProgress = true;

        lbUnamapped.Visible = false;
        lblUnmapped.Visible = false;
        this.dgvPlayers.DataSource = null;
        this.tbStatus.Clear();

        this.pbLoadingPlayers.Visible = true;

        if (team.Players == null)
          team.Players = await DataLoader.LoadPlayersAsync(team.TeamId);

        _cachedStats = LoadCachedData(team);

        foreach (var p in team.Players)
        {
          var pl = _cachedStats.Where(cs => cs.PlayerId == p.PlayerId).FirstOrDefault();
          if (pl != null)
            p.CurrentSeasonStatistics = pl.CurrentSeasonStatistics;
        }

        if (team.GameLineups != null)
        {
          List<string> matchedPlayers = SetLineupStatus(team);
          List<string> unmatchedPlayers = new List<string>();
          foreach (string s in team.GameLineups)
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
          DetermineLineupStatus(team);
          lbUnamapped.Visible = false;
          lblUnmapped.Visible = false;

        }

        this.pbLoadingPlayers.Visible = false;
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
        c.Width = 40;
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


        dgvPlayers.DataSource = team.Players;
        foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
        {
          dgvr.Cells[7].Style = new DataGridViewCellStyle()
          {
            Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Italic)
          };
          Player player = (Player)dgvr.DataBoundItem;

          if (player.Injury != null)
            for (int i = 0; i < dgvr.Cells.Count; i++)
              dgvr.Cells[i].ToolTipText = player.Injury;

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
          else if (player.Lineup == Player.LineUpStatus.UNKNOWN)
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

        for (int i = 0; i < team.Players.Count; i++)
        {
          string text2 = team.Players[i].TmUrl.Substring(1);
          string text3 = text2.Substring(0, text2.IndexOf("/"));
          team.Players[i].PlayerStatsUrl = text3 + "/leistungsdaten/spieler/" + team.Players[i].PlayerId + "/saison/" + text + "/plus/1";
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
      _playerLoadingInProgress = false;
    }

    private async Task GetPlayersData(Player p, Team team)
    {
      StreamWriter streamWriter = null, currentSeasonData = null;
      try
      {
        if (p.Lineup == Player.LineUpStatus.YES)
        {
          Statistics statistics = new Statistics();

          p.Statistics = new List<Statistics>();
          p.Matches = new List<Match>();
          streamWriter = new StreamWriter(Application.StartupPath + Helper.GetTeamDataFileName(team), true);

          currentSeasonData = new StreamWriter(Application.StartupPath + Helper.GetTeamTempDataFileName(team), true);

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
                if (list.First().Statistics != null)
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

    private List<Player> LoadCachedData(Team team)
    {
      this._cachedStats = new List<Player>();

      try
      {
        //LOADING PREVIOUS SEASON DATA
        if (File.Exists(Application.StartupPath + Helper.GetTeamDataFileName(team)))
        {
          StreamReader streamReader = new StreamReader(Application.StartupPath + Helper.GetTeamDataFileName(team));
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
        if (File.Exists(Application.StartupPath + Helper.GetTeamTempDataFileName(team)))
        {
          StreamReader streamReader = new StreamReader(Application.StartupPath + Helper.GetTeamTempDataFileName(team));
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
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
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
        string lurl = "";
        if (g.LineupUrl.IndexOf("soccer/") != -1)
          lurl = g.LineupUrl.Substring(g.LineupUrl.IndexOf("soccer/") + 7);
        else
          lurl = g.LineupUrl.Substring(g.LineupUrl.IndexOf(".com/") + 5);

        var competition = _cachedCompetitions.Where(cc => cc.AlternativeName.Count > 0 && cc.AlternativeName.Contains(lurl.Split('/')[0] + "/" + lurl.Split('/')[1])).FirstOrDefault();
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

        string lurl = "";
        if (g.LineupUrl.IndexOf("soccer/") != -1)
          lurl = g.LineupUrl.Substring(g.LineupUrl.IndexOf("soccer/") + 7);
        else
          lurl = g.LineupUrl.Substring(g.LineupUrl.IndexOf(".com/") + 5);
        var tt = lurl.Split('/')[0].Replace("-", " ").ToUpper() + " - " + lurl.Split('/')[1].Replace("-", " ").ToUpper();

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
              catch (Exception ex)
              {
                Logger.Exception(ex);
                MessageBox.Show(ex.Message);
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
                catch (Exception ex)
                {
                  Logger.Exception(ex);
                  MessageBox.Show(ex.Message);
                }
              }
            }
          }
        }
      }

      pbLivescore.Visible = false;
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
     await StartLivescoreFeedAsync();
    }

    private async void rbSlijedi_Click(object sender, EventArgs e)
    {
      await StartLivescoreFeedAsync();
    }

    private async void rbLive_Click(object sender, EventArgs e)
    {
      dtpDatum.Value = DateTime.Now;
      await StartLivescoreFeedAsync();
    }

    private async void rbCompleted_Click(object sender, EventArgs e)
    {
      await StartLivescoreFeedAsync();
    }

    private async void tmrReload_Tick_1(object sender, EventArgs e)
    {
      await StartLivescoreFeedAsync();
    }



    private async void lbMatches_CellClick(object sender, DataGridViewCellEventArgs e)
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
        if (_scheduleUpdateInProgress == true || _competitionUpdateInProgress == true || _generatingInProgress == true || _playerLoadingInProgress == true)
          return;
        await LoadLineups();
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

    private void DetermineLineupStatus(Team t)
    {
      try
      {
        foreach (var p in t.Players)
        {
          var comment = p.Injury;
          p.Lineup = Player.LineUpStatus.YES;

          #region position
          //NO POSITION SET
          var position = p.MainPosition;
          if (position == "")
            p.Lineup = Player.LineUpStatus.UNKNOWN;
          #endregion positions

          if (comment != null && position != "GK")
          {
            string dateS;
            DateTime date;
            bool status;
            var nextGame = t.Schedule.Where(s => s.Date > DateTime.Now).OrderBy(s => s.Date).FirstOrDefault();
            #region suspension
            if (comment.ToLower().Contains("suspension") || comment.ToLower().Contains("leave") || comment.ToLower().Contains("no eligibility"))
            {
              string[] parts = Regex.Split(comment, " - ");
              var suspensionUntil = parts[1];
              var competitionName = parts[2];


              if (suspensionUntil != "")
              {
                suspensionUntil = comment.Substring(comment.ToLower().IndexOf(" until ") + 7);
                dateS = suspensionUntil.Substring(0, suspensionUntil.IndexOf(" - ")).Trim();
                status = DateTime.TryParseExact(dateS, "MMM d, yyyy", null, DateTimeStyles.None, out date);
                if (status == true)
                {
                  if (nextGame != null)
                  {
                    if (nextGame.Date.HasValue && nextGame.Date < date && (t.CompetitionName == competitionName || competitionName == "cross-competition"))
                      p.Lineup = Player.LineUpStatus.NO;
                  }
                  else
                  {
                    if (t.CompetitionName == competitionName || competitionName == "cross-competition")
                      p.Lineup = Player.LineUpStatus.UNKNOWN;
                  }
                }
                else
                  p.Lineup = Player.LineUpStatus.UNKNOWN;

              }
              else
              {
                if (t.CompetitionName == competitionName || competitionName == "cross-competition")
                  p.Lineup = Player.LineUpStatus.NO;
              }
            }
            #endregion suspension

            #region injury
            //INJURY
            if (comment.ToLower().Contains("return unknown"))
            {
              p.Lineup = Player.LineUpStatus.NO;
            }
            else if (comment.ToLower().Contains("return"))
            {
              if (comment.ToLower().IndexOf(" on ") != -1)
              {
                dateS = comment.Substring(comment.ToLower().IndexOf(" on ") + 4);
                status = DateTime.TryParseExact(dateS, "MMM d, yyyy", null, DateTimeStyles.None, out date);
                if (status == true)
                {
                  //var nextGame = t.Schedule.Where(s => s.Date > DateTime.Now).OrderBy(s=>s.Date).FirstOrDefault();
                  if (nextGame != null)
                  {
                    if (nextGame.Date.HasValue && nextGame.Date < date)
                      p.Lineup = Player.LineUpStatus.NO;
                  }
                  else
                    p.Lineup = Player.LineUpStatus.UNKNOWN;
                }
                else
                  p.Lineup = Player.LineUpStatus.UNKNOWN;
              }
              else
                p.Lineup = Player.LineUpStatus.UNKNOWN;
            }
          }
          #endregion injury
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
    }

    private List<string> SetLineupStatus(Team team)
    {
      List<string> matchedPlayers = new List<string>();
      try
      {
        List<string> gameLineups;

        if (team.GameLineups == null)
          return matchedPlayers;
        else
          gameLineups = team.GameLineups.ConvertAll(d => d.ToLower());
        foreach (Player p in team.Players)
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
                      p.Lineup = Player.LineUpStatus.UNKNOWN;
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
                      p.Lineup = Player.LineUpStatus.UNKNOWN;
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

    private async void btnGenerateExcel_Click(object sender, EventArgs e)
    {
      _generatingInProgress = true;
      AllowControls(false);
      await GenerateExcel(_selectedTeam);
      AllowControls(true);
      _generatingInProgress = false;
    }

    private async Task GenerateExcel(Team team, bool automatic = false)
    {
      try
      {
        var comp = _cachedCompetitions.Where(cc => cc.CompetitionId == team.CompetitionId).FirstOrDefault();

        if (comp == null)
        {
          MessageBox.Show("Izabrani tim nije mapiran. Mapiraj tim pa pokušaj opet!");
          return;
        }

        var fileName = Application.StartupPath + Helper.GetTeamArchiveFileName(comp, team);

        if (File.Exists(fileName))
          if (Helper.IsFileLocked(new FileInfo(fileName)))
          {
            MessageBox.Show("Arhiva '" + fileName + "' je otvorena. Zatvori je pa pokusaj opet.");
            return;
          }

        if (team == null)
          return;

        if (team.Players == null)
          return;

        if (automatic == false)
        {
          foreach (DataGridViewRow r in dgvPlayers.Rows)
          {
            Player p = (Player)r.DataBoundItem;
            team.Players.Where(pp => pp.PlayerId == p.PlayerId).FirstOrDefault().Lineup = p.Lineup;
          }
        }

        if (automatic == false)
        {
          if (team.GameLineups != null &&
              team.Players.Where(p => p.Lineup == Player.LineUpStatus.YES).Count() < 11)
          {
            DialogResult dr =
              MessageBox.Show(
                "Neki od igraca iz najavljenog sastava nisu mapirani. Nastaviti generisanje dokumenta bez njih ?",
                "Pitanje", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes)
              return;
          }
        }

        this._counter = 0;
        this.tbStatus.Visible = true;
        this._cachedStats = LoadCachedData(team);

        var pcd = _cachedStats.FirstOrDefault();
        if (pcd != null && cbCurrentSeason.SelectedIndex == 0)
        {
          int maxYear = int.Parse(pcd.Statistics.Max(s => s.Year));
          cbCurrentSeason.SelectedItem = maxYear + 1;
        }

        var currentSeasonData = new FileInfo(Application.StartupPath + Helper.GetTeamTempDataFileName(team));
        if (currentSeasonData.Exists == true)
          currentSeasonData.Delete();

        foreach (Player p in team.Players)
        {
          this.tbStatus.Text = (this._counter + 1).ToString() + " od " + team.Players.Where(pl => pl.Lineup == TMS.Player.LineUpStatus.YES).Count() + " - " + p.Name;
          if (automatic == false)
          {
            foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
            {
              CurrentPlayer = p;
              if (dgvr.DataBoundItem == CurrentPlayer)
                dgvPlayers.CurrentCell = dgvr.Cells[0];
            }
          }

          await GetPlayersData(p, team);
          if (_generatingInProgress == false)
            return;
          if (automatic == false)
            UpdatePlayersGrid(p.PlayerId);
        }

        DocumentBuilder.CreateCXMLDocument(fileName, team);

        Process.Start(fileName);
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }
    }

    private void UpdatePlayersGrid(int playerId)
    {
      foreach (DataGridViewRow dgvr in dgvPlayers.Rows)
      {
        Player player = (Player)dgvr.DataBoundItem;

        if (player.Statistics != null && player.Statistics.Count > 0 && player.PlayerId == playerId)
        {
          dgvr.Cells[3].Value = player.Statistics[0].GamesPlayed;
          dgvr.Cells[4].Value = player.Statistics[0].MinutesPlayed;
          dgvr.Cells[5].Value = player.Statistics[0].GoalsScored;
        }
      }
    }

    private void LoadArchive(Competition c)
    {
      try
      {
        if (c == null)
          return;
        List<FileInfo> allFiles = new List<FileInfo>();

        DirectoryInfo di = new DirectoryInfo(Application.StartupPath + Helper.GetCompetitionArchiveDirectoryName(c));
        if (di.Exists == false)
          di.Create();

        foreach (FileInfo fi in di.GetFiles().Where(f => f.Name == "_" + c.CompetitionName.NormalizeString().ToUpper() + ".xlsx"))
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
      _scheduleUpdateInProgress = true;
      AllowControls(false);
      await KreirajArhivu(_selectedCompetition);
      _scheduleUpdateInProgress = false;
      AllowControls(true);
    }

    private async Task KreirajArhivu(Competition c)
    {
      try
      {
        var archiveFileName = Application.StartupPath + Helper.GetCompetitionArchiveFileName(c);
        if (File.Exists(archiveFileName) && Helper.IsFileLocked(new FileInfo(archiveFileName)))
        {
          MessageBox.Show("Arhiva '" + archiveFileName + "' je otvorena. Zatvori je pa pokusaj opet.");
          return;
        }

        if (File.Exists(archiveFileName) == true)
        {
          DialogResult dr =
          MessageBox.Show(
            "Arhiva '" + archiveFileName + "' vec postoji! Da li treba nabraviti novu i obrisati postojecu ?",
            "Pitanje", MessageBoxButtons.YesNo);
          if (dr == DialogResult.No)
            return;
        }
        foreach (Team t in c.Teams)
        {
          if (t.Schedule == null)
          {
            List<Match> schedule = await DataLoader.LoadSchedule(_cachedTeams, t, cbCurrentSeason.SelectedItem.ToString());
            t.Schedule = schedule;
          }
          var season = cbCurrentSeason.SelectedItem;
          lbTeams.SelectedItem = t;
          cbCurrentSeason.SelectedItem = season;

        }

        DocumentBuilder.CreateCompetitionArchive(archiveFileName, c);
        if (File.Exists(archiveFileName))
        {
          Process.Start(archiveFileName);
          LoadArchive(c);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        Logger.Exception(ex);
      }
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

    private async void btnAzurirajArhivu_Click(object sender, EventArgs e)
    {
      _scheduleUpdateInProgress = true;
      AllowControls(false);
      await AzurirajArhivu(_selectedTeam);
      _scheduleUpdateInProgress = false;
      AllowControls(true);
    }

    private async Task<bool> AzurirajArhivu(Team team)
    {
      try
      {

        if (Helper.IsFileLocked(new FileInfo(Application.StartupPath + Helper.GetTemplateFileName())))
        {
          MessageBox.Show("Template.xlsx je otvoren. Zatvori ga pa pokusaj opet.");
          return false;
        }

        Competition competition = _cachedCompetitions.Where(cc => cc.CompetitionId == team.CompetitionId).FirstOrDefault();

        if (competition == null)
          return false;

        string archiveFileName = Application.StartupPath + Helper.GetCompetitionArchiveFileName(competition);

        if (File.Exists(archiveFileName) == false)
        {
          DialogResult dr =
          MessageBox.Show(
            "Ne postoji arhiva " + archiveFileName + "! Da li je treba napraviti ?",
            "Pitanje", MessageBoxButtons.YesNo);
          if (dr == DialogResult.Yes)
          {
            await LoadTeamsAsync(false);
            await KreirajArhivu(competition);
          }
          return true;
        }

        if (Helper.IsFileLocked(new FileInfo(archiveFileName)))
        {
          MessageBox.Show("Arhiva '" + archiveFileName + "' je otvorena. Zatvori je pa pokusaj opet.");
          return false;
        }

        if (competition != null)
        {
          lbCompetition.SelectedItem = competition;
          await LoadTeamsAsync(false);
          var teams = _cachedTeams.Where(ct => ct.CompetitionId == competition.CompetitionId).ToList();
          foreach (var t in teams)
          {
            if (t.Schedule == null)
            {
              List<Match> schedule = await DataLoader.LoadSchedule(_cachedTeams, t, cbCurrentSeason.SelectedItem.ToString());
              t.Schedule = schedule;
            }
            lbTeams.SelectedItem = t;
          }
          competition.Teams = teams;

          DocumentBuilder.UpdateCompetitionArchive(archiveFileName, competition);
          Process.Start(archiveFileName);
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
      }

      return true;
    }

    private void btnDeleteFile_Click(object sender, EventArgs e)
    {
      try
      {
        if (lbArhiva.SelectedItem != null)
        {
          string a = lbArhiva.SelectedItem.ToString();
          File.Delete(Application.StartupPath + Helper.GetCompetitionArchiveDirectoryName(_selectedCompetition) + "\\" + a);
          LoadArchive(_selectedCompetition);
        }
      }
      catch (Exception ex)
      {
        Logger.Exception(ex);
        MessageBox.Show(ex.Message);
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

      string lurl = "";
      if (SelectedGame.LineupUrl.IndexOf("soccer/") != -1)
        lurl = SelectedGame.LineupUrl.Substring(SelectedGame.LineupUrl.IndexOf("soccer/") + 7);
      else
        lurl = SelectedGame.LineupUrl.Substring(SelectedGame.LineupUrl.IndexOf(".com/") + 5);

      var competitionShortName = lurl.Split('/')[0] + "/" + lurl.Split('/')[1];

      frmCompetitionMapping frm = new frmCompetitionMapping();
      frm.CachedCompetitions = this._cachedCompetitions;
      frm.CompetitionToMap = competitionShortName;
      DialogResult dr = frm.ShowDialog();
      if (dr == System.Windows.Forms.DialogResult.OK)
      {
        Competition mappedCompetition = frm.MappedComeptition;
        mappedCompetition.AlternativeName.Add(competitionShortName);
        UpdateCompetitionAlternativeName(mappedCompetition);
      }
    }

    private async void rbIzabranaLiga_Click(object sender, EventArgs e)
    {
      await StartLivescoreFeedAsync();
    }

    private async void rbSve_Click(object sender, EventArgs e)
    {
      await StartLivescoreFeedAsync();
    }

    private void cbCurrentSeason_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        if (lbTeams.SelectedItem != null)
        {
          var selectedTeam = (Team)lbTeams.SelectedItem;
          if (cbCurrentSeason.SelectedIndex == 1)
          {
            this._cachedStats = this.LoadCachedData(_selectedTeam);
            var pcd = _cachedStats.FirstOrDefault();
            if (pcd != null && pcd.Statistics != null)
            {
              int maxYear = int.Parse(pcd.Statistics.Max(s => s.Year));
              if (maxYear == (int)cbCurrentSeason.SelectedItem)
              {
                File.Delete(Application.StartupPath + Helper.GetTeamDataFileName(selectedTeam));
                File.Delete(Application.StartupPath + Helper.GetTeamTempDataFileName(selectedTeam));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        Logger.Exception(ex);
      }
    }

    private void AllowControls(bool allow)
    {
      btnAzurirajArhivu.Enabled = allow;
      btnArhiva.Enabled = allow;
      btnAzurirajLigu.Enabled = allow;
      lbTeams.Enabled = allow;
      btnGenerateExcel.Enabled = allow;
      cbTeams.Enabled = allow;
      lbCompetition.Enabled = allow;
      cbCountries.Enabled = allow;
      lbCountries.Enabled = allow;
      btnDeleteFile.Enabled = allow;
      cbCurrentSeason.Enabled = allow;
      btnStop.Visible = !allow;
    }

    private async void btnAzurirajLigu_Click(object sender, EventArgs e)
    {
      var existingTeams = (List<Team>)lbTeams.DataSource;

      _competitionUpdateInProgress = true;
      AllowControls(false);
      bool status = await AzurirajArhivu(_selectedTeam);

      if (status == true)
      {
        lbTeams.DataSource = existingTeams;

        for (int i = 0; i < lbTeams.Items.Count; i++)
        {
          _selectedTeam = existingTeams[i];
          lbTeams.SelectedIndex = i;
          await LoadPlayers(_selectedTeam);

          if (_competitionUpdateInProgress == false)
            break;
          _generatingInProgress = true;
          await GenerateExcel(_selectedTeam, false);
          _generatingInProgress = false;
          if (_competitionUpdateInProgress == false)
            break;
        }
      }

      _competitionUpdateInProgress = false;
      AllowControls(true);
    }

    private async void gledajToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (lbMatches.SelectedRows.Count > 0)
      {
        Game g = (Game)lbMatches.SelectedRows[0].DataBoundItem;

        var url = await DataLoader.GetStream(g.Home, g.Away);
        if (url != null)
          Process.Start(url);
        else
          MessageBox.Show("Nije pronadjen stream!");
      }
    }

    private async void pogledajGoloveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (lbMatches.SelectedRows.Count > 0)
      {
        Game g = (Game)lbMatches.SelectedRows[0].DataBoundItem;

        var url = await DataLoader.GetFootage(g.Home, g.Away, dtpDatum.Value);
        if (url != null)
          Process.Start(url);
        else
          MessageBox.Show("Nije pronadjen link!");
      }
    }
  }
}
