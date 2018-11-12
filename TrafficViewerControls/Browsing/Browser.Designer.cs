namespace TrafficViewerControls.Browsing
{
	partial class Browser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Browser));
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.AddressBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.TrapMenu = new System.Windows.Forms.ToolStripSplitButton();
            this._trapRequestsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._trapResponsesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this._webBrowser = new TrafficViewerControls.Browsing.WebBrowserEx();
            this.ToolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStrip
            // 
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripLabel1,
            this.AddressBox,
            this.toolStripButton5,
            this.TrapMenu});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(896, 25);
            this.ToolStrip.TabIndex = 2;
            this.ToolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Back";
            this.toolStripButton1.Click += new System.EventHandler(this.BackClick);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Forward";
            this.toolStripButton2.Click += new System.EventHandler(this.ForwardClick);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel1.Text = "Address:";
            // 
            // AddressBox
            // 
            this.AddressBox.AcceptsReturn = true;
            this.AddressBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.AddressBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.AddressBox.Name = "AddressBox";
            this.AddressBox.Size = new System.Drawing.Size(450, 25);
            this.AddressBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddressBoxKeyDown);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "Reload";
            this.toolStripButton5.Click += new System.EventHandler(this.RefreshClick);
            // 
            // TrapMenu
            // 
            this.TrapMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TrapMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._trapRequestsMenu,
            this._trapResponsesMenu});
            this.TrapMenu.Image = ((System.Drawing.Image)(resources.GetObject("TrapMenu.Image")));
            this.TrapMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TrapMenu.Name = "TrapMenu";
            this.TrapMenu.Size = new System.Drawing.Size(47, 22);
            this.TrapMenu.Text = "Trap";
            this.TrapMenu.Visible = false;
            // 
            // _trapRequestsMenu
            // 
            this._trapRequestsMenu.Name = "_trapRequestsMenu";
            this._trapRequestsMenu.Size = new System.Drawing.Size(129, 22);
            this._trapRequestsMenu.Text = "Requests";
            this._trapRequestsMenu.Click += new System.EventHandler(this.TrapRequestsClick);
            // 
            // _trapResponsesMenu
            // 
            this._trapResponsesMenu.Name = "_trapResponsesMenu";
            this._trapResponsesMenu.Size = new System.Drawing.Size(129, 22);
            this._trapResponsesMenu.Text = "Responses";
            this._trapResponsesMenu.Click += new System.EventHandler(this.TrapResponseClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 498);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(896, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _progressBar
            // 
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // _webBrowser
            // 
            this._webBrowser.AllowWebBrowserDrop = false;
            this._webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._webBrowser.Location = new System.Drawing.Point(2, 28);
            this._webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.SetUIHandler = false;
            this._webBrowser.Size = new System.Drawing.Size(892, 492);
            this._webBrowser.TabIndex = 1;
            this._webBrowser.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // Browser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 520);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this._webBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Browser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browser";
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private WebBrowserEx _webBrowser;

		public WebBrowserEx WebBrowser
		{
			get { return _webBrowser; }
		}
		protected System.Windows.Forms.ToolStripTextBox AddressBox;
		protected System.Windows.Forms.ToolStrip ToolStrip;
		protected System.Windows.Forms.ToolStripSplitButton TrapMenu;

		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripButton toolStripButton5;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar _progressBar;
		
		private System.Windows.Forms.ToolStripMenuItem _trapRequestsMenu;
		private System.Windows.Forms.ToolStripMenuItem _trapResponsesMenu;
	}
}