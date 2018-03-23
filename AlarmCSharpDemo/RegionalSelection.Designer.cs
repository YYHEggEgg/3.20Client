namespace AlarmCSharpDemo
{
    partial class RegionalSelection
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
            this.AreaList = new System.Windows.Forms.ListBox();
            this.AddAreaBtn = new System.Windows.Forms.Button();
            this.EnterBtn = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AreaList
            // 
            this.AreaList.FormattingEnabled = true;
            this.AreaList.ItemHeight = 12;
            this.AreaList.Location = new System.Drawing.Point(46, 37);
            this.AreaList.Name = "AreaList";
            this.AreaList.Size = new System.Drawing.Size(208, 148);
            this.AreaList.TabIndex = 0;
            // 
            // AddAreaBtn
            // 
            this.AddAreaBtn.Location = new System.Drawing.Point(12, 205);
            this.AddAreaBtn.Name = "AddAreaBtn";
            this.AddAreaBtn.Size = new System.Drawing.Size(75, 23);
            this.AddAreaBtn.TabIndex = 1;
            this.AddAreaBtn.Text = "添加地区";
            this.AddAreaBtn.UseVisualStyleBackColor = true;
            this.AddAreaBtn.Click += new System.EventHandler(this.AddAreaBtn_Click);
            // 
            // EnterBtn
            // 
            this.EnterBtn.Location = new System.Drawing.Point(116, 205);
            this.EnterBtn.Name = "EnterBtn";
            this.EnterBtn.Size = new System.Drawing.Size(75, 23);
            this.EnterBtn.TabIndex = 2;
            this.EnterBtn.Text = "登录";
            this.EnterBtn.UseVisualStyleBackColor = true;
            this.EnterBtn.Click += new System.EventHandler(this.EnterBtn_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Location = new System.Drawing.Point(223, 205);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(75, 23);
            this.ExitBtn.TabIndex = 3;
            this.ExitBtn.Text = "退出";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "地区:";
            // 
            // RegionalSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 245);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.EnterBtn);
            this.Controls.Add(this.AddAreaBtn);
            this.Controls.Add(this.AreaList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegionalSelection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegionalSelection";
            this.Load += new System.EventHandler(this.RegionalSelection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox AreaList;
        private System.Windows.Forms.Button AddAreaBtn;
        private System.Windows.Forms.Button EnterBtn;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Label label1;
    }
}