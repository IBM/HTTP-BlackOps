namespace TrafficViewerControls
{
	partial class RequestViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestViewer));
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabHttpTraffic = new System.Windows.Forms.TabPage();
            this._pageTrafficView = new TrafficViewerControls.RequestTrafficView();
            this._tabEntities = new System.Windows.Forms.TabPage();
            this._pageEntitiesView = new TrafficViewerControls.EntitiesView();
            this._tabBrowser = new System.Windows.Forms.TabPage();
            this._pageBrowserView = new TrafficViewerControls.BrowserView();
            this._tabDom = new System.Windows.Forms.TabPage();
            this._domView = new TrafficViewerControls.RequestDomView();
            this._tabLogSync = new System.Windows.Forms.TabPage();
            this._pageLogSyncView = new TrafficViewerControls.RequestLogSyncView();
            this._tabControl.SuspendLayout();
            this._tabHttpTraffic.SuspendLayout();
            this._tabEntities.SuspendLayout();
            this._tabBrowser.SuspendLayout();
            this._tabDom.SuspendLayout();
            this._tabLogSync.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tabControl
            // 
            resources.ApplyResources(this._tabControl, "_tabControl");
            this._tabControl.Controls.Add(this._tabHttpTraffic);
            this._tabControl.Controls.Add(this._tabEntities);
            this._tabControl.Controls.Add(this._tabBrowser);
            this._tabControl.Controls.Add(this._tabDom);
            this._tabControl.Controls.Add(this._tabLogSync);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            // 
            // _tabHttpTraffic
            // 
            this._tabHttpTraffic.BackColor = System.Drawing.Color.Black;
            this._tabHttpTraffic.Controls.Add(this._pageTrafficView);
            this._tabHttpTraffic.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this._tabHttpTraffic, "_tabHttpTraffic");
            this._tabHttpTraffic.Name = "_tabHttpTraffic";
            // 
            // _pageTrafficView
            // 
            resources.ApplyResources(this._pageTrafficView, "_pageTrafficView");
            this._pageTrafficView.BackColor = System.Drawing.Color.Black;
            this._pageTrafficView.Name = "_pageTrafficView";
            this._pageTrafficView.RequestText = "";
            this._pageTrafficView.ResponseText = "";
            // 
            // _tabEntities
            // 
            this._tabEntities.Controls.Add(this._pageEntitiesView);
            resources.ApplyResources(this._tabEntities, "_tabEntities");
            this._tabEntities.Name = "_tabEntities";
            this._tabEntities.UseVisualStyleBackColor = true;
            // 
            // _pageEntitiesView
            // 
            resources.ApplyResources(this._pageEntitiesView, "_pageEntitiesView");
            this._pageEntitiesView.BackColor = System.Drawing.Color.Black;
            this._pageEntitiesView.Name = "_pageEntitiesView";
            // 
            // _tabBrowser
            // 
            this._tabBrowser.BackColor = System.Drawing.Color.Black;
            this._tabBrowser.Controls.Add(this._pageBrowserView);
            this._tabBrowser.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this._tabBrowser, "_tabBrowser");
            this._tabBrowser.Name = "_tabBrowser";
            // 
            // _pageBrowserView
            // 
            resources.ApplyResources(this._pageBrowserView, "_pageBrowserView");
            this._pageBrowserView.Name = "_pageBrowserView";
            // 
            // _tabDom
            // 
            this._tabDom.BackColor = System.Drawing.Color.Black;
            this._tabDom.Controls.Add(this._domView);
            this._tabDom.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this._tabDom, "_tabDom");
            this._tabDom.Name = "_tabDom";
            // 
            // _domView
            // 
            resources.ApplyResources(this._domView, "_domView");
            this._domView.Name = "_domView";
            // 
            // _tabLogSync
            // 
            this._tabLogSync.BackColor = System.Drawing.Color.Black;
            this._tabLogSync.Controls.Add(this._pageLogSyncView);
            this._tabLogSync.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this._tabLogSync, "_tabLogSync");
            this._tabLogSync.Name = "_tabLogSync";
            // 
            // _pageLogSyncView
            // 
            this._pageLogSyncView.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this._pageLogSyncView, "_pageLogSyncView");
            this._pageLogSyncView.EventTime = new System.DateTime(((long)(0)));
            this._pageLogSyncView.Name = "_pageLogSyncView";
            // 
            // RequestViewer
            // 
            this.Controls.Add(this._tabControl);
            this.Name = "RequestViewer";
            resources.ApplyResources(this, "$this");
            this._tabControl.ResumeLayout(false);
            this._tabHttpTraffic.ResumeLayout(false);
            this._tabEntities.ResumeLayout(false);
            this._tabBrowser.ResumeLayout(false);
            this._tabDom.ResumeLayout(false);
            this._tabLogSync.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _tabHttpTraffic;
		private System.Windows.Forms.TabPage _tabBrowser;
		private System.Windows.Forms.TabPage _tabLogSync;
		private RequestTrafficView _pageTrafficView;
		private BrowserView _pageBrowserView;
		private RequestLogSyncView _pageLogSyncView;
		private System.Windows.Forms.TabPage _tabDom;
		private RequestDomView _domView;
        private System.Windows.Forms.TabPage _tabEntities;
        private EntitiesView _pageEntitiesView;

	}
}
