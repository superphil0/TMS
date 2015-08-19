using System.ComponentModel;
using System.Windows.Forms;
namespace TMS
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnGenerateExcel;
        private ListBox lbCompetition;
        private ListBox lbTeams;
        private Label lblLige;
        private Button btnDeleteFile;
        private PictureBox pbLoadingTeams;
        private PictureBox pbLoadingPlayers;
        private ListBox lbCountries;
        private ComboBox cbCountries;
        private PictureBox pbLoadingCompetitions;
        private PictureBox pbLoadingCountries;
        private ListBox lbLatest;
        private Label lblLatest;
        private Button btnRefresh;
        private DateTimePicker dtpDatum;
        private Timer tmrReload;
        private ListBox lbUnamapped;
        private Label lblUnmapped;
        private ContextMenuStrip cmLineupstatus;
        private ToolStripMenuItem tsmiYES;
        private ToolStripMenuItem tsmiNO;
        private PictureBox pbLivescore;
        private TextBox tbStatus;
        private RadioButton rbLive;
        private RadioButton rbSlijedi;
        private RadioButton rbCompleted;
        private ContextMenuStrip cmsMapTeam;
        private ToolStripMenuItem mapirajTimToolStripMenuItem;
        private DataGridView lbMatches;
        private DataGridView dgvPlayers;
        private Button btnStop;
        private ComboBox cbTeams;
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.btnGenerateExcel = new System.Windows.Forms.Button();
      this.lbCompetition = new System.Windows.Forms.ListBox();
      this.lbTeams = new System.Windows.Forms.ListBox();
      this.cmsMapTeam = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mapirajTimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.lblLige = new System.Windows.Forms.Label();
      this.btnDeleteFile = new System.Windows.Forms.Button();
      this.lbCountries = new System.Windows.Forms.ListBox();
      this.cbCountries = new System.Windows.Forms.ComboBox();
      this.cbTeams = new System.Windows.Forms.ComboBox();
      this.lbLatest = new System.Windows.Forms.ListBox();
      this.lblLatest = new System.Windows.Forms.Label();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.dtpDatum = new System.Windows.Forms.DateTimePicker();
      this.tmrReload = new System.Windows.Forms.Timer(this.components);
      this.cmLineupstatus = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.tsmiYES = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiNO = new System.Windows.Forms.ToolStripMenuItem();
      this.lbUnamapped = new System.Windows.Forms.ListBox();
      this.lblUnmapped = new System.Windows.Forms.Label();
      this.tbStatus = new System.Windows.Forms.TextBox();
      this.pbLivescore = new System.Windows.Forms.PictureBox();
      this.pbLoadingPlayers = new System.Windows.Forms.PictureBox();
      this.pbLoadingCountries = new System.Windows.Forms.PictureBox();
      this.pbLoadingCompetitions = new System.Windows.Forms.PictureBox();
      this.pbLoadingTeams = new System.Windows.Forms.PictureBox();
      this.rbLive = new System.Windows.Forms.RadioButton();
      this.rbSlijedi = new System.Windows.Forms.RadioButton();
      this.rbCompleted = new System.Windows.Forms.RadioButton();
      this.lbMatches = new System.Windows.Forms.DataGridView();
      this.dgvPlayers = new System.Windows.Forms.DataGridView();
      this.btnStop = new System.Windows.Forms.Button();
      this.cbCurrentSeason = new System.Windows.Forms.ComboBox();
      this.lblCurrentSeason = new System.Windows.Forms.Label();
      this.btnArhiva = new System.Windows.Forms.Button();
      this.lbArhiva = new System.Windows.Forms.ListBox();
      this.btnAzurirajArhivu = new System.Windows.Forms.Button();
      this.cmsMapTeam.SuspendLayout();
      this.cmLineupstatus.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbLivescore)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingPlayers)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingCountries)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingCompetitions)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingTeams)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lbMatches)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).BeginInit();
      this.SuspendLayout();
      // 
      // btnGenerateExcel
      // 
      this.btnGenerateExcel.Enabled = false;
      this.btnGenerateExcel.Location = new System.Drawing.Point(587, 509);
      this.btnGenerateExcel.Margin = new System.Windows.Forms.Padding(4);
      this.btnGenerateExcel.Name = "btnGenerateExcel";
      this.btnGenerateExcel.Size = new System.Drawing.Size(274, 36);
      this.btnGenerateExcel.TabIndex = 6;
      this.btnGenerateExcel.Text = "NOVI EXCEL DOKUMENT";
      this.btnGenerateExcel.UseVisualStyleBackColor = true;
      this.btnGenerateExcel.Click += new System.EventHandler(this.btnGenerateExcel_Click);
      // 
      // lbCompetition
      // 
      this.lbCompetition.FormattingEnabled = true;
      this.lbCompetition.ItemHeight = 16;
      this.lbCompetition.Location = new System.Drawing.Point(10, 226);
      this.lbCompetition.Margin = new System.Windows.Forms.Padding(4);
      this.lbCompetition.Name = "lbCompetition";
      this.lbCompetition.ScrollAlwaysVisible = true;
      this.lbCompetition.Size = new System.Drawing.Size(194, 228);
      this.lbCompetition.TabIndex = 7;
      this.lbCompetition.Click += new System.EventHandler(this.lbCompetition_Click);
      this.lbCompetition.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbCompetition_KeyDown);
      // 
      // lbTeams
      // 
      this.lbTeams.ContextMenuStrip = this.cmsMapTeam;
      this.lbTeams.FormattingEnabled = true;
      this.lbTeams.ItemHeight = 16;
      this.lbTeams.Location = new System.Drawing.Point(587, 50);
      this.lbTeams.Margin = new System.Windows.Forms.Padding(4);
      this.lbTeams.Name = "lbTeams";
      this.lbTeams.ScrollAlwaysVisible = true;
      this.lbTeams.Size = new System.Drawing.Size(275, 436);
      this.lbTeams.TabIndex = 8;
      this.lbTeams.Click += new System.EventHandler(this.lbTeams_Click);
      this.lbTeams.SelectedIndexChanged += new System.EventHandler(this.lbTeams_SelectedIndexChanged);
      this.lbTeams.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbTeams_KeyDown);
      // 
      // cmsMapTeam
      // 
      this.cmsMapTeam.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapirajTimToolStripMenuItem});
      this.cmsMapTeam.Name = "cmsMapTeam";
      this.cmsMapTeam.Size = new System.Drawing.Size(136, 26);
      // 
      // mapirajTimToolStripMenuItem
      // 
      this.mapirajTimToolStripMenuItem.Name = "mapirajTimToolStripMenuItem";
      this.mapirajTimToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
      this.mapirajTimToolStripMenuItem.Text = "Mapiraj tim";
      this.mapirajTimToolStripMenuItem.Click += new System.EventHandler(this.mapirajTimToolStripMenuItem_Click);
      // 
      // lblLige
      // 
      this.lblLige.AutoSize = true;
      this.lblLige.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.lblLige.Location = new System.Drawing.Point(8, 206);
      this.lblLige.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblLige.Name = "lblLige";
      this.lblLige.Size = new System.Drawing.Size(93, 16);
      this.lblLige.TabIndex = 11;
      this.lblLige.Text = "Takmičenje:";
      // 
      // btnDeleteFile
      // 
      this.btnDeleteFile.Location = new System.Drawing.Point(460, 494);
      this.btnDeleteFile.Margin = new System.Windows.Forms.Padding(4);
      this.btnDeleteFile.Name = "btnDeleteFile";
      this.btnDeleteFile.Size = new System.Drawing.Size(120, 24);
      this.btnDeleteFile.TabIndex = 17;
      this.btnDeleteFile.Text = "Obriši";
      this.btnDeleteFile.UseVisualStyleBackColor = true;
      this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
      // 
      // lbCountries
      // 
      this.lbCountries.FormattingEnabled = true;
      this.lbCountries.ItemHeight = 16;
      this.lbCountries.Location = new System.Drawing.Point(11, 43);
      this.lbCountries.Margin = new System.Windows.Forms.Padding(4);
      this.lbCountries.Name = "lbCountries";
      this.lbCountries.ScrollAlwaysVisible = true;
      this.lbCountries.Size = new System.Drawing.Size(193, 148);
      this.lbCountries.TabIndex = 20;
      this.lbCountries.Click += new System.EventHandler(this.lbCountries_Click);
      // 
      // cbCountries
      // 
      this.cbCountries.FormattingEnabled = true;
      this.cbCountries.Location = new System.Drawing.Point(10, 10);
      this.cbCountries.Name = "cbCountries";
      this.cbCountries.Size = new System.Drawing.Size(194, 24);
      this.cbCountries.TabIndex = 22;
      this.cbCountries.TextUpdate += new System.EventHandler(this.cbCountries_TextUpdate);
      this.cbCountries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbCountries_KeyDown);
      // 
      // cbTeams
      // 
      this.cbTeams.FormattingEnabled = true;
      this.cbTeams.Location = new System.Drawing.Point(587, 12);
      this.cbTeams.Name = "cbTeams";
      this.cbTeams.Size = new System.Drawing.Size(276, 24);
      this.cbTeams.TabIndex = 25;
      this.cbTeams.TextUpdate += new System.EventHandler(this.cbTeams_TextUpdate);
      this.cbTeams.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbTeams_KeyDown);
      // 
      // lbLatest
      // 
      this.lbLatest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.lbLatest.FormattingEnabled = true;
      this.lbLatest.ItemHeight = 16;
      this.lbLatest.Location = new System.Drawing.Point(10, 610);
      this.lbLatest.Margin = new System.Windows.Forms.Padding(4);
      this.lbLatest.Name = "lbLatest";
      this.lbLatest.ScrollAlwaysVisible = true;
      this.lbLatest.Size = new System.Drawing.Size(570, 84);
      this.lbLatest.TabIndex = 26;
      this.lbLatest.DoubleClick += new System.EventHandler(this.lbLatest_DoubleClick);
      // 
      // lblLatest
      // 
      this.lblLatest.AutoSize = true;
      this.lblLatest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.lblLatest.Location = new System.Drawing.Point(12, 494);
      this.lblLatest.Name = "lblLatest";
      this.lblLatest.Size = new System.Drawing.Size(109, 16);
      this.lblLatest.TabIndex = 27;
      this.lblLatest.Text = "Svi dokumenti:";
      // 
      // btnRefresh
      // 
      this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRefresh.Location = new System.Drawing.Point(410, 10);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(170, 23);
      this.btnRefresh.TabIndex = 31;
      this.btnRefresh.Text = "Osvježi";
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
      // 
      // dtpDatum
      // 
      this.dtpDatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dtpDatum.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dtpDatum.Location = new System.Drawing.Point(210, 10);
      this.dtpDatum.Name = "dtpDatum";
      this.dtpDatum.Size = new System.Drawing.Size(185, 22);
      this.dtpDatum.TabIndex = 30;
      // 
      // tmrReload
      // 
      this.tmrReload.Interval = 300000;
      this.tmrReload.Tick += new System.EventHandler(this.tmrReload_Tick_1);
      // 
      // cmLineupstatus
      // 
      this.cmLineupstatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiYES,
            this.tsmiNO});
      this.cmLineupstatus.Name = "cmLineupstatus";
      this.cmLineupstatus.Size = new System.Drawing.Size(113, 48);
      // 
      // tsmiYES
      // 
      this.tsmiYES.Name = "tsmiYES";
      this.tsmiYES.Size = new System.Drawing.Size(112, 22);
      this.tsmiYES.Text = "Igra";
      this.tsmiYES.Click += new System.EventHandler(this.tsmiYES_Click);
      // 
      // tsmiNO
      // 
      this.tsmiNO.Name = "tsmiNO";
      this.tsmiNO.Size = new System.Drawing.Size(112, 22);
      this.tsmiNO.Text = "Ne igra";
      this.tsmiNO.Click += new System.EventHandler(this.tsmiNO_Click);
      // 
      // lbUnamapped
      // 
      this.lbUnamapped.FormattingEnabled = true;
      this.lbUnamapped.ItemHeight = 16;
      this.lbUnamapped.Location = new System.Drawing.Point(587, 642);
      this.lbUnamapped.Name = "lbUnamapped";
      this.lbUnamapped.Size = new System.Drawing.Size(274, 52);
      this.lbUnamapped.TabIndex = 33;
      this.lbUnamapped.Visible = false;
      // 
      // lblUnmapped
      // 
      this.lblUnmapped.AutoSize = true;
      this.lblUnmapped.Location = new System.Drawing.Point(584, 623);
      this.lblUnmapped.Name = "lblUnmapped";
      this.lblUnmapped.Size = new System.Drawing.Size(117, 16);
      this.lblUnmapped.TabIndex = 34;
      this.lblUnmapped.Text = "Nemapirani igrači:";
      this.lblUnmapped.Visible = false;
      // 
      // tbStatus
      // 
      this.tbStatus.Location = new System.Drawing.Point(588, 549);
      this.tbStatus.Margin = new System.Windows.Forms.Padding(4);
      this.tbStatus.Multiline = true;
      this.tbStatus.Name = "tbStatus";
      this.tbStatus.ReadOnly = true;
      this.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbStatus.Size = new System.Drawing.Size(272, 36);
      this.tbStatus.TabIndex = 5;
      this.tbStatus.Visible = false;
      // 
      // pbLivescore
      // 
      this.pbLivescore.BackColor = System.Drawing.Color.White;
      this.pbLivescore.Image = global::TMS.Properties.Resources.loading4;
      this.pbLivescore.Location = new System.Drawing.Point(344, 222);
      this.pbLivescore.Margin = new System.Windows.Forms.Padding(4);
      this.pbLivescore.Name = "pbLivescore";
      this.pbLivescore.Size = new System.Drawing.Size(90, 110);
      this.pbLivescore.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pbLivescore.TabIndex = 36;
      this.pbLivescore.TabStop = false;
      this.pbLivescore.Visible = false;
      // 
      // pbLoadingPlayers
      // 
      this.pbLoadingPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.pbLoadingPlayers.BackColor = System.Drawing.Color.White;
      this.pbLoadingPlayers.Image = global::TMS.Properties.Resources.loading4;
      this.pbLoadingPlayers.Location = new System.Drawing.Point(951, 322);
      this.pbLoadingPlayers.Margin = new System.Windows.Forms.Padding(4);
      this.pbLoadingPlayers.Name = "pbLoadingPlayers";
      this.pbLoadingPlayers.Size = new System.Drawing.Size(90, 110);
      this.pbLoadingPlayers.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pbLoadingPlayers.TabIndex = 19;
      this.pbLoadingPlayers.TabStop = false;
      this.pbLoadingPlayers.Visible = false;
      // 
      // pbLoadingCountries
      // 
      this.pbLoadingCountries.BackColor = System.Drawing.Color.White;
      this.pbLoadingCountries.Image = global::TMS.Properties.Resources.loading4;
      this.pbLoadingCountries.Location = new System.Drawing.Point(60, 65);
      this.pbLoadingCountries.Margin = new System.Windows.Forms.Padding(4);
      this.pbLoadingCountries.Name = "pbLoadingCountries";
      this.pbLoadingCountries.Size = new System.Drawing.Size(90, 110);
      this.pbLoadingCountries.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pbLoadingCountries.TabIndex = 24;
      this.pbLoadingCountries.TabStop = false;
      this.pbLoadingCountries.Visible = false;
      // 
      // pbLoadingCompetitions
      // 
      this.pbLoadingCompetitions.BackColor = System.Drawing.Color.White;
      this.pbLoadingCompetitions.Image = global::TMS.Properties.Resources.loading4;
      this.pbLoadingCompetitions.Location = new System.Drawing.Point(60, 297);
      this.pbLoadingCompetitions.Margin = new System.Windows.Forms.Padding(4);
      this.pbLoadingCompetitions.Name = "pbLoadingCompetitions";
      this.pbLoadingCompetitions.Size = new System.Drawing.Size(90, 110);
      this.pbLoadingCompetitions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pbLoadingCompetitions.TabIndex = 23;
      this.pbLoadingCompetitions.TabStop = false;
      this.pbLoadingCompetitions.Visible = false;
      // 
      // pbLoadingTeams
      // 
      this.pbLoadingTeams.BackColor = System.Drawing.Color.White;
      this.pbLoadingTeams.Image = global::TMS.Properties.Resources.loading4;
      this.pbLoadingTeams.Location = new System.Drawing.Point(680, 237);
      this.pbLoadingTeams.Margin = new System.Windows.Forms.Padding(4);
      this.pbLoadingTeams.Name = "pbLoadingTeams";
      this.pbLoadingTeams.Size = new System.Drawing.Size(90, 118);
      this.pbLoadingTeams.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pbLoadingTeams.TabIndex = 18;
      this.pbLoadingTeams.TabStop = false;
      this.pbLoadingTeams.Visible = false;
      // 
      // rbLive
      // 
      this.rbLive.AutoSize = true;
      this.rbLive.Location = new System.Drawing.Point(354, 36);
      this.rbLive.Name = "rbLive";
      this.rbLive.Size = new System.Drawing.Size(60, 20);
      this.rbLive.TabIndex = 37;
      this.rbLive.Text = "Uživo";
      this.rbLive.UseVisualStyleBackColor = true;
      this.rbLive.Click += new System.EventHandler(this.rbLive_Click);
      // 
      // rbSlijedi
      // 
      this.rbSlijedi.AutoSize = true;
      this.rbSlijedi.Checked = true;
      this.rbSlijedi.Location = new System.Drawing.Point(211, 36);
      this.rbSlijedi.Name = "rbSlijedi";
      this.rbSlijedi.Size = new System.Drawing.Size(63, 20);
      this.rbSlijedi.TabIndex = 38;
      this.rbSlijedi.TabStop = true;
      this.rbSlijedi.Text = "Slijedi";
      this.rbSlijedi.UseVisualStyleBackColor = true;
      this.rbSlijedi.Click += new System.EventHandler(this.rbSlijedi_Click);
      // 
      // rbCompleted
      // 
      this.rbCompleted.AutoSize = true;
      this.rbCompleted.Location = new System.Drawing.Point(476, 36);
      this.rbCompleted.Name = "rbCompleted";
      this.rbCompleted.Size = new System.Drawing.Size(83, 20);
      this.rbCompleted.TabIndex = 39;
      this.rbCompleted.Text = "Završeno";
      this.rbCompleted.UseVisualStyleBackColor = true;
      this.rbCompleted.Click += new System.EventHandler(this.rbCompleted_Click);
      // 
      // lbMatches
      // 
      this.lbMatches.AllowUserToResizeRows = false;
      this.lbMatches.BackgroundColor = System.Drawing.Color.White;
      this.lbMatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.lbMatches.ColumnHeadersVisible = false;
      this.lbMatches.Location = new System.Drawing.Point(210, 62);
      this.lbMatches.Name = "lbMatches";
      this.lbMatches.ReadOnly = true;
      this.lbMatches.RowHeadersVisible = false;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbMatches.RowsDefaultCellStyle = dataGridViewCellStyle1;
      this.lbMatches.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.lbMatches.Size = new System.Drawing.Size(370, 424);
      this.lbMatches.TabIndex = 40;
      this.lbMatches.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lbMatches_CellClick);
      // 
      // dgvPlayers
      // 
      this.dgvPlayers.AllowUserToResizeRows = false;
      this.dgvPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dgvPlayers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvPlayers.BackgroundColor = System.Drawing.Color.White;
      this.dgvPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvPlayers.ColumnHeadersVisible = false;
      this.dgvPlayers.ContextMenuStrip = this.cmLineupstatus;
      this.dgvPlayers.Location = new System.Drawing.Point(866, 10);
      this.dgvPlayers.Name = "dgvPlayers";
      this.dgvPlayers.ReadOnly = true;
      this.dgvPlayers.RowHeadersWidth = 30;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dgvPlayers.RowsDefaultCellStyle = dataGridViewCellStyle2;
      this.dgvPlayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgvPlayers.Size = new System.Drawing.Size(255, 684);
      this.dgvPlayers.TabIndex = 32;
      this.dgvPlayers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPlayers_CellClick);
      // 
      // btnStop
      // 
      this.btnStop.Location = new System.Drawing.Point(587, 590);
      this.btnStop.Name = "btnStop";
      this.btnStop.Size = new System.Drawing.Size(274, 30);
      this.btnStop.TabIndex = 41;
      this.btnStop.Text = "ZAUSTAVI";
      this.btnStop.UseVisualStyleBackColor = true;
      this.btnStop.Visible = false;
      this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
      // 
      // cbCurrentSeason
      // 
      this.cbCurrentSeason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbCurrentSeason.FormattingEnabled = true;
      this.cbCurrentSeason.Location = new System.Drawing.Point(741, 486);
      this.cbCurrentSeason.Name = "cbCurrentSeason";
      this.cbCurrentSeason.Size = new System.Drawing.Size(121, 24);
      this.cbCurrentSeason.TabIndex = 42;
      // 
      // lblCurrentSeason
      // 
      this.lblCurrentSeason.AutoSize = true;
      this.lblCurrentSeason.Location = new System.Drawing.Point(588, 489);
      this.lblCurrentSeason.Name = "lblCurrentSeason";
      this.lblCurrentSeason.Size = new System.Drawing.Size(57, 16);
      this.lblCurrentSeason.TabIndex = 43;
      this.lblCurrentSeason.Text = "Sezona:";
      // 
      // btnArhiva
      // 
      this.btnArhiva.Enabled = false;
      this.btnArhiva.Location = new System.Drawing.Point(10, 462);
      this.btnArhiva.Name = "btnArhiva";
      this.btnArhiva.Size = new System.Drawing.Size(194, 23);
      this.btnArhiva.TabIndex = 44;
      this.btnArhiva.Text = "Napravi novu arhivu";
      this.btnArhiva.UseVisualStyleBackColor = true;
      this.btnArhiva.Click += new System.EventHandler(this.btnArhiva_Click);
      // 
      // lbArhiva
      // 
      this.lbArhiva.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.lbArhiva.FormattingEnabled = true;
      this.lbArhiva.ItemHeight = 16;
      this.lbArhiva.Location = new System.Drawing.Point(10, 520);
      this.lbArhiva.Margin = new System.Windows.Forms.Padding(4);
      this.lbArhiva.Name = "lbArhiva";
      this.lbArhiva.ScrollAlwaysVisible = true;
      this.lbArhiva.Size = new System.Drawing.Size(570, 84);
      this.lbArhiva.TabIndex = 45;
      this.lbArhiva.DoubleClick += new System.EventHandler(this.lbArhiva_DoubleClick);
      // 
      // btnAzurirajArhivu
      // 
      this.btnAzurirajArhivu.Enabled = false;
      this.btnAzurirajArhivu.Location = new System.Drawing.Point(210, 492);
      this.btnAzurirajArhivu.Name = "btnAzurirajArhivu";
      this.btnAzurirajArhivu.Size = new System.Drawing.Size(127, 23);
      this.btnAzurirajArhivu.TabIndex = 46;
      this.btnAzurirajArhivu.Text = "Azuriraj arhivu";
      this.btnAzurirajArhivu.UseVisualStyleBackColor = true;
      this.btnAzurirajArhivu.Click += new System.EventHandler(this.btnAzurirajArhivu_Click);
      // 
      // frmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.LightSteelBlue;
      this.ClientSize = new System.Drawing.Size(1123, 702);
      this.Controls.Add(this.btnAzurirajArhivu);
      this.Controls.Add(this.lbArhiva);
      this.Controls.Add(this.btnArhiva);
      this.Controls.Add(this.lblCurrentSeason);
      this.Controls.Add(this.cbCurrentSeason);
      this.Controls.Add(this.btnStop);
      this.Controls.Add(this.pbLivescore);
      this.Controls.Add(this.pbLoadingTeams);
      this.Controls.Add(this.rbCompleted);
      this.Controls.Add(this.rbSlijedi);
      this.Controls.Add(this.rbLive);
      this.Controls.Add(this.lblUnmapped);
      this.Controls.Add(this.lbUnamapped);
      this.Controls.Add(this.pbLoadingPlayers);
      this.Controls.Add(this.btnRefresh);
      this.Controls.Add(this.dtpDatum);
      this.Controls.Add(this.lblLatest);
      this.Controls.Add(this.lbLatest);
      this.Controls.Add(this.cbTeams);
      this.Controls.Add(this.pbLoadingCountries);
      this.Controls.Add(this.pbLoadingCompetitions);
      this.Controls.Add(this.cbCountries);
      this.Controls.Add(this.lbCountries);
      this.Controls.Add(this.btnDeleteFile);
      this.Controls.Add(this.lblLige);
      this.Controls.Add(this.lbCompetition);
      this.Controls.Add(this.btnGenerateExcel);
      this.Controls.Add(this.tbStatus);
      this.Controls.Add(this.dgvPlayers);
      this.Controls.Add(this.lbTeams);
      this.Controls.Add(this.lbMatches);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.KeyPreview = true;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "frmMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "TMS";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.cmsMapTeam.ResumeLayout(false);
      this.cmLineupstatus.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pbLivescore)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingPlayers)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingCountries)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingCompetitions)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbLoadingTeams)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lbMatches)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        private ComboBox cbCurrentSeason;
        private Label lblCurrentSeason;
        private Button btnArhiva;
    private ListBox lbArhiva;
    private Button btnAzurirajArhivu;
  }
}

