using TrafficViewerControls.Browsing;
namespace TrafficViewerControls
{
    partial class TrafficViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrafficViewerForm));
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            this._rightSplitContainer = new System.Windows.Forms.SplitContainer();
            this._dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            this._dialogSaveFile = new System.Windows.Forms.SaveFileDialog();
            this._dialogFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this._requestViewer = new TrafficViewerControls.RequestViewer();
            this._proxyConsole = new TrafficViewerControls.Browsing.ProxyConsole();
            this._requestsList = new TrafficViewerControls.RequestList.TVRequestsList();
            this._menuStrip = new TrafficViewerControls.TVMenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._rightSplitContainer)).BeginInit();
            this._rightSplitContainer.Panel1.SuspendLayout();
            this._rightSplitContainer.Panel2.SuspendLayout();
            this._rightSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _splitContainer
            // 
            resources.ApplyResources(this._splitContainer, "_splitContainer");
            this._splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.Controls.Add(this._requestViewer);
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.Controls.Add(this._rightSplitContainer);
            // 
            // _rightSplitContainer
            // 
            this._rightSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this._rightSplitContainer, "_rightSplitContainer");
            this._rightSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._rightSplitContainer.Name = "_rightSplitContainer";
            // 
            // _rightSplitContainer.Panel1
            // 
            this._rightSplitContainer.Panel1.Controls.Add(this._proxyConsole);
            // 
            // _rightSplitContainer.Panel2
            // 
            this._rightSplitContainer.Panel2.Controls.Add(this._requestsList);
            // 
            // _dialogOpenFile
            // 
            resources.ApplyResources(this._dialogOpenFile, "_dialogOpenFile");
            this._dialogOpenFile.RestoreDirectory = true;
            // 
            // _dialogSaveFile
            // 
            resources.ApplyResources(this._dialogSaveFile, "_dialogSaveFile");
            this._dialogSaveFile.RestoreDirectory = true;
            // 
            // _dialogFolderBrowser
            // 
            resources.ApplyResources(this._dialogFolderBrowser, "_dialogFolderBrowser");
            this._dialogFolderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // _requestViewer
            // 
            resources.ApplyResources(this._requestViewer, "_requestViewer");
            this._requestViewer.ForeColor = System.Drawing.Color.Black;
            this._requestViewer.LogSyncViewEvent = new System.DateTime(((long)(0)));
            this._requestViewer.Name = "_requestViewer";
            this._requestViewer.RequestText = "";
            this._requestViewer.ResponseText = "";
            // 
            // _proxyConsole
            // 
            resources.ApplyResources(this._proxyConsole, "_proxyConsole");
            this._proxyConsole.BackColor = System.Drawing.Color.Black;
            this._proxyConsole.ForeColor = System.Drawing.Color.White;
            this._proxyConsole.Name = "_proxyConsole";
            this._proxyConsole.Status = TrafficViewerSDK.Http.TVConsoleStatus.ServerStarted;
            // 
            // _requestsList
            // 
            this._requestsList.BackColor = System.Drawing.Color.Black;
            this._requestsList.DataSource = null;
            this._requestsList.DisableAutoScroll = false;
            resources.ApplyResources(this._requestsList, "_requestsList");
            this._requestsList.ForeColor = System.Drawing.Color.Black;
            this._requestsList.Name = "_requestsList";
            this._requestsList.RequestEventsQueueSize = 0;
            this._requestsList.StopLoadingEvents = false;
            this._requestsList.RequestSelected += new TrafficViewerSDK.TVDataAccessorDataEvent(this.RequestSelected);
            // 
            // _menuStrip
            // 
            resources.ApplyResources(this._menuStrip, "_menuStrip");
            this._menuStrip.BackColor = System.Drawing.SystemColors.Menu;
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.State = TrafficViewerControls.TVMenuStripStates.Loaded;
            this._menuStrip.OpenClick += new System.EventHandler(this.OpenClick);
            this._menuStrip.NewClick += new System.EventHandler(this.NewClick);
            this._menuStrip.ExportClick += new System.EventHandler(this.ExportClick);
            this._menuStrip.OpenUnpackedClick += new System.EventHandler(this.OpenUnpackedClick);
            this._menuStrip.ImportConnectToLog += new System.EventHandler(this.ImportLog);
            this._menuStrip.SaveClick += new System.EventHandler(this.SaveClick);
            this._menuStrip.SaveAsClick += new System.EventHandler(this.SaveAsClick);
            this._menuStrip.StopLoad += new System.EventHandler(this.StopLoadOperation);
            this._menuStrip.Clear += new System.EventHandler(this.Clear);
            this._menuStrip.Tail += new System.EventHandler(this.Tail);
            this._menuStrip.StopTail += new System.EventHandler(this.StopLoadOperation);
            this._menuStrip.Options += new System.EventHandler(this.OptionsClick);
            this._menuStrip.Search += new System.EventHandler(this.SearchClick);
            this._menuStrip.EmbeddedBrowserClicked += new System.EventHandler(this.EmbeddedBrowserClicked);
            this._menuStrip.CurrentProfileClicked += new System.EventHandler(this.CurrentProfileClicked);
            // 
            // TrafficViewerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this._splitContainer);
            this.Controls.Add(this._menuStrip);
            this.ForeColor = System.Drawing.Color.Lime;
            this.Name = "TrafficViewerForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrafficViewerMainFormClosing);
            this.Load += new System.EventHandler(this.TrafficViewerMainLoad);
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
            this._splitContainer.ResumeLayout(false);
            this._rightSplitContainer.Panel1.ResumeLayout(false);
            this._rightSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._rightSplitContainer)).EndInit();
            this._rightSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TrafficViewerControls.TVMenuStrip _menuStrip;
        private System.Windows.Forms.SplitContainer _splitContainer;
		private TrafficViewerControls.RequestViewer _requestViewer;
		private System.Windows.Forms.OpenFileDialog _dialogOpenFile;
		private System.Windows.Forms.SaveFileDialog _dialogSaveFile;
        private System.Windows.Forms.FolderBrowserDialog _dialogFolderBrowser;
        private System.Windows.Forms.SplitContainer _rightSplitContainer;
        private ProxyConsole _proxyConsole;
        private RequestList.TVRequestsList _requestsList;







    }
}

