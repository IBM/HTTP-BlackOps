namespace TrafficViewerControls.TextBoxes
{
	partial class TrafficTextBox
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrafficTextBox));
            this._textBox = new System.Windows.Forms.RichTextBox();
            this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._wrapMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._findMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.findAgainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._cutMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._copyMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._pasteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.encodeDecodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.urlEncodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.urlEncodeEverythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.urlDecodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.htmlEncodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.htmlDecodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.base64EncodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.base64DecodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.countToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.formatXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testXXEProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertXXETestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertSequenceVariableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertFuzzStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIEncodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIDecodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // _textBox
            // 
            this._textBox.AcceptsTab = true;
            this._textBox.BackColor = System.Drawing.SystemColors.Info;
            this._textBox.ContextMenuStrip = this._contextMenu;
            this._textBox.DetectUrls = false;
            resources.ApplyResources(this._textBox, "_textBox");
            this._textBox.Name = "_textBox";
            this._textBox.TabStop = false;
            // 
            // _contextMenu
            // 
            this._contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._wrapMenu,
            this.toolStripSeparator1,
            this._findMenu,
            this.findAgainToolStripMenuItem,
            this.toolStripSeparator2,
            this._cutMenu,
            this._copyMenu,
            this._pasteMenu,
            this.toolStripSeparator3,
            this.encodeDecodeToolStripMenuItem,
            this.toolStripSeparator4,
            this.countToolStripMenuItem,
            this.toolStripSeparator5,
            this.formatXMLToolStripMenuItem,
            this.testXXEProcessingToolStripMenuItem,
            this.insertXXETestToolStripMenuItem,
            this.insertSequenceVariableToolStripMenuItem,
            this.insertFuzzStringToolStripMenuItem});
            this._contextMenu.Name = "_contextMenu";
            resources.ApplyResources(this._contextMenu, "_contextMenu");
            // 
            // _wrapMenu
            // 
            this._wrapMenu.Checked = true;
            this._wrapMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this._wrapMenu.Name = "_wrapMenu";
            resources.ApplyResources(this._wrapMenu, "_wrapMenu");
            this._wrapMenu.Click += new System.EventHandler(this.WrapToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // _findMenu
            // 
            this._findMenu.Name = "_findMenu";
            resources.ApplyResources(this._findMenu, "_findMenu");
            this._findMenu.Click += new System.EventHandler(this.FindMenuClick);
            // 
            // findAgainToolStripMenuItem
            // 
            this.findAgainToolStripMenuItem.Name = "findAgainToolStripMenuItem";
            resources.ApplyResources(this.findAgainToolStripMenuItem, "findAgainToolStripMenuItem");
            this.findAgainToolStripMenuItem.Click += new System.EventHandler(this.FindAgainClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // _cutMenu
            // 
            this._cutMenu.Name = "_cutMenu";
            resources.ApplyResources(this._cutMenu, "_cutMenu");
            this._cutMenu.Click += new System.EventHandler(this.CutMenuClick);
            // 
            // _copyMenu
            // 
            this._copyMenu.Name = "_copyMenu";
            resources.ApplyResources(this._copyMenu, "_copyMenu");
            this._copyMenu.Click += new System.EventHandler(this.CopyMenuClick);
            // 
            // _pasteMenu
            // 
            this._pasteMenu.Name = "_pasteMenu";
            resources.ApplyResources(this._pasteMenu, "_pasteMenu");
            this._pasteMenu.Click += new System.EventHandler(this.PasteMenuClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // encodeDecodeToolStripMenuItem
            // 
            this.encodeDecodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.urlEncodeToolStripMenuItem,
            this.urlEncodeEverythingToolStripMenuItem,
            this.urlDecodeToolStripMenuItem,
            this.htmlEncodeToolStripMenuItem,
            this.htmlDecodeToolStripMenuItem,
            this.base64EncodeToolStripMenuItem,
            this.base64DecodeToolStripMenuItem,
            this.aSCIIEncodeToolStripMenuItem,
            this.aSCIIDecodeToolStripMenuItem});
            this.encodeDecodeToolStripMenuItem.Name = "encodeDecodeToolStripMenuItem";
            resources.ApplyResources(this.encodeDecodeToolStripMenuItem, "encodeDecodeToolStripMenuItem");
            // 
            // urlEncodeToolStripMenuItem
            // 
            this.urlEncodeToolStripMenuItem.Name = "urlEncodeToolStripMenuItem";
            resources.ApplyResources(this.urlEncodeToolStripMenuItem, "urlEncodeToolStripMenuItem");
            this.urlEncodeToolStripMenuItem.Click += new System.EventHandler(this.urlEncodeToolStripMenuItem_Click);
            // 
            // urlEncodeEverythingToolStripMenuItem
            // 
            this.urlEncodeEverythingToolStripMenuItem.Name = "urlEncodeEverythingToolStripMenuItem";
            resources.ApplyResources(this.urlEncodeEverythingToolStripMenuItem, "urlEncodeEverythingToolStripMenuItem");
            this.urlEncodeEverythingToolStripMenuItem.Click += new System.EventHandler(this.urlEncodeEverythingToolStripMenuItem_Click);
            // 
            // urlDecodeToolStripMenuItem
            // 
            this.urlDecodeToolStripMenuItem.Name = "urlDecodeToolStripMenuItem";
            resources.ApplyResources(this.urlDecodeToolStripMenuItem, "urlDecodeToolStripMenuItem");
            this.urlDecodeToolStripMenuItem.Click += new System.EventHandler(this.urlDecodeToolStripMenuItem_Click);
            // 
            // htmlEncodeToolStripMenuItem
            // 
            this.htmlEncodeToolStripMenuItem.Name = "htmlEncodeToolStripMenuItem";
            resources.ApplyResources(this.htmlEncodeToolStripMenuItem, "htmlEncodeToolStripMenuItem");
            this.htmlEncodeToolStripMenuItem.Click += new System.EventHandler(this.htmlEncodeToolStripMenuItem_Click);
            // 
            // htmlDecodeToolStripMenuItem
            // 
            this.htmlDecodeToolStripMenuItem.Name = "htmlDecodeToolStripMenuItem";
            resources.ApplyResources(this.htmlDecodeToolStripMenuItem, "htmlDecodeToolStripMenuItem");
            this.htmlDecodeToolStripMenuItem.Click += new System.EventHandler(this.htmlDecodeToolStripMenuItem_Click);
            // 
            // base64EncodeToolStripMenuItem
            // 
            this.base64EncodeToolStripMenuItem.Name = "base64EncodeToolStripMenuItem";
            resources.ApplyResources(this.base64EncodeToolStripMenuItem, "base64EncodeToolStripMenuItem");
            this.base64EncodeToolStripMenuItem.Click += new System.EventHandler(this.base64EncodeToolStripMenuItem_Click);
            // 
            // base64DecodeToolStripMenuItem
            // 
            this.base64DecodeToolStripMenuItem.Name = "base64DecodeToolStripMenuItem";
            resources.ApplyResources(this.base64DecodeToolStripMenuItem, "base64DecodeToolStripMenuItem");
            this.base64DecodeToolStripMenuItem.Click += new System.EventHandler(this.base64DecodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // countToolStripMenuItem
            // 
            this.countToolStripMenuItem.Name = "countToolStripMenuItem";
            resources.ApplyResources(this.countToolStripMenuItem, "countToolStripMenuItem");
            this.countToolStripMenuItem.Click += new System.EventHandler(this.countToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // formatXMLToolStripMenuItem
            // 
            this.formatXMLToolStripMenuItem.Name = "formatXMLToolStripMenuItem";
            resources.ApplyResources(this.formatXMLToolStripMenuItem, "formatXMLToolStripMenuItem");
            this.formatXMLToolStripMenuItem.Click += new System.EventHandler(this.FormatXMLClick);
            // 
            // testXXEProcessingToolStripMenuItem
            // 
            this.testXXEProcessingToolStripMenuItem.Name = "testXXEProcessingToolStripMenuItem";
            resources.ApplyResources(this.testXXEProcessingToolStripMenuItem, "testXXEProcessingToolStripMenuItem");
            this.testXXEProcessingToolStripMenuItem.Click += new System.EventHandler(this.testXXEProcessingToolStripMenuItem_Click);
            // 
            // insertXXETestToolStripMenuItem
            // 
            this.insertXXETestToolStripMenuItem.Name = "insertXXETestToolStripMenuItem";
            resources.ApplyResources(this.insertXXETestToolStripMenuItem, "insertXXETestToolStripMenuItem");
            this.insertXXETestToolStripMenuItem.Click += new System.EventHandler(this.insertXXETestToolStripMenuItem_Click);
            // 
            // insertSequenceVariableToolStripMenuItem
            // 
            this.insertSequenceVariableToolStripMenuItem.Name = "insertSequenceVariableToolStripMenuItem";
            resources.ApplyResources(this.insertSequenceVariableToolStripMenuItem, "insertSequenceVariableToolStripMenuItem");
            this.insertSequenceVariableToolStripMenuItem.Click += new System.EventHandler(this.insertSequenceVariableToolStripMenuItem_Click);
            // 
            // insertFuzzStringToolStripMenuItem
            // 
            this.insertFuzzStringToolStripMenuItem.Name = "insertFuzzStringToolStripMenuItem";
            resources.ApplyResources(this.insertFuzzStringToolStripMenuItem, "insertFuzzStringToolStripMenuItem");
            this.insertFuzzStringToolStripMenuItem.Click += new System.EventHandler(this.insertFuzzStringToolStripMenuItem_Click);
            // 
            // aSCIIEncodeToolStripMenuItem
            // 
            this.aSCIIEncodeToolStripMenuItem.Name = "aSCIIEncodeToolStripMenuItem";
            resources.ApplyResources(this.aSCIIEncodeToolStripMenuItem, "aSCIIEncodeToolStripMenuItem");
            this.aSCIIEncodeToolStripMenuItem.Click += new System.EventHandler(this.aSCIIEncodeToolStripMenuItem_Click);
            // 
            // aSCIIDecodeToolStripMenuItem
            // 
            this.aSCIIDecodeToolStripMenuItem.Name = "aSCIIDecodeToolStripMenuItem";
            resources.ApplyResources(this.aSCIIDecodeToolStripMenuItem, "aSCIIDecodeToolStripMenuItem");
            this.aSCIIDecodeToolStripMenuItem.Click += new System.EventHandler(this.aSCIIDecodeToolStripMenuItem_Click);
            // 
            // TrafficTextBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this._textBox);
            resources.ApplyResources(this, "$this");
            this.Name = "TrafficTextBox";
            this._contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.RichTextBox _textBox;
		protected System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.Windows.Forms.ToolStripMenuItem _wrapMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem _cutMenu;
		private System.Windows.Forms.ToolStripMenuItem _copyMenu;
		private System.Windows.Forms.ToolStripMenuItem _pasteMenu;
		private System.Windows.Forms.ToolStripMenuItem _findMenu;
		private System.Windows.Forms.ToolStripMenuItem findAgainToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem countToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem formatXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertSequenceVariableToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertFuzzStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testXXEProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertXXETestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodeDecodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem urlEncodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem urlEncodeEverythingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem urlDecodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem htmlEncodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem htmlDecodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem base64EncodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem base64DecodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIEncodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIDecodeToolStripMenuItem;
	}
}
