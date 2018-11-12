namespace TrafficViewerControls
{
	partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            this.label1 = new System.Windows.Forms.Label();
            this._listExporters = new System.Windows.Forms.ComboBox();
            this._buttonExport = new System.Windows.Forms.Button();
            this._fileName = new TrafficViewerControls.FileSelector();
            this._checkReplaceHost = new System.Windows.Forms.CheckBox();
            this._textNewHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._textNewPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Export type:";
            // 
            // _listExporters
            // 
            this._listExporters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._listExporters.FormattingEnabled = true;
            this._listExporters.Location = new System.Drawing.Point(79, 10);
            this._listExporters.Name = "_listExporters";
            this._listExporters.Size = new System.Drawing.Size(284, 21);
            this._listExporters.TabIndex = 3;
            this._listExporters.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // _buttonExport
            // 
            this._buttonExport.Location = new System.Drawing.Point(158, 186);
            this._buttonExport.Name = "_buttonExport";
            this._buttonExport.Size = new System.Drawing.Size(75, 23);
            this._buttonExport.TabIndex = 5;
            this._buttonExport.Text = "Export";
            this._buttonExport.UseVisualStyleBackColor = true;
            this._buttonExport.Click += new System.EventHandler(this.ExportClick);
            // 
            // _fileName
            // 
            this._fileName.BackColor = System.Drawing.Color.Transparent;
            this._fileName.CheckFileExists = true;
            this._fileName.Filter = "";
            this._fileName.Label = "File name:";
            this._fileName.Location = new System.Drawing.Point(8, 43);
            this._fileName.Margin = new System.Windows.Forms.Padding(4);
            this._fileName.Name = "_fileName";
            this._fileName.Size = new System.Drawing.Size(355, 33);
            this._fileName.TabIndex = 1;
            // 
            // _checkReplaceHost
            // 
            this._checkReplaceHost.AutoSize = true;
            this._checkReplaceHost.Location = new System.Drawing.Point(14, 109);
            this._checkReplaceHost.Name = "_checkReplaceHost";
            this._checkReplaceHost.Size = new System.Drawing.Size(89, 17);
            this._checkReplaceHost.TabIndex = 6;
            this._checkReplaceHost.Text = "Replace host";
            this._checkReplaceHost.UseVisualStyleBackColor = true;
            this._checkReplaceHost.CheckedChanged += new System.EventHandler(this.ReplaceHostCheckedChanged);
            // 
            // _textNewHost
            // 
            this._textNewHost.Location = new System.Drawing.Point(79, 141);
            this._textNewHost.Name = "_textNewHost";
            this._textNewHost.ReadOnly = true;
            this._textNewHost.Size = new System.Drawing.Size(154, 20);
            this._textNewHost.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "New host:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(250, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "New port:";
            // 
            // _textNewPort
            // 
            this._textNewPort.Location = new System.Drawing.Point(309, 141);
            this._textNewPort.Name = "_textNewPort";
            this._textNewPort.ReadOnly = true;
            this._textNewPort.Size = new System.Drawing.Size(54, 20);
            this._textNewPort.TabIndex = 10;
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 235);
            this.Controls.Add(this._textNewPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._textNewHost);
            this.Controls.Add(this._checkReplaceHost);
            this.Controls.Add(this._buttonExport);
            this.Controls.Add(this._listExporters);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._fileName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(364, 211);
            this.Name = "ExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private FileSelector _fileName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _listExporters;
		private System.Windows.Forms.Button _buttonExport;
		private System.Windows.Forms.CheckBox _checkReplaceHost;
		private System.Windows.Forms.TextBox _textNewHost;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _textNewPort;

	}
}