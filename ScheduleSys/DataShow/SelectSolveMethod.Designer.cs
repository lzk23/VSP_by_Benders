namespace ScheduleSys.DataShow
{
    partial class SelectSolveMethod
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
            this.btn_run = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBox_solvingmethod = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // btn_run
            // 
            this.btn_run.Location = new System.Drawing.Point(264, 206);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(89, 29);
            this.btn_run.TabIndex = 0;
            this.btn_run.Text = "Run";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 1;
            // 
            // checkedListBox_solvingmethod
            // 
            this.checkedListBox_solvingmethod.FormattingEnabled = true;
            this.checkedListBox_solvingmethod.Location = new System.Drawing.Point(44, 29);
            this.checkedListBox_solvingmethod.Name = "checkedListBox_solvingmethod";
            this.checkedListBox_solvingmethod.Size = new System.Drawing.Size(120, 84);
            this.checkedListBox_solvingmethod.TabIndex = 4;
            // 
            // SelectSolveMethod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 247);
            this.Controls.Add(this.checkedListBox_solvingmethod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_run);
            this.Name = "SelectSolveMethod";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectSolveMethod";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox checkedListBox_solvingmethod;


    }
}