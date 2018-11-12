namespace TrafficViewerControls.TextBoxes
{
	partial class GetTCPConnectionDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetTCPConnectionDetails));
            this._checkIsSecure = new System.Windows.Forms.CheckBox();
            this._textHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._textPort = new System.Windows.Forms.TextBox();
            this._buttonStart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _checkIsSecure
            // 
            this._checkIsSecure.AutoSize = true;
            this._checkIsSecure.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkIsSecure.Location = new System.Drawing.Point(30, 88);
            this._checkIsSecure.Name = "_checkIsSecure";
            this._checkIsSecure.Size = new System.Drawing.Size(89, 18);
            this._checkIsSecure.TabIndex = 3;
            this._checkIsSecure.Text = "Is Secure";
            this._checkIsSecure.UseVisualStyleBackColor = true;
            // 
            // _textHost
            // 
            this._textHost.Location = new System.Drawing.Point(121, 29);
            this._textHost.Name = "_textHost";
            this._textHost.Size = new System.Drawing.Size(311, 20);
            this._textHost.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 22);
            this.label1.TabIndex = 7;
            this.label1.Text = "Host name:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port:";
            // 
            // _textPort
            // 
            this._textPort.Location = new System.Drawing.Point(121, 60);
            this._textPort.Name = "_textPort";
            this._textPort.Size = new System.Drawing.Size(70, 20);
            this._textPort.TabIndex = 8;
            this._textPort.TextChanged += new System.EventHandler(this._textPort_TextChanged);
            // 
            // _buttonStart
            // 
            this._buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonStart.BackColor = System.Drawing.Color.Transparent;
            this._buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonStart.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonStart.ForeColor = System.Drawing.Color.White;
            this._buttonStart.Location = new System.Drawing.Point(135, 131);
            this._buttonStart.Name = "_buttonStart";
            this._buttonStart.Size = new System.Drawing.Size(97, 23);
            this._buttonStart.TabIndex = 10;
            this._buttonStart.Text = "OK";
            this._buttonStart.UseVisualStyleBackColor = false;
            this._buttonStart.Click += new System.EventHandler(this._buttonStart_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(250, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GetTCPConnectionDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(495, 167);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._buttonStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._textPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._textHost);
            this.Controls.Add(this._checkIsSecure);
            this.ForeColor = System.Drawing.Color.Lime;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(511, 205);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(511, 205);
            this.Name = "GetTCPConnectionDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TCP Connection Details";
            this.Activated += new System.EventHandler(this.GetTCPConnectionDetails_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _checkIsSecure;
		private System.Windows.Forms.TextBox _textHost;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _textPort;
		private System.Windows.Forms.Button _buttonStart;
		private System.Windows.Forms.Button button1;
	}
}