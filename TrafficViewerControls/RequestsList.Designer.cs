namespace TrafficViewerControls
{
    partial class RequestsList
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestsList));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this._dataGrid = new System.Windows.Forms.DataGridView();
			this._id = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._requestLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._status = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._thread = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._description = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._reqTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._respTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._respSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._custom1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._custom2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._custom3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._highlightDescription = new System.Windows.Forms.ToolStripMenuItem();
			this._highlightSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._browse = new System.Windows.Forms.ToolStripMenuItem();
			this._httpLaunch = new System.Windows.Forms.ToolStripMenuItem();
			this._httpsLaunch = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.copyUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hTTPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hTTPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._copySelection = new System.Windows.Forms.ToolStripMenuItem();
			this._selectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this._deleteSelection = new System.Windows.Forms.ToolStripMenuItem();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this._stopLink = new System.Windows.Forms.ToolStripStatusLabel();
			this._colorDialog = new System.Windows.Forms.ColorDialog();
			this._folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this._progressTimer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this._dataGrid)).BeginInit();
			this._contextMenuStrip.SuspendLayout();
			this._statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _dataGrid
			// 
			this._dataGrid.AllowUserToAddRows = false;
			this._dataGrid.AllowUserToDeleteRows = false;
			this._dataGrid.AllowUserToOrderColumns = true;
			this._dataGrid.AllowUserToResizeRows = false;
			resources.ApplyResources(this._dataGrid, "_dataGrid");
			this._dataGrid.BackgroundColor = System.Drawing.SystemColors.Control;
			this._dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this._dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this._dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this._dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._id,
            this._requestLine,
            this._status,
            this._thread,
            this._description,
            this._reqTime,
            this._respTime,
            this._respSize,
            this._custom1,
            this._custom2,
            this._custom3});
			this._dataGrid.ContextMenuStrip = this._contextMenuStrip;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this._dataGrid.DefaultCellStyle = dataGridViewCellStyle2;
			this._dataGrid.Name = "_dataGrid";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this._dataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this._dataGrid.RowHeadersVisible = false;
			this._dataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this._dataGrid.RowTemplate.Height = 20;
			this._dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataGridMouseClick);
			this._dataGrid.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.ColumnDisplayIndexChanged);
			this._dataGrid.SelectionChanged += new System.EventHandler(this.DataGridSelectionChanged);
			// 
			// _id
			// 
			this._id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._id.FillWeight = 8F;
			resources.ApplyResources(this._id, "_id");
			this._id.Name = "_id";
			this._id.ReadOnly = true;
			// 
			// _requestLine
			// 
			this._requestLine.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._requestLine.FillWeight = 30F;
			resources.ApplyResources(this._requestLine, "_requestLine");
			this._requestLine.Name = "_requestLine";
			this._requestLine.ReadOnly = true;
			// 
			// _status
			// 
			this._status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._status.FillWeight = 7F;
			resources.ApplyResources(this._status, "_status");
			this._status.Name = "_status";
			this._status.ReadOnly = true;
			// 
			// _thread
			// 
			this._thread.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._thread.FillWeight = 14F;
			resources.ApplyResources(this._thread, "_thread");
			this._thread.Name = "_thread";
			this._thread.ReadOnly = true;
			// 
			// _description
			// 
			this._description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._description.FillWeight = 20F;
			resources.ApplyResources(this._description, "_description");
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			// 
			// _reqTime
			// 
			this._reqTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._reqTime.FillWeight = 15F;
			resources.ApplyResources(this._reqTime, "_reqTime");
			this._reqTime.Name = "_reqTime";
			this._reqTime.ReadOnly = true;
			// 
			// _respTime
			// 
			this._respTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._respTime.FillWeight = 15F;
			resources.ApplyResources(this._respTime, "_respTime");
			this._respTime.Name = "_respTime";
			this._respTime.ReadOnly = true;
			// 
			// _respSize
			// 
			this._respSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this._respSize.FillWeight = 10F;
			resources.ApplyResources(this._respSize, "_respSize");
			this._respSize.Name = "_respSize";
			this._respSize.ReadOnly = true;
			// 
			// _custom1
			// 
			this._custom1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this._custom1, "_custom1");
			this._custom1.Name = "_custom1";
			// 
			// _custom2
			// 
			this._custom2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this._custom2, "_custom2");
			this._custom2.Name = "_custom2";
			// 
			// _custom3
			// 
			this._custom3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this._custom3, "_custom3");
			this._custom3.Name = "_custom3";
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnsToolStripMenuItem,
            this._highlightDescription,
            this._highlightSelection,
            this.toolStripSeparator1,
            this._browse,
            this.toolStripSeparator2,
            this.copyUrlToolStripMenuItem,
            this._copySelection,
            this._selectAll,
            this.toolStripSeparator3,
            this._deleteSelection});
			this._contextMenuStrip.Name = "_contextMenuStrip";
			resources.ApplyResources(this._contextMenuStrip, "_contextMenuStrip");
			// 
			// columnsToolStripMenuItem
			// 
			this.columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
			resources.ApplyResources(this.columnsToolStripMenuItem, "columnsToolStripMenuItem");
			// 
			// _highlightDescription
			// 
			this._highlightDescription.Name = "_highlightDescription";
			resources.ApplyResources(this._highlightDescription, "_highlightDescription");
			this._highlightDescription.Click += new System.EventHandler(this.HighlightDescriptionClick);
			// 
			// _highlightSelection
			// 
			this._highlightSelection.Name = "_highlightSelection";
			resources.ApplyResources(this._highlightSelection, "_highlightSelection");
			this._highlightSelection.Click += new System.EventHandler(this.HighlightSelectionClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// _browse
			// 
			this._browse.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._httpLaunch,
            this._httpsLaunch});
			this._browse.Name = "_browse";
			resources.ApplyResources(this._browse, "_browse");
			this._browse.MouseEnter += new System.EventHandler(this.BrowseMenuOver);
			// 
			// _httpLaunch
			// 
			this._httpLaunch.Name = "_httpLaunch";
			resources.ApplyResources(this._httpLaunch, "_httpLaunch");
			this._httpLaunch.Click += new System.EventHandler(this.HttpLaunchClick);
			// 
			// _httpsLaunch
			// 
			this._httpsLaunch.Name = "_httpsLaunch";
			resources.ApplyResources(this._httpsLaunch, "_httpsLaunch");
			this._httpsLaunch.Click += new System.EventHandler(this.HttpsLaunchClick);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// copyUrlToolStripMenuItem
			// 
			this.copyUrlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hTTPToolStripMenuItem,
            this.hTTPSToolStripMenuItem});
			this.copyUrlToolStripMenuItem.Name = "copyUrlToolStripMenuItem";
			resources.ApplyResources(this.copyUrlToolStripMenuItem, "copyUrlToolStripMenuItem");
			// 
			// hTTPToolStripMenuItem
			// 
			this.hTTPToolStripMenuItem.Name = "hTTPToolStripMenuItem";
			resources.ApplyResources(this.hTTPToolStripMenuItem, "hTTPToolStripMenuItem");
			this.hTTPToolStripMenuItem.Click += new System.EventHandler(this.CopyUrlHttpClick);
			// 
			// hTTPSToolStripMenuItem
			// 
			this.hTTPSToolStripMenuItem.Name = "hTTPSToolStripMenuItem";
			resources.ApplyResources(this.hTTPSToolStripMenuItem, "hTTPSToolStripMenuItem");
			this.hTTPSToolStripMenuItem.Click += new System.EventHandler(this.CopyUrlHttpsClick);
			// 
			// _copySelection
			// 
			this._copySelection.Name = "_copySelection";
			resources.ApplyResources(this._copySelection, "_copySelection");
			this._copySelection.Click += new System.EventHandler(this.CopySelectionClick);
			// 
			// _selectAll
			// 
			this._selectAll.Name = "_selectAll";
			resources.ApplyResources(this._selectAll, "_selectAll");
			this._selectAll.Click += new System.EventHandler(this.SelectAllClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// _deleteSelection
			// 
			this._deleteSelection.Name = "_deleteSelection";
			resources.ApplyResources(this._deleteSelection, "_deleteSelection");
			this._deleteSelection.Click += new System.EventHandler(this.DeleteSelectionClick);
			// 
			// _statusStrip
			// 
			resources.ApplyResources(this._statusStrip, "_statusStrip");
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel,
            this._progressBar,
            this._stopLink});
			this._statusStrip.Name = "_statusStrip";
			// 
			// _statusLabel
			// 
			this._statusLabel.Name = "_statusLabel";
			resources.ApplyResources(this._statusLabel, "_statusLabel");
			// 
			// _progressBar
			// 
			this._progressBar.Name = "_progressBar";
			resources.ApplyResources(this._progressBar, "_progressBar");
			this._progressBar.Step = 25;
			// 
			// _stopLink
			// 
			resources.ApplyResources(this._stopLink, "_stopLink");
			this._stopLink.ForeColor = System.Drawing.Color.Blue;
			this._stopLink.IsLink = true;
			this._stopLink.Name = "_stopLink";
			// 
			// _progressTimer
			// 
			this._progressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
			// 
			// RequestsList
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._statusStrip);
			this.Controls.Add(this._dataGrid);
			this.Name = "RequestsList";
			((System.ComponentModel.ISupportInitialize)(this._dataGrid)).EndInit();
			this._contextMenuStrip.ResumeLayout(false);
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dataGrid;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
		private System.Windows.Forms.ToolStripProgressBar _progressBar;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem columnsToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel _stopLink;
		private System.Windows.Forms.ToolStripMenuItem _highlightDescription;
		private System.Windows.Forms.ToolStripMenuItem _highlightSelection;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem _browse;
		private System.Windows.Forms.ToolStripMenuItem _httpLaunch;
		private System.Windows.Forms.ToolStripMenuItem _httpsLaunch;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem _copySelection;
		private System.Windows.Forms.ToolStripMenuItem _selectAll;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem _deleteSelection;
		private System.Windows.Forms.ColorDialog _colorDialog;
		private System.Windows.Forms.FolderBrowserDialog _folderBrowser;
		private System.Windows.Forms.Timer _progressTimer;
		private System.Windows.Forms.DataGridViewTextBoxColumn _id;
		private System.Windows.Forms.DataGridViewTextBoxColumn _requestLine;
		private System.Windows.Forms.DataGridViewTextBoxColumn _status;
		private System.Windows.Forms.DataGridViewTextBoxColumn _thread;
		private System.Windows.Forms.DataGridViewTextBoxColumn _description;
		private System.Windows.Forms.DataGridViewTextBoxColumn _reqTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn _respTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn _respSize;
		private System.Windows.Forms.DataGridViewTextBoxColumn _custom1;
		private System.Windows.Forms.DataGridViewTextBoxColumn _custom2;
		private System.Windows.Forms.DataGridViewTextBoxColumn _custom3;
		private System.Windows.Forms.ToolStripMenuItem copyUrlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hTTPToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hTTPSToolStripMenuItem;
    }
}
