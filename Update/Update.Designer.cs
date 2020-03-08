namespace Update
{
    partial class Update
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            this.pb_Total = new System.Windows.Forms.ProgressBar();
            this.txt_FileAmt = new System.Windows.Forms.Label();
            this.txt_CurrentFile = new System.Windows.Forms.Label();
            this.txt_FileName = new System.Windows.Forms.Label();
            this.pb_Current = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_FileCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pb_Total
            // 
            this.pb_Total.Location = new System.Drawing.Point(97, 38);
            this.pb_Total.Name = "pb_Total";
            this.pb_Total.Size = new System.Drawing.Size(398, 23);
            this.pb_Total.TabIndex = 0;
            // 
            // txt_FileAmt
            // 
            this.txt_FileAmt.AutoSize = true;
            this.txt_FileAmt.Location = new System.Drawing.Point(421, 73);
            this.txt_FileAmt.Name = "txt_FileAmt";
            this.txt_FileAmt.Size = new System.Drawing.Size(0, 15);
            this.txt_FileAmt.TabIndex = 1;
            // 
            // txt_CurrentFile
            // 
            this.txt_CurrentFile.AutoSize = true;
            this.txt_CurrentFile.Location = new System.Drawing.Point(12, 121);
            this.txt_CurrentFile.Name = "txt_CurrentFile";
            this.txt_CurrentFile.Size = new System.Drawing.Size(0, 15);
            this.txt_CurrentFile.TabIndex = 3;
            // 
            // txt_FileName
            // 
            this.txt_FileName.AutoSize = true;
            this.txt_FileName.Location = new System.Drawing.Point(94, 73);
            this.txt_FileName.Name = "txt_FileName";
            this.txt_FileName.Size = new System.Drawing.Size(75, 15);
            this.txt_FileName.TabIndex = 4;
            this.txt_FileName.Text = "当前文件:";
            // 
            // pb_Current
            // 
            this.pb_Current.Location = new System.Drawing.Point(97, 91);
            this.pb_Current.Name = "pb_Current";
            this.pb_Current.Size = new System.Drawing.Size(398, 23);
            this.pb_Current.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "总进度:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "当前文件:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(349, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "文件大小:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "当前进度:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "文件个数:";
            // 
            // txt_FileCount
            // 
            this.txt_FileCount.AutoSize = true;
            this.txt_FileCount.Location = new System.Drawing.Point(94, 18);
            this.txt_FileCount.Name = "txt_FileCount";
            this.txt_FileCount.Size = new System.Drawing.Size(15, 15);
            this.txt_FileCount.TabIndex = 12;
            this.txt_FileCount.Text = "0";
            // 
            // Update
            // 
            this.ClientSize = new System.Drawing.Size(524, 141);
            this.Controls.Add(this.txt_FileCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pb_Current);
            this.Controls.Add(this.txt_FileName);
            this.Controls.Add(this.txt_CurrentFile);
            this.Controls.Add(this.txt_FileAmt);
            this.Controls.Add(this.pb_Total);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Update";
            this.Text = "自动更新";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pb_Total;
        private System.Windows.Forms.Label txt_FileAmt;
        private System.Windows.Forms.Label txt_CurrentFile;
        private System.Windows.Forms.Label txt_FileName;
        private System.Windows.Forms.ProgressBar pb_Current;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txt_FileCount;
    }
}

