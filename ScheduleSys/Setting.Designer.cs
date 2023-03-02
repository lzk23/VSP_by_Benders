namespace ScheduleSys
{
    partial class Setting
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
            this.Setting_Paremeterbtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Setting_Paremeterbtn
            // 
            this.Setting_Paremeterbtn.Location = new System.Drawing.Point(30, 53);
            this.Setting_Paremeterbtn.Name = "Setting_Paremeterbtn";
            this.Setting_Paremeterbtn.Size = new System.Drawing.Size(75, 23);
            this.Setting_Paremeterbtn.TabIndex = 0;
            this.Setting_Paremeterbtn.Text = "Paremeter";
            this.Setting_Paremeterbtn.UseVisualStyleBackColor = true;
            this.Setting_Paremeterbtn.Click += new System.EventHandler(this.Setting_Paremeterbtn_Click);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 412);
            this.Controls.Add(this.Setting_Paremeterbtn);
            this.Name = "Setting";
            this.Text = "Setting";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Setting_Paremeterbtn;


    }
}