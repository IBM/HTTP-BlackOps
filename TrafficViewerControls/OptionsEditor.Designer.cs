/******************************************************************
* IBM Confidential
* OCO Source Materials
* IBM Rational Traffic Viewer
* (c) Copyright IBM Corp. 2010 All Rights Reserved.
* 
* The source code for this program is not published or otherwise
* divested of its trade secrets, irrespective of what has been
* deposited with the U.S. Copyright Office.
******************************************************************/
namespace TrafficViewerControls
{
	partial class OptionsEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsEditor));
			this._buttonOk = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._tabControl = new System.Windows.Forms.TabControl();
			this._tabGeneral = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._checkOptSpeed = new System.Windows.Forms.RadioButton();
			this._checkOptMemory = new System.Windows.Forms.RadioButton();
			this._tabServer = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this._gridVarDefs = new TrafficViewerControls.OptionsGrid();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this._numProxyPort = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._checkProxyBrowser = new System.Windows.Forms.RadioButton();
			this._checkProxyStrict = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this._tabControl.SuspendLayout();
			this._tabGeneral.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this._tabServer.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._numProxyPort)).BeginInit();
			this.SuspendLayout();
			// 
			// _buttonOk
			// 
			this._buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this._buttonOk.Location = new System.Drawing.Point(134, 462);
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.Size = new System.Drawing.Size(75, 23);
			this._buttonOk.TabIndex = 0;
			this._buttonOk.Text = "OK";
			this._buttonOk.UseVisualStyleBackColor = true;
			this._buttonOk.Click += new System.EventHandler(this.OkClick);
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this._buttonCancel.Location = new System.Drawing.Point(218, 462);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(75, 23);
			this._buttonCancel.TabIndex = 1;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.CancelClick);
			// 
			// _tabControl
			// 
			this._tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tabControl.Controls.Add(this._tabGeneral);
			this._tabControl.Controls.Add(this._tabServer);
			this._tabControl.Location = new System.Drawing.Point(4, 3);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(426, 456);
			this._tabControl.TabIndex = 2;
			// 
			// _tabGeneral
			// 
			this._tabGeneral.Controls.Add(this.groupBox2);
			this._tabGeneral.Location = new System.Drawing.Point(4, 22);
			this._tabGeneral.Name = "_tabGeneral";
			this._tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this._tabGeneral.Size = new System.Drawing.Size(418, 430);
			this._tabGeneral.TabIndex = 0;
			this._tabGeneral.Text = "General";
			this._tabGeneral.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._checkOptSpeed);
			this.groupBox2.Controls.Add(this._checkOptMemory);
			this.groupBox2.Location = new System.Drawing.Point(4, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(408, 119);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Performance";
			// 
			// _checkOptSpeed
			// 
			this._checkOptSpeed.AutoSize = true;
			this._checkOptSpeed.Location = new System.Drawing.Point(206, 30);
			this._checkOptSpeed.Name = "_checkOptSpeed";
			this._checkOptSpeed.Size = new System.Drawing.Size(112, 17);
			this._checkOptSpeed.TabIndex = 1;
			this._checkOptSpeed.Text = "Optimize for speed";
			this._checkOptSpeed.UseVisualStyleBackColor = true;
			this._checkOptSpeed.CheckedChanged += new System.EventHandler(this.OptSpeedCheckedChanged);
			// 
			// _checkOptMemory
			// 
			this._checkOptMemory.AutoSize = true;
			this._checkOptMemory.Checked = true;
			this._checkOptMemory.Location = new System.Drawing.Point(22, 30);
			this._checkOptMemory.Name = "_checkOptMemory";
			this._checkOptMemory.Size = new System.Drawing.Size(119, 17);
			this._checkOptMemory.TabIndex = 0;
			this._checkOptMemory.TabStop = true;
			this._checkOptMemory.Text = "Optimize for memory";
			this._checkOptMemory.UseVisualStyleBackColor = true;
			this._checkOptMemory.CheckedChanged += new System.EventHandler(this.OptMemoryCheckedChanged);
			// 
			// _tabServer
			// 
			this._tabServer.Controls.Add(this.groupBox4);
			this._tabServer.Controls.Add(this.groupBox3);
			this._tabServer.Location = new System.Drawing.Point(4, 22);
			this._tabServer.Name = "_tabServer";
			this._tabServer.Padding = new System.Windows.Forms.Padding(3);
			this._tabServer.Size = new System.Drawing.Size(418, 430);
			this._tabServer.TabIndex = 1;
			this._tabServer.Text = "HTTP";
			this._tabServer.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this._gridVarDefs);
			this.groupBox4.Location = new System.Drawing.Point(6, 153);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(408, 237);
			this.groupBox4.TabIndex = 7;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Variable Definitions";
			// 
			// _gridVarDefs
			// 
			this._gridVarDefs.BackColor = System.Drawing.Color.Transparent;
			this._gridVarDefs.Columns = "Location:Path,Query,Cookies,Body\r\nDefinition";
			this._gridVarDefs.LabelText = "";
			this._gridVarDefs.Location = new System.Drawing.Point(6, 14);
			this._gridVarDefs.Name = "_gridVarDefs";
			this._gridVarDefs.Size = new System.Drawing.Size(396, 223);
			this._gridVarDefs.TabIndex = 0;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this._numProxyPort);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this._checkProxyBrowser);
			this.groupBox3.Controls.Add(this._checkProxyStrict);
			this.groupBox3.Location = new System.Drawing.Point(7, 14);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(408, 128);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Proxy";
			// 
			// _numProxyPort
			// 
			this._numProxyPort.Location = new System.Drawing.Point(113, 24);
			this._numProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this._numProxyPort.Name = "_numProxyPort";
			this._numProxyPort.Size = new System.Drawing.Size(102, 20);
			this._numProxyPort.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(21, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(66, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Proxy Mode:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Proxy Port:";
			// 
			// _checkProxyBrowser
			// 
			this._checkProxyBrowser.AutoSize = true;
			this._checkProxyBrowser.Location = new System.Drawing.Point(113, 72);
			this._checkProxyBrowser.Name = "_checkProxyBrowser";
			this._checkProxyBrowser.Size = new System.Drawing.Size(102, 17);
			this._checkProxyBrowser.TabIndex = 1;
			this._checkProxyBrowser.Text = "Browser Friendly";
			this._checkProxyBrowser.UseVisualStyleBackColor = true;
			this._checkProxyBrowser.CheckedChanged += new System.EventHandler(this.ProxyBrowserCheckedChanged);
			// 
			// _checkProxyStrict
			// 
			this._checkProxyStrict.AutoSize = true;
			this._checkProxyStrict.Checked = true;
			this._checkProxyStrict.Location = new System.Drawing.Point(113, 49);
			this._checkProxyStrict.Name = "_checkProxyStrict";
			this._checkProxyStrict.Size = new System.Drawing.Size(49, 17);
			this._checkProxyStrict.TabIndex = 0;
			this._checkProxyStrict.TabStop = true;
			this._checkProxyStrict.Text = "Strict";
			this._checkProxyStrict.UseVisualStyleBackColor = true;
			this._checkProxyStrict.CheckedChanged += new System.EventHandler(this.ProxyStrictCheckedChanged);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(231, 30);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(112, 17);
			this.radioButton1.TabIndex = 1;
			this.radioButton1.Text = "Optimize for speed";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Checked = true;
			this.radioButton2.Location = new System.Drawing.Point(22, 30);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(119, 17);
			this.radioButton2.TabIndex = 0;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "Optimize for memory";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// OptionsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 493);
			this.Controls.Add(this._tabControl);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonOk);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsEditor";
			this.Text = "Traffic Viewer Options";
			this._tabControl.ResumeLayout(false);
			this._tabGeneral.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this._tabServer.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._numProxyPort)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _buttonOk;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _tabGeneral;
		private System.Windows.Forms.TabPage _tabServer;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton _checkOptMemory;
		private System.Windows.Forms.RadioButton _checkOptSpeed;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton _checkProxyBrowser;
		private System.Windows.Forms.RadioButton _checkProxyStrict;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox4;
		private OptionsGrid _gridVarDefs;
		private System.Windows.Forms.NumericUpDown _numProxyPort;
	}
}