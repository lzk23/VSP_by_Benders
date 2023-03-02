namespace ScheduleSys.SystemSetting
{
    partial class TrainColor
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
            this.colorDialog_traincolor = new System.Windows.Forms.ColorDialog();
            this.dataGridView_traincolor = new System.Windows.Forms.DataGridView();
            this.button_settingcolor = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.button_readcolor = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_traincolor)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_traincolor
            // 
            this.dataGridView_traincolor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_traincolor.Location = new System.Drawing.Point(12, 12);
            this.dataGridView_traincolor.Name = "dataGridView_traincolor";
            this.dataGridView_traincolor.RowTemplate.Height = 23;
            this.dataGridView_traincolor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_traincolor.Size = new System.Drawing.Size(296, 388);
            this.dataGridView_traincolor.TabIndex = 0;
            // 
            // button_settingcolor
            // 
            this.button_settingcolor.Location = new System.Drawing.Point(352, 38);
            this.button_settingcolor.Name = "button_settingcolor";
            this.button_settingcolor.Size = new System.Drawing.Size(141, 23);
            this.button_settingcolor.TabIndex = 1;
            this.button_settingcolor.Text = "Setting Color";
            this.button_settingcolor.UseVisualStyleBackColor = true;
            this.button_settingcolor.Click += new System.EventHandler(this.button_settingcolor_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(352, 87);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(141, 23);
            this.button_save.TabIndex = 2;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_readcolor
            // 
            this.button_readcolor.Location = new System.Drawing.Point(352, 139);
            this.button_readcolor.Name = "button_readcolor";
            this.button_readcolor.Size = new System.Drawing.Size(141, 23);
            this.button_readcolor.TabIndex = 3;
            this.button_readcolor.Text = "Read Color";
            this.button_readcolor.UseVisualStyleBackColor = true;
            this.button_readcolor.Click += new System.EventHandler(this.button_readcolor_Click);
            // 
            // TrainColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 403);
            this.Controls.Add(this.button_readcolor);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_settingcolor);
            this.Controls.Add(this.dataGridView_traincolor);
            this.Name = "TrainColor";
            this.Text = "TrainColor";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_traincolor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog_traincolor;
        private System.Windows.Forms.DataGridView dataGridView_traincolor;
        private System.Windows.Forms.Button button_settingcolor;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_readcolor;
    }
}