namespace TrafficViewerControls
{
    partial class TVMenuStrip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TVMenuStrip));
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._file = new System.Windows.Forms.ToolStripMenuItem();
            this._new = new System.Windows.Forms.ToolStripMenuItem();
            this._importLog = new System.Windows.Forms.ToolStripMenuItem();
            this._open = new System.Windows.Forms.ToolStripMenuItem();
            this._openUnpacked = new System.Windows.Forms.ToolStripMenuItem();
            this._export = new System.Windows.Forms.ToolStripMenuItem();
            this._save = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAs = new System.Windows.Forms.ToolStripMenuItem();
            this._exploitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._tools = new System.Windows.Forms.ToolStripMenuItem();
            this._useEmbeddedBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.trafficToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._visualizeTraffic = new System.Windows.Forms.ToolStripMenuItem();
            this._analysisModulesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._tail = new System.Windows.Forms.ToolStripMenuItem();
            this._stopTail = new System.Windows.Forms.ToolStripMenuItem();
            this._clear = new System.Windows.Forms.ToolStripMenuItem();
            this._stopLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._editCurrentProfile = new System.Windows.Forms.ToolStripMenuItem();
            this._options = new System.Windows.Forms.ToolStripMenuItem();
            this._search = new System.Windows.Forms.ToolStripMenuItem();
            this._menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _menuStrip
            // 
            resources.ApplyResources(this._menuStrip, "_menuStrip");
            this._menuStrip.BackColor = System.Drawing.Color.Black;
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._file,
            this._exploitMenu,
            this._tools,
            this.toolStripMenuItem1,
            this._search});
            this._menuStrip.Name = "_menuStrip";
            // 
            // _file
            // 
            this._file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._new,
            this._importLog,
            this._open,
            this._openUnpacked,
            this._export,
            this._save,
            this._saveAs});
            resources.ApplyResources(this._file, "_file");
            this._file.ForeColor = System.Drawing.Color.LightGray;
            this._file.Name = "_file";
            // 
            // _new
            // 
            this._new.Name = "_new";
            resources.ApplyResources(this._new, "_new");
            // 
            // _importLog
            // 
            this._importLog.Name = "_importLog";
            resources.ApplyResources(this._importLog, "_importLog");
            // 
            // _open
            // 
            this._open.Name = "_open";
            resources.ApplyResources(this._open, "_open");
            // 
            // _openUnpacked
            // 
            this._openUnpacked.Name = "_openUnpacked";
            resources.ApplyResources(this._openUnpacked, "_openUnpacked");
            // 
            // _export
            // 
            this._export.Name = "_export";
            resources.ApplyResources(this._export, "_export");
            // 
            // _save
            // 
            this._save.Name = "_save";
            resources.ApplyResources(this._save, "_save");
            // 
            // _saveAs
            // 
            this._saveAs.Name = "_saveAs";
            resources.ApplyResources(this._saveAs, "_saveAs");
            // 
            // _exploitMenu
            // 
            resources.ApplyResources(this._exploitMenu, "_exploitMenu");
            this._exploitMenu.ForeColor = System.Drawing.Color.LightGray;
            this._exploitMenu.Name = "_exploitMenu";
            // 
            // _tools
            // 
            this._tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._useEmbeddedBrowser,
            this.trafficToolsToolStripMenuItem,
            this.toolStripSeparator1,
            this._tail,
            this._stopTail,
            this._clear,
            this._stopLoad});
            resources.ApplyResources(this._tools, "_tools");
            this._tools.ForeColor = System.Drawing.Color.LightGray;
            this._tools.Name = "_tools";
            // 
            // _useEmbeddedBrowser
            // 
            this._useEmbeddedBrowser.Name = "_useEmbeddedBrowser";
            resources.ApplyResources(this._useEmbeddedBrowser, "_useEmbeddedBrowser");
            // 
            // trafficToolsToolStripMenuItem
            // 
            this.trafficToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._visualizeTraffic,
            this._analysisModulesMenu});
            this.trafficToolsToolStripMenuItem.Name = "trafficToolsToolStripMenuItem";
            resources.ApplyResources(this.trafficToolsToolStripMenuItem, "trafficToolsToolStripMenuItem");
            // 
            // _visualizeTraffic
            // 
            this._visualizeTraffic.Name = "_visualizeTraffic";
            resources.ApplyResources(this._visualizeTraffic, "_visualizeTraffic");
            // 
            // _analysisModulesMenu
            // 
            this._analysisModulesMenu.Name = "_analysisModulesMenu";
            resources.ApplyResources(this._analysisModulesMenu, "_analysisModulesMenu");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // _tail
            // 
            resources.ApplyResources(this._tail, "_tail");
            this._tail.ForeColor = System.Drawing.Color.Lime;
            this._tail.Image = global::TrafficViewerControls.Properties.Resources.tail;
            this._tail.Name = "_tail";
            // 
            // _stopTail
            // 
            resources.ApplyResources(this._stopTail, "_stopTail");
            this._stopTail.Image = global::TrafficViewerControls.Properties.Resources.untail;
            this._stopTail.Name = "_stopTail";
            // 
            // _clear
            // 
            this._clear.Name = "_clear";
            resources.ApplyResources(this._clear, "_clear");
            // 
            // _stopLoad
            // 
            this._stopLoad.Name = "_stopLoad";
            resources.ApplyResources(this._stopLoad, "_stopLoad");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._editCurrentProfile,
            this._options});
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.LightGray;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // _editCurrentProfile
            // 
            this._editCurrentProfile.Name = "_editCurrentProfile";
            resources.ApplyResources(this._editCurrentProfile, "_editCurrentProfile");
            // 
            // _options
            // 
            this._options.Name = "_options";
            resources.ApplyResources(this._options, "_options");
            // 
            // _search
            // 
            resources.ApplyResources(this._search, "_search");
            this._search.ForeColor = System.Drawing.Color.LightGray;
            this._search.Name = "_search";
            // 
            // TVMenuStrip
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._menuStrip);
            this.Name = "TVMenuStrip";
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem _file;
        private System.Windows.Forms.ToolStripMenuItem _open;
		private System.Windows.Forms.ToolStripMenuItem _importLog;
        private System.Windows.Forms.ToolStripMenuItem _tools;
		private System.Windows.Forms.ToolStripMenuItem _search;
        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _save;
        private System.Windows.Forms.ToolStripMenuItem _saveAs;
		private System.Windows.Forms.ToolStripMenuItem _openUnpacked;
		private System.Windows.Forms.ToolStripMenuItem _export;
        private System.Windows.Forms.ToolStripMenuItem _new;
		private System.Windows.Forms.ToolStripMenuItem _exploitMenu;
		private System.Windows.Forms.ToolStripMenuItem trafficToolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _visualizeTraffic;
		private System.Windows.Forms.ToolStripMenuItem _analysisModulesMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem _tail;
		private System.Windows.Forms.ToolStripMenuItem _stopTail;
		private System.Windows.Forms.ToolStripMenuItem _clear;
        private System.Windows.Forms.ToolStripMenuItem _stopLoad;
        private System.Windows.Forms.ToolStripMenuItem _useEmbeddedBrowser;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem _editCurrentProfile;
        private System.Windows.Forms.ToolStripMenuItem _options;
    }
}
