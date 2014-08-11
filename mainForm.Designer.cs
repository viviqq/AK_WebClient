namespace WebClient
{
    partial class mainForm
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnTest = new System.Windows.Forms.Button();
            this.txResult = new System.Windows.Forms.RichTextBox();
            this.txtjson = new System.Windows.Forms.RichTextBox();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbNote = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.lbLastDt = new System.Windows.Forms.Label();
            this.lbNowDt = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnTest
            // 
            this.BtnTest.Location = new System.Drawing.Point(35, 264);
            this.BtnTest.Name = "BtnTest";
            this.BtnTest.Size = new System.Drawing.Size(117, 45);
            this.BtnTest.TabIndex = 1;
            this.BtnTest.Text = "订单数据导入";
            this.BtnTest.UseVisualStyleBackColor = true;
            this.BtnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // txResult
            // 
            this.txResult.Location = new System.Drawing.Point(321, 140);
            this.txResult.Name = "txResult";
            this.txResult.Size = new System.Drawing.Size(276, 138);
            this.txResult.TabIndex = 3;
            this.txResult.Text = "";
            this.txResult.Visible = false;
            // 
            // txtjson
            // 
            this.txtjson.Location = new System.Drawing.Point(321, 284);
            this.txtjson.Name = "txtjson";
            this.txtjson.Size = new System.Drawing.Size(276, 123);
            this.txtjson.TabIndex = 4;
            this.txtjson.Text = "";
            this.txtjson.Visible = false;
            // 
            // dtStart
            // 
            this.dtStart.Location = new System.Drawing.Point(112, 108);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(165, 21);
            this.dtStart.TabIndex = 7;
            // 
            // dtEnd
            // 
            this.dtEnd.Location = new System.Drawing.Point(110, 143);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(167, 21);
            this.dtEnd.TabIndex = 8;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(33, 199);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(262, 28);
            this.progressBar1.TabIndex = 9;
            // 
            // lbNote
            // 
            this.lbNote.AutoSize = true;
            this.lbNote.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lbNote.Location = new System.Drawing.Point(31, 184);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(29, 12);
            this.lbNote.TabIndex = 11;
            this.lbNote.Text = "准备";
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(176, 184);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(0, 12);
            this.lbCount.TabIndex = 12;
            // 
            // lbLastDt
            // 
            this.lbLastDt.AutoSize = true;
            this.lbLastDt.Location = new System.Drawing.Point(33, 39);
            this.lbLastDt.Name = "lbLastDt";
            this.lbLastDt.Size = new System.Drawing.Size(83, 12);
            this.lbLastDt.TabIndex = 13;
            this.lbLastDt.Text = "上次导入日期:";
            // 
            // lbNowDt
            // 
            this.lbNowDt.AutoSize = true;
            this.lbNowDt.Location = new System.Drawing.Point(31, 72);
            this.lbNowDt.Name = "lbNowDt";
            this.lbNowDt.Size = new System.Drawing.Size(83, 12);
            this.lbNowDt.TabIndex = 14;
            this.lbNowDt.Text = "当 前 日  期:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "开始时间:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "结束时间:";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(178, 264);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(117, 45);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 321);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbNowDt);
            this.Controls.Add(this.lbLastDt);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.lbNote);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dtEnd);
            this.Controls.Add(this.dtStart);
            this.Controls.Add(this.txtjson);
            this.Controls.Add(this.txResult);
            this.Controls.Add(this.BtnTest);
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "爱凯国际速卖通订单导入程序";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnTest;
        private System.Windows.Forms.RichTextBox txResult;
        private System.Windows.Forms.RichTextBox txtjson;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbNote;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.Label lbLastDt;
        private System.Windows.Forms.Label lbNowDt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
    }
}

