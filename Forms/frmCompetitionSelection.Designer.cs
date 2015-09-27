namespace TMS
{
    partial class frmCompetitionSelection
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
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.cbCompetitions = new System.Windows.Forms.CheckedListBox();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(116, 469);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 28;
      this.btnOK.Text = "Potvrdi";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(209, 469);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 29;
      this.btnCancel.Text = "Odustani";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // cbCompetitions
      // 
      this.cbCompetitions.CheckOnClick = true;
      this.cbCompetitions.FormattingEnabled = true;
      this.cbCompetitions.Location = new System.Drawing.Point(25, 22);
      this.cbCompetitions.Name = "cbCompetitions";
      this.cbCompetitions.ScrollAlwaysVisible = true;
      this.cbCompetitions.Size = new System.Drawing.Size(361, 429);
      this.cbCompetitions.TabIndex = 30;
      // 
      // frmCompetitionSelection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.LightSteelBlue;
      this.ClientSize = new System.Drawing.Size(414, 504);
      this.Controls.Add(this.cbCompetitions);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "frmCompetitionSelection";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Izbor takmičenja";
      this.Load += new System.EventHandler(this.frmTeamMapping_Load);
      this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.CheckedListBox cbCompetitions;
  }
}