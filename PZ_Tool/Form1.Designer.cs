namespace PZ_Tool
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPanzerFrontbinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedTankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allTanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectedLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxLevel = new System.Windows.Forms.PictureBox();
            this.groupBoxTank = new System.Windows.Forms.GroupBox();
            this.comboBoxTank = new System.Windows.Forms.ComboBox();
            this.groupBoxLevel = new System.Windows.Forms.GroupBox();
            this.comboBoxLevel = new System.Windows.Forms.ComboBox();
            this.groupBoxLevelPack = new System.Windows.Forms.GroupBox();
            this.comboBoxLevelPack = new System.Windows.Forms.ComboBox();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBarProcess = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLevel)).BeginInit();
            this.groupBoxTank.SuspendLayout();
            this.groupBoxLevel.SuspendLayout();
            this.groupBoxLevelPack.SuspendLayout();
            this.panelProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.extractToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(300, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openPanzerFrontbinToolStripMenuItem});
            this.openToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openPanzerFrontbinToolStripMenuItem
            // 
            this.openPanzerFrontbinToolStripMenuItem.Name = "openPanzerFrontbinToolStripMenuItem";
            this.openPanzerFrontbinToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.openPanzerFrontbinToolStripMenuItem.Text = "Panzer Front .BIN file";
            this.openPanzerFrontbinToolStripMenuItem.Click += new System.EventHandler(this.openPanzerFrontbinToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedTankToolStripMenuItem,
            this.allTanksToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectedLevelToolStripMenuItem});
            this.extractToolStripMenuItem.Enabled = false;
            this.extractToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(59, 21);
            this.extractToolStripMenuItem.Text = "Extract";
            // 
            // selectedTankToolStripMenuItem
            // 
            this.selectedTankToolStripMenuItem.Name = "selectedTankToolStripMenuItem";
            this.selectedTankToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.selectedTankToolStripMenuItem.Text = "Selected transport";
            this.selectedTankToolStripMenuItem.Click += new System.EventHandler(this.selectedTankToolStripMenuItem_Click);
            // 
            // allTanksToolStripMenuItem
            // 
            this.allTanksToolStripMenuItem.Name = "allTanksToolStripMenuItem";
            this.allTanksToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.allTanksToolStripMenuItem.Text = "All transport";
            this.allTanksToolStripMenuItem.Click += new System.EventHandler(this.allTanksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(180, 6);
            // 
            // selectedLevelToolStripMenuItem
            // 
            this.selectedLevelToolStripMenuItem.Name = "selectedLevelToolStripMenuItem";
            this.selectedLevelToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.selectedLevelToolStripMenuItem.Text = "Selected level";
            this.selectedLevelToolStripMenuItem.Click += new System.EventHandler(this.selectedLevelToolStripMenuItem_Click);
            // 
            // pictureBoxLevel
            // 
            this.pictureBoxLevel.InitialImage = null;
            this.pictureBoxLevel.Location = new System.Drawing.Point(6, 55);
            this.pictureBoxLevel.Name = "pictureBoxLevel";
            this.pictureBoxLevel.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxLevel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLevel.TabIndex = 6;
            this.pictureBoxLevel.TabStop = false;
            // 
            // groupBoxTank
            // 
            this.groupBoxTank.Controls.Add(this.comboBoxTank);
            this.groupBoxTank.Location = new System.Drawing.Point(12, 27);
            this.groupBoxTank.Name = "groupBoxTank";
            this.groupBoxTank.Size = new System.Drawing.Size(275, 58);
            this.groupBoxTank.TabIndex = 7;
            this.groupBoxTank.TabStop = false;
            this.groupBoxTank.Text = "Transport";
            // 
            // comboBoxTank
            // 
            this.comboBoxTank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTank.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxTank.FormattingEnabled = true;
            this.comboBoxTank.Location = new System.Drawing.Point(6, 19);
            this.comboBoxTank.Name = "comboBoxTank";
            this.comboBoxTank.Size = new System.Drawing.Size(256, 24);
            this.comboBoxTank.TabIndex = 9;
            // 
            // groupBoxLevel
            // 
            this.groupBoxLevel.Controls.Add(this.comboBoxLevel);
            this.groupBoxLevel.Controls.Add(this.pictureBoxLevel);
            this.groupBoxLevel.Location = new System.Drawing.Point(12, 161);
            this.groupBoxLevel.Name = "groupBoxLevel";
            this.groupBoxLevel.Size = new System.Drawing.Size(275, 326);
            this.groupBoxLevel.TabIndex = 8;
            this.groupBoxLevel.TabStop = false;
            this.groupBoxLevel.Text = "Levels";
            // 
            // comboBoxLevel
            // 
            this.comboBoxLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxLevel.FormattingEnabled = true;
            this.comboBoxLevel.Location = new System.Drawing.Point(6, 19);
            this.comboBoxLevel.Name = "comboBoxLevel";
            this.comboBoxLevel.Size = new System.Drawing.Size(256, 24);
            this.comboBoxLevel.TabIndex = 0;
            this.comboBoxLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxLevel_SelectedIndexChanged);
            // 
            // groupBoxLevelPack
            // 
            this.groupBoxLevelPack.Controls.Add(this.comboBoxLevelPack);
            this.groupBoxLevelPack.Location = new System.Drawing.Point(11, 91);
            this.groupBoxLevelPack.Name = "groupBoxLevelPack";
            this.groupBoxLevelPack.Size = new System.Drawing.Size(275, 58);
            this.groupBoxLevelPack.TabIndex = 9;
            this.groupBoxLevelPack.TabStop = false;
            this.groupBoxLevelPack.Text = "Level pack";
            // 
            // comboBoxLevelPack
            // 
            this.comboBoxLevelPack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLevelPack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxLevelPack.FormattingEnabled = true;
            this.comboBoxLevelPack.Location = new System.Drawing.Point(6, 19);
            this.comboBoxLevelPack.Name = "comboBoxLevelPack";
            this.comboBoxLevelPack.Size = new System.Drawing.Size(256, 24);
            this.comboBoxLevelPack.TabIndex = 9;
            // 
            // panelProgress
            // 
            this.panelProgress.BackColor = System.Drawing.SystemColors.Control;
            this.panelProgress.Controls.Add(this.labelProgress);
            this.panelProgress.Controls.Add(this.progressBarProcess);
            this.panelProgress.Location = new System.Drawing.Point(0, 0);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(300, 498);
            this.panelProgress.TabIndex = 10;
            this.panelProgress.Visible = false;
            // 
            // labelProgress
            // 
            this.labelProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelProgress.Location = new System.Drawing.Point(18, 198);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(263, 39);
            this.labelProgress.TabIndex = 13;
            this.labelProgress.Text = "Processing...";
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBarProcess
            // 
            this.progressBarProcess.BackColor = System.Drawing.SystemColors.ControlDark;
            this.progressBarProcess.Location = new System.Drawing.Point(18, 240);
            this.progressBarProcess.Name = "progressBarProcess";
            this.progressBarProcess.Size = new System.Drawing.Size(263, 23);
            this.progressBarProcess.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 498);
            this.Controls.Add(this.panelProgress);
            this.Controls.Add(this.groupBoxLevelPack);
            this.Controls.Add(this.groupBoxLevel);
            this.Controls.Add(this.groupBoxTank);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Panzer Front PS1 Tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLevel)).EndInit();
            this.groupBoxTank.ResumeLayout(false);
            this.groupBoxLevel.ResumeLayout(false);
            this.groupBoxLevelPack.ResumeLayout(false);
            this.panelProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPanzerFrontbinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedTankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allTanksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectedLevelToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxLevel;
        private System.Windows.Forms.GroupBox groupBoxTank;
        private System.Windows.Forms.GroupBox groupBoxLevel;
        private System.Windows.Forms.ComboBox comboBoxTank;
        private System.Windows.Forms.ComboBox comboBoxLevel;
        private System.Windows.Forms.GroupBox groupBoxLevelPack;
        private System.Windows.Forms.ComboBox comboBoxLevelPack;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.ProgressBar progressBarProcess;
        private System.Windows.Forms.Label labelProgress;
    }
}

