using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMS
{
  public partial class frmTeamMapping : Form
  {
    public string TeamToMap { get; set; }
    public List<Team> CachedTeams { get; set; }
    public Team MappedTeam { get; set; }
    public string CompetitionId { get; set; }

    public frmTeamMapping()
    {
      InitializeComponent();
    }

    private void cbTeams_TextUpdate(object sender, EventArgs e)
    {
      string text = this.cbTeams.Text;
      List<Team> dataSource = (
          from ts in this.CachedTeams
          where (ts.CompetitionId==CompetitionId || CompetitionId==null)&& ts.TeamName.ToUpper().Contains(this.cbTeams.Text.ToUpper()) 
          select ts).ToList<Team>();
      this.lbTeams.ValueMember = "TeamId";
      this.lbTeams.DisplayMember = "TeamFullName";
      this.lbTeams.DataSource = dataSource;
      this.cbTeams.Text = text;
      this.cbTeams.Select(this.cbTeams.Text.Length, 0);
    }

    private void frmTeamMapping_Load(object sender, EventArgs e)
    {
      tbTeamToMap.Text = TeamToMap;
      cbTeams.Focus();
      cbTeams.Select();
      this.cbTeams.DataSource = this.CachedTeams;
      this.cbTeams.DisplayMember = "TeamName";
      this.cbTeams.ValueMember = "TeamId";
      this.cbTeams.SelectedIndex = -1;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.OK;
      if (lbTeams.SelectedItem != null)
        MappedTeam = (Team)lbTeams.SelectedItem;
      Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void lbTeams_DoubleClick(object sender, EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.OK;
      if (lbTeams.SelectedItem != null)
        MappedTeam = (Team)lbTeams.SelectedItem;
      Close();
    }

    private void lbTeams_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Return)
      {
        DialogResult = System.Windows.Forms.DialogResult.OK;
        if (lbTeams.SelectedItem != null)
          MappedTeam = (Team)lbTeams.SelectedItem;
        Close();
      }
    }
  }
}
