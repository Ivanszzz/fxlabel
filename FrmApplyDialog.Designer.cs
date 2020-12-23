namespace AHLabelPrint
{
	partial class FrmApplyDialog
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
			this.label1 = new System.Windows.Forms.Label();
			this.lbl_selRowsCount = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comb_applytype = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comb_applyreason = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txt_remark = new System.Windows.Forms.TextBox();
			this.btn_sumbitApply = new System.Windows.Forms.Button();
			this.btn_cancelApply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "选中标签行数:";
			// 
			// lbl_selRowsCount
			// 
			this.lbl_selRowsCount.AutoSize = true;
			this.lbl_selRowsCount.Location = new System.Drawing.Point(93, 12);
			this.lbl_selRowsCount.Name = "lbl_selRowsCount";
			this.lbl_selRowsCount.Size = new System.Drawing.Size(41, 12);
			this.lbl_selRowsCount.TabIndex = 1;
			this.lbl_selRowsCount.Text = "label2";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(32, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "申请类型:";
			// 
			// comb_applytype
			// 
			this.comb_applytype.Enabled = false;
			this.comb_applytype.FormattingEnabled = true;
			this.comb_applytype.Location = new System.Drawing.Point(95, 34);
			this.comb_applytype.Name = "comb_applytype";
			this.comb_applytype.Size = new System.Drawing.Size(306, 20);
			this.comb_applytype.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(32, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "申请原因:";
			// 
			// comb_applyreason
			// 
			this.comb_applyreason.FormattingEnabled = true;
			this.comb_applyreason.Items.AddRange(new object[] {
            "标签丢失",
            "标签破损",
            "标签模糊",
            "标签错误",
            "打印机硬件故障",
            "打印机程序异常"});
			this.comb_applyreason.Location = new System.Drawing.Point(95, 60);
			this.comb_applyreason.Name = "comb_applyreason";
			this.comb_applyreason.Size = new System.Drawing.Size(306, 20);
			this.comb_applyreason.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(56, 89);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(35, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "备注:";
			// 
			// txt_remark
			// 
			this.txt_remark.Location = new System.Drawing.Point(96, 90);
			this.txt_remark.Multiline = true;
			this.txt_remark.Name = "txt_remark";
			this.txt_remark.Size = new System.Drawing.Size(305, 108);
			this.txt_remark.TabIndex = 3;
			// 
			// btn_sumbitApply
			// 
			this.btn_sumbitApply.Location = new System.Drawing.Point(123, 208);
			this.btn_sumbitApply.Name = "btn_sumbitApply";
			this.btn_sumbitApply.Size = new System.Drawing.Size(75, 23);
			this.btn_sumbitApply.TabIndex = 4;
			this.btn_sumbitApply.Text = "提交";
			this.btn_sumbitApply.UseVisualStyleBackColor = true;
			this.btn_sumbitApply.Click += new System.EventHandler(this.btn_sumbitApply_Click);
			// 
			// btn_cancelApply
			// 
			this.btn_cancelApply.Location = new System.Drawing.Point(214, 208);
			this.btn_cancelApply.Name = "btn_cancelApply";
			this.btn_cancelApply.Size = new System.Drawing.Size(75, 23);
			this.btn_cancelApply.TabIndex = 4;
			this.btn_cancelApply.Text = "取消";
			this.btn_cancelApply.UseVisualStyleBackColor = true;
			this.btn_cancelApply.Click += new System.EventHandler(this.btn_cancelApply_Click);
			// 
			// FrmApplyDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(403, 244);
			this.Controls.Add(this.btn_cancelApply);
			this.Controls.Add(this.btn_sumbitApply);
			this.Controls.Add(this.txt_remark);
			this.Controls.Add(this.comb_applyreason);
			this.Controls.Add(this.comb_applytype);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lbl_selRowsCount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmApplyDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "标签申请";
			this.Load += new System.EventHandler(this.FrmApplyDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbl_selRowsCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comb_applytype;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comb_applyreason;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txt_remark;
		private System.Windows.Forms.Button btn_sumbitApply;
		private System.Windows.Forms.Button btn_cancelApply;
	}
}