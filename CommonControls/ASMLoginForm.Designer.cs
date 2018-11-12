namespace CommonControls
{
	partial class ASMLoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ASMLoginForm));
            this.label1 = new System.Windows.Forms.Label();
            this._textUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._textHostAndPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._textPassword = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this._checkRemember = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name:";
            // 
            // _textUserName
            // 
            this._textUserName.Location = new System.Drawing.Point(147, 67);
            this._textUserName.Name = "_textUserName";
            this._textUserName.Size = new System.Drawing.Size(216, 20);
            this._textUserName.TabIndex = 1;
            this._textUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextKeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Login to AppScan Enterprise";
            // 
            // _textHostAndPort
            // 
            this._textHostAndPort.Location = new System.Drawing.Point(147, 40);
            this._textHostAndPort.Name = "_textHostAndPort";
            this._textHostAndPort.Size = new System.Drawing.Size(216, 20);
            this._textHostAndPort.TabIndex = 4;
            this._textHostAndPort.Text = "<appscan server>:9443";
            this._textHostAndPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextKeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Host and Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password:";
            // 
            // _textPassword
            // 
            this._textPassword.Location = new System.Drawing.Point(147, 96);
            this._textPassword.Name = "_textPassword";
            this._textPassword.PasswordChar = '*';
            this._textPassword.Size = new System.Drawing.Size(216, 20);
            this._textPassword.TabIndex = 2;
            this._textPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextKeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(172, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.LoginClick);
            // 
            // _checkRemember
            // 
            this._checkRemember.AutoSize = true;
            this._checkRemember.Checked = true;
            this._checkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkRemember.Location = new System.Drawing.Point(35, 127);
            this._checkRemember.Name = "_checkRemember";
            this._checkRemember.Size = new System.Drawing.Size(77, 17);
            this._checkRemember.TabIndex = 6;
            this._checkRemember.Text = "Remember";
            this._checkRemember.UseVisualStyleBackColor = true;
            // 
            // ASMLoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 192);
            this.Controls.Add(this._checkRemember);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._textPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._textHostAndPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._textUserName);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(437, 230);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(437, 230);
            this.Name = "ASMLoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login to AppScan Enterprise";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _textUserName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _textHostAndPort;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _textPassword;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox _checkRemember;
	}
}