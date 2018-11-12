namespace TrafficViewerControls.Browsing
{
	partial class CredentialsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CredentialsForm));
            this.label1 = new System.Windows.Forms.Label();
            this._textUser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._textPass = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._textDomain = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _textUser
            // 
            resources.ApplyResources(this._textUser, "_textUser");
            this._textUser.Name = "_textUser";
            this._textUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserKeyPress);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // _textPass
            // 
            resources.ApplyResources(this._textPass, "_textPass");
            this._textPass.Name = "_textPass";
            this._textPass.UseSystemPasswordChar = true;
            this._textPass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PassKeyPress);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OKClick);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // _textDomain
            // 
            resources.ApplyResources(this._textDomain, "_textDomain");
            this._textDomain.Name = "_textDomain";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // CredentialsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._textDomain);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._textPass);
            this.Controls.Add(this._textUser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CredentialsForm";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _textUser;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _textPass;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _textDomain;
        private System.Windows.Forms.Label label4;
	}
}