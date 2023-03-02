namespace ScheduleSys.DataShow
{
    partial class IntervalGraph
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kColoringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_stationname = new System.Windows.Forms.ToolStripTextBox();
            this.button_last = new System.Windows.Forms.Button();
            this.button_next = new System.Windows.Forms.Button();
            this.pictureBox_intervalgraph = new System.Windows.Forms.PictureBox();
            this.dataGridView_capacity = new System.Windows.Forms.DataGridView();
            this.trackUsageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_intervalgraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_capacity)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.pictureBox_intervalgraph);
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 600);
            this.panel1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.analysisToolStripMenuItem,
            this.toolStripTextBox_stationname});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1175, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kColoringToolStripMenuItem,
            this.trackUsageToolStripMenuItem});
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            this.analysisToolStripMenuItem.Size = new System.Drawing.Size(66, 23);
            this.analysisToolStripMenuItem.Text = "Analysis";
            // 
            // kColoringToolStripMenuItem
            // 
            this.kColoringToolStripMenuItem.Name = "kColoringToolStripMenuItem";
            this.kColoringToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.kColoringToolStripMenuItem.Text = "K-Coloring";
            this.kColoringToolStripMenuItem.Click += new System.EventHandler(this.kColoringToolStripMenuItem_Click);
            // 
            // toolStripTextBox_stationname
            // 
            this.toolStripTextBox_stationname.Name = "toolStripTextBox_stationname";
            this.toolStripTextBox_stationname.Size = new System.Drawing.Size(100, 23);
            // 
            // button_last
            // 
            this.button_last.Location = new System.Drawing.Point(226, 2);
            this.button_last.Name = "button_last";
            this.button_last.Size = new System.Drawing.Size(75, 23);
            this.button_last.TabIndex = 1;
            this.button_last.Text = "Last";
            this.button_last.UseVisualStyleBackColor = true;
            this.button_last.Click += new System.EventHandler(this.button_last_Click);
            // 
            // button_next
            // 
            this.button_next.Location = new System.Drawing.Point(307, 2);
            this.button_next.Name = "button_next";
            this.button_next.Size = new System.Drawing.Size(75, 23);
            this.button_next.TabIndex = 2;
            this.button_next.Text = "Next";
            this.button_next.UseVisualStyleBackColor = true;
            this.button_next.Click += new System.EventHandler(this.button_next_Click);
            // 
            // pictureBox_intervalgraph
            // 
            this.pictureBox_intervalgraph.Location = new System.Drawing.Point(12, 3);
            this.pictureBox_intervalgraph.Name = "pictureBox_intervalgraph";
            this.pictureBox_intervalgraph.Size = new System.Drawing.Size(783, 418);
            this.pictureBox_intervalgraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_intervalgraph.TabIndex = 0;
            this.pictureBox_intervalgraph.TabStop = false;
            // 
            // dataGridView_capacity
            // 
            this.dataGridView_capacity.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_capacity.Location = new System.Drawing.Point(801, 28);
            this.dataGridView_capacity.Name = "dataGridView_capacity";
            this.dataGridView_capacity.RowTemplate.Height = 23;
            this.dataGridView_capacity.Size = new System.Drawing.Size(374, 600);
            this.dataGridView_capacity.TabIndex = 4;
            // 
            // trackUsageToolStripMenuItem
            // 
            this.trackUsageToolStripMenuItem.Name = "trackUsageToolStripMenuItem";
            this.trackUsageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.trackUsageToolStripMenuItem.Text = "Track Usage";
            this.trackUsageToolStripMenuItem.Click += new System.EventHandler(this.trackUsageToolStripMenuItem_Click);
            // 
            // IntervalGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1175, 490);
            this.Controls.Add(this.dataGridView_capacity);
            this.Controls.Add(this.button_last);
            this.Controls.Add(this.button_next);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "IntervalGraph";
            this.Text = "IntervalGraph";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_intervalgraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_capacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_intervalgraph;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kColoringToolStripMenuItem;
        private System.Windows.Forms.Button button_last;
        private System.Windows.Forms.Button button_next;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_stationname;
        private System.Windows.Forms.DataGridView dataGridView_capacity;
        private System.Windows.Forms.ToolStripMenuItem trackUsageToolStripMenuItem;
    }
}