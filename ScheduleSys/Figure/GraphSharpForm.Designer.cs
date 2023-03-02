namespace ScheduleSys.Figure
{
    partial class GraphSharpForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outGraphTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outPutNodeCoodinateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readCoodinateFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1362, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outGraphTreeToolStripMenuItem,
            this.outPutNodeCoodinateToolStripMenuItem,
            this.readCoodinateFromFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // outGraphTreeToolStripMenuItem
            // 
            this.outGraphTreeToolStripMenuItem.Name = "outGraphTreeToolStripMenuItem";
            this.outGraphTreeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.outGraphTreeToolStripMenuItem.Text = "Out_Graph_Tree";
            // 
            // outPutNodeCoodinateToolStripMenuItem
            // 
            this.outPutNodeCoodinateToolStripMenuItem.Name = "outPutNodeCoodinateToolStripMenuItem";
            this.outPutNodeCoodinateToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.outPutNodeCoodinateToolStripMenuItem.Text = "OutPut_Node_Coodinate";
            // 
            // readCoodinateFromFileToolStripMenuItem
            // 
            this.readCoodinateFromFileToolStripMenuItem.Name = "readCoodinateFromFileToolStripMenuItem";
            this.readCoodinateFromFileToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.readCoodinateFromFileToolStripMenuItem.Text = "Read_Coodinate_FromFile";
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(0, 28);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(1382, 711);
            this.elementHost1.TabIndex = 1;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // GraphSharpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 741);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GraphSharpForm";
            this.Text = "GraphSharpForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outGraphTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outPutNodeCoodinateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readCoodinateFromFileToolStripMenuItem;
        public System.Windows.Forms.Integration.ElementHost elementHost1;
    }
}