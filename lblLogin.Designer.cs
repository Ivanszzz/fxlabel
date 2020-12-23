namespace AHLabelPrint
{
	partial class lblLogin
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(lblLogin));
			this.label1 = new System.Windows.Forms.Label();
			this.txt_pwd = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btn_Login = new System.Windows.Forms.Button();
			this.txt_username = new System.Windows.Forms.ComboBox();
			this.chk_remember = new System.Windows.Forms.CheckBox();
			this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(71, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "用户名:";
			// 
			// txt_pwd
			// 
			this.txt_pwd.Location = new System.Drawing.Point(124, 82);
			this.txt_pwd.Name = "txt_pwd";
			this.txt_pwd.PasswordChar = '*';
			this.txt_pwd.Size = new System.Drawing.Size(162, 21);
			this.txt_pwd.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(83, 85);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "密码:";
			// 
			// btn_Login
			// 
			this.btn_Login.Location = new System.Drawing.Point(155, 144);
			this.btn_Login.Name = "btn_Login";
			this.btn_Login.Size = new System.Drawing.Size(75, 23);
			this.btn_Login.TabIndex = 3;
			this.btn_Login.Text = "登录";
			this.btn_Login.UseVisualStyleBackColor = true;
			this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
			// 
			// txt_username
			// 
			this.txt_username.FormattingEnabled = true;
			this.txt_username.Location = new System.Drawing.Point(124, 46);
			this.txt_username.Name = "txt_username";
			this.txt_username.Size = new System.Drawing.Size(162, 20);
			this.txt_username.TabIndex = 0;
			this.txt_username.SelectedValueChanged += new System.EventHandler(this.txt_username_SelectedValueChanged);
			// 
			// chk_remember
			// 
			this.chk_remember.AutoSize = true;
			this.chk_remember.Location = new System.Drawing.Point(124, 110);
			this.chk_remember.Name = "chk_remember";
			this.chk_remember.Size = new System.Drawing.Size(72, 16);
			this.chk_remember.TabIndex = 2;
			this.chk_remember.Text = "记住密码";
			this.chk_remember.UseVisualStyleBackColor = true;
			// 
			// skinEngine1
			// 
			this.skinEngine1.@__DrawButtonFocusRectangle = true;
			this.skinEngine1.DisabledButtonTextColor = System.Drawing.Color.Gray;
			this.skinEngine1.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
			this.skinEngine1.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.skinEngine1.SerialNumber = "";
			this.skinEngine1.SkinFile = null;
			// 
			// lblLogin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(397, 219);
			this.Controls.Add(this.chk_remember);
			this.Controls.Add(this.txt_username);
			this.Controls.Add(this.btn_Login);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txt_pwd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "lblLogin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "登录";
			this.Load += new System.EventHandler(this.lblLogin_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txt_pwd;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btn_Login;
		private System.Windows.Forms.ComboBox txt_username;
		private System.Windows.Forms.CheckBox chk_remember;
		private Sunisoft.IrisSkin.SkinEngine skinEngine1;
	}
}