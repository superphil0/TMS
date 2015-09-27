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
  public partial class frmCompetitionSelection : Form
  {
    public List<Competition> Competitions { get; set; }
    public List<Competition> SelectedCompetitions { get; set; }

    public frmCompetitionSelection()
    {
      InitializeComponent();
    }

    private void frmTeamMapping_Load(object sender, EventArgs e)
    {
      cbCompetitions.Focus();
      cbCompetitions.Select();
      this.cbCompetitions.DataSource = this.Competitions;
      this.cbCompetitions.DisplayMember = "CompetitionFullName";
      this.cbCompetitions.ValueMember = "CompetitionId";
      this.cbCompetitions.SelectedIndex = -1;


      for (int i = 0; i < cbCompetitions.Items.Count; i++)
        cbCompetitions.SetItemChecked(i, true);
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      SelectedCompetitions = new List<Competition>();
      DialogResult = System.Windows.Forms.DialogResult.OK;
      for (int i = 0; i < cbCompetitions.Items.Count; i++)
      {
        bool status = cbCompetitions.GetItemChecked(i);
        if(status==true)
        {
          SelectedCompetitions.Add((Competition)cbCompetitions.Items[i]);
        }
      }


      Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }


  }
}
