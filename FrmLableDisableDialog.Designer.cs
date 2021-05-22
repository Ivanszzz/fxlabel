
namespace AHLabelPrint
{
    partial class FrmLableDisableDialog
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
            this.labelinput_textBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Dgv_labellist = new System.Windows.Forms.DataGridView();
            this.md5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.workcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clientcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serialnumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.printdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.n_Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.reason_comboBox = new System.Windows.Forms.ComboBox();
            this.remake_label = new System.Windows.Forms.Label();
            this.remake_textBox = new System.Windows.Forms.TextBox();
            this.cancel_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_labellist)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "扫入标签:";
            // 
            // labelinput_textBox
            // 
            this.labelinput_textBox.Location = new System.Drawing.Point(77, 10);
            this.labelinput_textBox.Name = "labelinput_textBox";
            this.labelinput_textBox.Size = new System.Drawing.Size(621, 21);
            this.labelinput_textBox.TabIndex = 0;
            this.labelinput_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.labelinput_textBox_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Dgv_labellist);
            this.groupBox1.Location = new System.Drawing.Point(14, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(700, 168);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "标签信息";
            // 
            // Dgv_labellist
            // 
            this.Dgv_labellist.AllowUserToAddRows = false;
            this.Dgv_labellist.AllowUserToResizeColumns = false;
            this.Dgv_labellist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.Dgv_labellist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.md5,
            this.workcode,
            this.clientcode,
            this.serialnumber,
            this.printdate,
            this.creator,
            this.customer,
            this.n_Id});
            this.Dgv_labellist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Dgv_labellist.Location = new System.Drawing.Point(3, 17);
            this.Dgv_labellist.Name = "Dgv_labellist";
            this.Dgv_labellist.ReadOnly = true;
            this.Dgv_labellist.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.Dgv_labellist.RowTemplate.Height = 23;
            this.Dgv_labellist.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.Dgv_labellist.Size = new System.Drawing.Size(694, 148);
            this.Dgv_labellist.TabIndex = 0;
            this.Dgv_labellist.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.Dgv_labellist_UserDeletingRow);
            // 
            // md5
            // 
            this.md5.HeaderText = "标签标识码";
            this.md5.Name = "md5";
            this.md5.ReadOnly = true;
            this.md5.Visible = false;
            // 
            // workcode
            // 
            this.workcode.HeaderText = "工单号";
            this.workcode.Name = "workcode";
            this.workcode.ReadOnly = true;
            this.workcode.Width = 110;
            // 
            // clientcode
            // 
            this.clientcode.HeaderText = "客户料号";
            this.clientcode.Name = "clientcode";
            this.clientcode.ReadOnly = true;
            this.clientcode.Width = 110;
            // 
            // serialnumber
            // 
            this.serialnumber.HeaderText = "流水号";
            this.serialnumber.Name = "serialnumber";
            this.serialnumber.ReadOnly = true;
            this.serialnumber.Width = 60;
            // 
            // printdate
            // 
            this.printdate.HeaderText = "打印日期";
            this.printdate.Name = "printdate";
            this.printdate.ReadOnly = true;
            this.printdate.Width = 120;
            // 
            // creator
            // 
            this.creator.HeaderText = "打印者";
            this.creator.Name = "creator";
            this.creator.ReadOnly = true;
            this.creator.Width = 70;
            // 
            // customer
            // 
            this.customer.HeaderText = "包装名称";
            this.customer.Name = "customer";
            this.customer.ReadOnly = true;
            this.customer.Width = 110;
            // 
            // n_Id
            // 
            this.n_Id.HeaderText = "标签ID";
            this.n_Id.Name = "n_Id";
            this.n_Id.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "申请原因:";
            // 
            // reason_comboBox
            // 
            this.reason_comboBox.Enabled = false;
            this.reason_comboBox.FormattingEnabled = true;
            this.reason_comboBox.Items.AddRange(new object[] {
            "标签数量大于良品数"});
            this.reason_comboBox.Location = new System.Drawing.Point(77, 212);
            this.reason_comboBox.Name = "reason_comboBox";
            this.reason_comboBox.Size = new System.Drawing.Size(621, 20);
            this.reason_comboBox.TabIndex = 5;
            // 
            // remake_label
            // 
            this.remake_label.AutoSize = true;
            this.remake_label.Location = new System.Drawing.Point(17, 242);
            this.remake_label.Name = "remake_label";
            this.remake_label.Size = new System.Drawing.Size(53, 12);
            this.remake_label.TabIndex = 6;
            this.remake_label.Text = "备   注:";
            // 
            // remake_textBox
            // 
            this.remake_textBox.Location = new System.Drawing.Point(77, 238);
            this.remake_textBox.Multiline = true;
            this.remake_textBox.Name = "remake_textBox";
            this.remake_textBox.Size = new System.Drawing.Size(329, 43);
            this.remake_textBox.TabIndex = 7;
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(484, 258);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 8;
            this.cancel_button.Text = "取消";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(601, 258);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 9;
            this.OK_button.Text = "申请";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(412, 238);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(11, 12);
            this.label10.TabIndex = 34;
            this.label10.Text = "*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(704, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 35;
            this.label3.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(704, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 36;
            this.label4.Text = "*";
            // 
            // FrmLableDisableDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(726, 293);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.remake_textBox);
            this.Controls.Add(this.remake_label);
            this.Controls.Add(this.reason_comboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelinput_textBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FrmLableDisableDialog";
            this.ShowIcon = false;
            this.Text = "单张标签的作废申请";
            this.Load += new System.EventHandler(this.FrmLableDisableDialog_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_labellist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox labelinput_textBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView Dgv_labellist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox reason_comboBox;
        private System.Windows.Forms.Label remake_label;
        private System.Windows.Forms.TextBox remake_textBox;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.DataGridViewTextBoxColumn md5;
        private System.Windows.Forms.DataGridViewTextBoxColumn workcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn clientcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn serialnumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn printdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn creator;
        private System.Windows.Forms.DataGridViewTextBoxColumn customer;
        private System.Windows.Forms.DataGridViewTextBoxColumn n_Id;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}