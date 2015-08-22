namespace TMS
{
    partial class frmCompetitionMapping
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbCompetitions = new System.Windows.Forms.ComboBox();
            this.lbCompetitions = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbCompetitionToMap = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbCompetitions
            // 
            this.cbCompetitions.FormattingEnabled = true;
            this.cbCompetitions.Location = new System.Drawing.Point(135, 41);
            this.cbCompetitions.Margin = new System.Windows.Forms.Padding(4);
            this.cbCompetitions.Name = "cbCompetitions";
            this.cbCompetitions.Size = new System.Drawing.Size(404, 24);
            this.cbCompetitions.TabIndex = 27;
            this.cbCompetitions.TextUpdate += new System.EventHandler(this.cbTeams_TextUpdate);
            // 
            // lbCompetitions
            // 
            this.lbCompetitions.FormattingEnabled = true;
            this.lbCompetitions.ItemHeight = 16;
            this.lbCompetitions.Location = new System.Drawing.Point(136, 88);
            this.lbCompetitions.Margin = new System.Windows.Forms.Padding(5);
            this.lbCompetitions.Name = "lbCompetitions";
            this.lbCompetitions.ScrollAlwaysVisible = true;
            this.lbCompetitions.Size = new System.Drawing.Size(403, 148);
            this.lbCompetitions.TabIndex = 26;
            this.lbCompetitions.DoubleClick += new System.EventHandler(this.lbTeams_DoubleClick);
            this.lbCompetitions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbTeams_KeyDown);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(258, 254);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 28;
            this.btnOK.Text = "Potvrdi";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(351, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 29;
            this.btnCancel.Text = "Odustani";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbCompetitionToMap
            // 
            this.tbCompetitionToMap.Location = new System.Drawing.Point(136, 14);
            this.tbCompetitionToMap.Name = "tbCompetitionToMap";
            this.tbCompetitionToMap.ReadOnly = true;
            this.tbCompetitionToMap.Size = new System.Drawing.Size(403, 22);
            this.tbCompetitionToMap.TabIndex = 30;
            // 
            // frmCompetitionMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(668, 289);
            this.Controls.Add(this.tbCompetitionToMap);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbCompetitions);
            this.Controls.Add(this.lbCompetitions);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmCompetitionMapping";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mapiranje takmicenja";
            this.Load += new System.EventHandler(this.frmTeamMapping_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbCompetitions;
        private System.Windows.Forms.ListBox lbCompetitions;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbCompetitionToMap;

    }
}