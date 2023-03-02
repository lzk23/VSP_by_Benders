namespace ScheduleSys.DataShow
{
    partial class ArriveDeparture
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
            this.pictureBox_arrivedeparture = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_arrivedeparture)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox_arrivedeparture
            // 
            this.pictureBox_arrivedeparture.Location = new System.Drawing.Point(3, 3);
            this.pictureBox_arrivedeparture.Name = "pictureBox_arrivedeparture";
            this.pictureBox_arrivedeparture.Size = new System.Drawing.Size(631, 351);
            this.pictureBox_arrivedeparture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_arrivedeparture.TabIndex = 0;
            this.pictureBox_arrivedeparture.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox_arrivedeparture);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1118, 462);
            this.panel1.TabIndex = 1;
            // 
            // ArriveDeparture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 474);
            this.Controls.Add(this.panel1);
            this.Name = "ArriveDeparture";
            this.Text = "ArriveDeparture";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_arrivedeparture)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_arrivedeparture;
        private System.Windows.Forms.Panel panel1;
    }
}