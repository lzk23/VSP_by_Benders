namespace ScheduleSys.DataShow
{
    partial class ShowResultByGraph
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.button_showbar = new System.Windows.Forms.Button();
            this.line_graph_btn = new System.Windows.Forms.Button();
            this.Import_data_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_solvingtime
            // 
            this.textBox.Location = new System.Drawing.Point(1, 2);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox_solvingtime";
            this.textBox.Size = new System.Drawing.Size(448, 392);
            this.textBox.TabIndex = 0;
            // 
            // button_showbar
            // 
            this.button_showbar.Location = new System.Drawing.Point(472, 182);
            this.button_showbar.Name = "button_showbar";
            this.button_showbar.Size = new System.Drawing.Size(123, 23);
            this.button_showbar.TabIndex = 1;
            this.button_showbar.Text = "Show bar graph";
            this.button_showbar.UseVisualStyleBackColor = true;
            this.button_showbar.Click += new System.EventHandler(this.button_showbar_Click);
            // 
            // line_graph_btn
            // 
            this.line_graph_btn.Location = new System.Drawing.Point(472, 231);
            this.line_graph_btn.Name = "line_graph_btn";
            this.line_graph_btn.Size = new System.Drawing.Size(123, 23);
            this.line_graph_btn.TabIndex = 2;
            this.line_graph_btn.Text = "Show line graph";
            this.line_graph_btn.UseVisualStyleBackColor = true;
            this.line_graph_btn.Click += new System.EventHandler(this.line_graph_btn_Click);
            // 
            // Import_data_btn
            // 
            this.Import_data_btn.Location = new System.Drawing.Point(472, 23);
            this.Import_data_btn.Name = "Import_data_btn";
            this.Import_data_btn.Size = new System.Drawing.Size(123, 23);
            this.Import_data_btn.TabIndex = 3;
            this.Import_data_btn.Text = "Import_data";
            this.Import_data_btn.UseVisualStyleBackColor = true;
            this.Import_data_btn.Click += new System.EventHandler(this.Import_data_btn_Click);
            // 
            // ShowResultByGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 406);
            this.Controls.Add(this.Import_data_btn);
            this.Controls.Add(this.line_graph_btn);
            this.Controls.Add(this.button_showbar);
            this.Controls.Add(this.textBox);
            this.Name = "ShowResultByGraph";
            this.Text = "ShowResultByGraph";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button button_showbar;
        private System.Windows.Forms.Button line_graph_btn;
        private System.Windows.Forms.Button Import_data_btn;
    }
}