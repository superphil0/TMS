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
    public partial class frmCompetitionMapping : Form
    {
        public string CompetitionToMap { get; set; }
        public List<Competition> CachedCompetitions { get; set; }
        public Competition MappedComeptition { get; set; }

        public frmCompetitionMapping()
        {
            InitializeComponent();
        }

        private void cbTeams_TextUpdate(object sender, EventArgs e)
        {
            string text = this.cbCompetitions.Text;
            List<Competition> dataSource = (
                from ts in this.CachedCompetitions
                where ts.CompetitionFullName.ToUpper().Contains(this.cbCompetitions.Text.ToUpper())
                select ts).ToList<Competition>();
            this.lbCompetitions.ValueMember = "CompetitionId";
            this.lbCompetitions.DisplayMember = "CompetitionFullName";
            this.lbCompetitions.DataSource = dataSource;
            this.cbCompetitions.Text = text;
            this.cbCompetitions.Select(this.cbCompetitions.Text.Length, 0);
        }

        private void frmTeamMapping_Load(object sender, EventArgs e)
        {
            tbCompetitionToMap.Text = CompetitionToMap;
            cbCompetitions.Focus();
            cbCompetitions.Select();
            this.cbCompetitions.DataSource = this.CachedCompetitions;
            this.cbCompetitions.DisplayMember = "CompetitionFullName";
            this.cbCompetitions.ValueMember = "CompetitionId";
            this.cbCompetitions.SelectedIndex = -1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            if (lbCompetitions.SelectedItem != null)
                MappedComeptition = (Competition)lbCompetitions.SelectedItem;
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
            if (lbCompetitions.SelectedItem != null)
                MappedComeptition = (Competition)lbCompetitions.SelectedItem;
            Close();
        }

        private void lbTeams_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                if (lbCompetitions.SelectedItem != null)
                    MappedComeptition = (Competition)lbCompetitions.SelectedItem;
                Close();
            }
        }
    }
}
