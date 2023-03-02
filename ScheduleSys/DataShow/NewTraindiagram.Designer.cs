namespace ScheduleSys.DataShow
{
    partial class NewTraindiagram
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox_newtraindiagram = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_show = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arriveTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.departureTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brokenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.headwayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capacityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tacticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_postpone = new System.Windows.Forms.ToolStripMenuItem();
            this.checkStockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStockPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_objvalue = new System.Windows.Forms.ToolStripTextBox();
            this.saveTimetableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveVariableToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_newtraindiagram)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox_newtraindiagram);
            this.panel1.Location = new System.Drawing.Point(12, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1100, 450);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox_newtraindiagram
            // 
            this.pictureBox_newtraindiagram.Location = new System.Drawing.Point(18, 27);
            this.pictureBox_newtraindiagram.Name = "pictureBox_newtraindiagram";
            this.pictureBox_newtraindiagram.Size = new System.Drawing.Size(478, 398);
            this.pictureBox_newtraindiagram.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_newtraindiagram.TabIndex = 0;
            this.pictureBox_newtraindiagram.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem_show,
            this.checkCompareToolStripMenuItem,
            this.tacticsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.toolStripTextBox_objvalue});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1220, 27);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveTimetableToolStripMenuItem,
            this.saveVariableToFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.saveToolStripMenuItem.Text = "Save Diagram";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem_show
            // 
            this.toolStripMenuItem_show.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorToolStripMenuItem,
            this.trainNameToolStripMenuItem,
            this.trainIndexToolStripMenuItem,
            this.arriveTimeToolStripMenuItem,
            this.departureTimeToolStripMenuItem,
            this.stockToolStripMenuItem,
            this.brokenToolStripMenuItem});
            this.toolStripMenuItem_show.Name = "toolStripMenuItem_show";
            this.toolStripMenuItem_show.Size = new System.Drawing.Size(78, 23);
            this.toolStripMenuItem_show.Text = "ShowStyle";
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.colorToolStripMenuItem.Text = "Color";
            this.colorToolStripMenuItem.Click += new System.EventHandler(this.colorToolStripMenuItem_Click);
            // 
            // trainNameToolStripMenuItem
            // 
            this.trainNameToolStripMenuItem.Name = "trainNameToolStripMenuItem";
            this.trainNameToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.trainNameToolStripMenuItem.Text = "Train Name";
            this.trainNameToolStripMenuItem.Click += new System.EventHandler(this.trainNameToolStripMenuItem_Click);
            // 
            // trainIndexToolStripMenuItem
            // 
            this.trainIndexToolStripMenuItem.Name = "trainIndexToolStripMenuItem";
            this.trainIndexToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.trainIndexToolStripMenuItem.Text = "Train Index";
            this.trainIndexToolStripMenuItem.Click += new System.EventHandler(this.trainIndexToolStripMenuItem_Click);
            // 
            // arriveTimeToolStripMenuItem
            // 
            this.arriveTimeToolStripMenuItem.Name = "arriveTimeToolStripMenuItem";
            this.arriveTimeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.arriveTimeToolStripMenuItem.Text = "Arrive Time";
            this.arriveTimeToolStripMenuItem.Click += new System.EventHandler(this.arriveTimeToolStripMenuItem_Click);
            // 
            // departureTimeToolStripMenuItem
            // 
            this.departureTimeToolStripMenuItem.Name = "departureTimeToolStripMenuItem";
            this.departureTimeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.departureTimeToolStripMenuItem.Text = "Departure Time";
            this.departureTimeToolStripMenuItem.Click += new System.EventHandler(this.departureTimeToolStripMenuItem_Click);
            // 
            // stockToolStripMenuItem
            // 
            this.stockToolStripMenuItem.Name = "stockToolStripMenuItem";
            this.stockToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.stockToolStripMenuItem.Text = "Stock";
            this.stockToolStripMenuItem.Click += new System.EventHandler(this.stockToolStripMenuItem_Click);
            // 
            // brokenToolStripMenuItem
            // 
            this.brokenToolStripMenuItem.Name = "brokenToolStripMenuItem";
            this.brokenToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.brokenToolStripMenuItem.Text = "Broken";
            this.brokenToolStripMenuItem.Click += new System.EventHandler(this.brokenToolStripMenuItem_Click);
            // 
            // checkCompareToolStripMenuItem
            // 
            this.checkCompareToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.headwayToolStripMenuItem,
            this.capacityToolStripMenuItem});
            this.checkCompareToolStripMenuItem.Name = "checkCompareToolStripMenuItem";
            this.checkCompareToolStripMenuItem.Size = new System.Drawing.Size(55, 23);
            this.checkCompareToolStripMenuItem.Text = "Check";
            // 
            // headwayToolStripMenuItem
            // 
            this.headwayToolStripMenuItem.Name = "headwayToolStripMenuItem";
            this.headwayToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.headwayToolStripMenuItem.Text = "Headway";
            this.headwayToolStripMenuItem.Click += new System.EventHandler(this.headwayToolStripMenuItem_Click);
            // 
            // capacityToolStripMenuItem
            // 
            this.capacityToolStripMenuItem.Name = "capacityToolStripMenuItem";
            this.capacityToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.capacityToolStripMenuItem.Text = "Capacity";
            this.capacityToolStripMenuItem.Click += new System.EventHandler(this.capacityToolStripMenuItem_Click);
            // 
            // tacticsToolStripMenuItem
            // 
            this.tacticsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_postpone,
            this.checkStockToolStripMenuItem});
            this.tacticsToolStripMenuItem.Name = "tacticsToolStripMenuItem";
            this.tacticsToolStripMenuItem.Size = new System.Drawing.Size(59, 23);
            this.tacticsToolStripMenuItem.Text = "Tactics";
            // 
            // toolStripMenuItem_postpone
            // 
            this.toolStripMenuItem_postpone.Name = "toolStripMenuItem_postpone";
            this.toolStripMenuItem_postpone.Size = new System.Drawing.Size(186, 22);
            this.toolStripMenuItem_postpone.Text = "PostPone";
            this.toolStripMenuItem_postpone.Click += new System.EventHandler(this.postpone_click);
            // 
            // checkStockToolStripMenuItem
            // 
            this.checkStockToolStripMenuItem.Name = "checkStockToolStripMenuItem";
            this.checkStockToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.checkStockToolStripMenuItem.Text = "CheckStockConflict";
            this.checkStockToolStripMenuItem.Click += new System.EventHandler(this.checkStockToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showStockPathToolStripMenuItem,
            this.showSolutionToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(52, 23);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // showStockPathToolStripMenuItem
            // 
            this.showStockPathToolStripMenuItem.Name = "showStockPathToolStripMenuItem";
            this.showStockPathToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.showStockPathToolStripMenuItem.Text = "Show_Stock_Path";
            this.showStockPathToolStripMenuItem.Click += new System.EventHandler(this.showStockPathToolStripMenuItem_Click);
            // 
            // showSolutionToolStripMenuItem
            // 
            this.showSolutionToolStripMenuItem.Name = "showSolutionToolStripMenuItem";
            this.showSolutionToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.showSolutionToolStripMenuItem.Text = "Show_Solution";
            this.showSolutionToolStripMenuItem.Click += new System.EventHandler(this.showSolutionToolStripMenuItem_Click);
            // 
            // toolStripTextBox_objvalue
            // 
            this.toolStripTextBox_objvalue.Name = "toolStripTextBox_objvalue";
            this.toolStripTextBox_objvalue.Size = new System.Drawing.Size(100, 23);
            // 
            // saveTimetableToolStripMenuItem
            // 
            this.saveTimetableToolStripMenuItem.Name = "saveTimetableToolStripMenuItem";
            this.saveTimetableToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.saveTimetableToolStripMenuItem.Text = "Save Timetable";
            this.saveTimetableToolStripMenuItem.Click += new System.EventHandler(this.saveTimetableToolStripMenuItem_Click);
            // 
            // saveVariableToFileToolStripMenuItem
            // 
            this.saveVariableToFileToolStripMenuItem.Name = "saveVariableToFileToolStripMenuItem";
            this.saveVariableToFileToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.saveVariableToFileToolStripMenuItem.Text = "Save Variable To File";
            this.saveVariableToFileToolStripMenuItem.Click += new System.EventHandler(this.saveVariableToFileToolStripMenuItem_Click);
            // 
            // NewTraindiagram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 509);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "NewTraindiagram";
            this.Text = "NewTraindiagram";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_newtraindiagram)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.PictureBox pictureBox_newtraindiagram;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_show;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem departureTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arriveTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkCompareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem headwayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem capacityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brokenToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_objvalue;
        private System.Windows.Forms.ToolStripMenuItem tacticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_postpone;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStockPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkStockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTimetableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveVariableToFileToolStripMenuItem;
    }
}