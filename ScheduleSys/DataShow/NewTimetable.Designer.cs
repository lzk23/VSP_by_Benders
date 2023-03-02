namespace ScheduleSys.DataShow
{
    partial class NewTimetable
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
            this.dataGridView_timetable = new System.Windows.Forms.DataGridView();
            this.textBox_solvingprocess = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_timetable)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_timetable
            // 
            this.dataGridView_timetable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_timetable.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_timetable.Name = "dataGridView_timetable";
            this.dataGridView_timetable.RowTemplate.Height = 23;
            this.dataGridView_timetable.Size = new System.Drawing.Size(701, 519);
            this.dataGridView_timetable.TabIndex = 0;
            // 
            // textBox_solvingprocess
            // 
            this.textBox_solvingprocess.Location = new System.Drawing.Point(707, 0);
            this.textBox_solvingprocess.Multiline = true;
            this.textBox_solvingprocess.Name = "textBox_solvingprocess";
            this.textBox_solvingprocess.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_solvingprocess.Size = new System.Drawing.Size(455, 519);
            this.textBox_solvingprocess.TabIndex = 1;
            // 
            // NewTimetable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 520);
            this.Controls.Add(this.textBox_solvingprocess);
            this.Controls.Add(this.dataGridView_timetable);
            this.Name = "NewTimetable";
            this.Text = "NewTimetable";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_timetable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView_timetable;
        public System.Windows.Forms.TextBox textBox_solvingprocess;

    }
}