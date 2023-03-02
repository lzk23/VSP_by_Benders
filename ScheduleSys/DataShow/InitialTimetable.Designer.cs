namespace ScheduleSys.DataShow
{
    partial class InitialTimetable
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
            this.dataGridView_initialtimetable = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_initialtimetable)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_initialtimetable
            // 
            this.dataGridView_initialtimetable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.dataGridView_initialtimetable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_initialtimetable.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_initialtimetable.Name = "dataGridView_initialtimetable";
            this.dataGridView_initialtimetable.RowTemplate.Height = 23;
            this.dataGridView_initialtimetable.Size = new System.Drawing.Size(719, 602);
            this.dataGridView_initialtimetable.TabIndex = 0;
            // 
            // InitialTimetable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 614);
            this.Controls.Add(this.dataGridView_initialtimetable);
            this.Name = "InitialTimetable";
            this.Text = "InitialTimetable";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_initialtimetable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView_initialtimetable;

    }
}