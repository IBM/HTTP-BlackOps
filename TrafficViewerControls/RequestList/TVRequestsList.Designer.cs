namespace TrafficViewerControls.RequestList
{
    partial class TVRequestsList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TVRequestsList));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this._dataGrid = new System.Windows.Forms.DataGridView();
            this._id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._host = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._requestLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._thread = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._reqTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._respTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._respSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._domId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._scheme = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._requestContext = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._refererId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._updatedPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._custom1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._custom2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._custom3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._highlightDescription = new System.Windows.Forms.ToolStripMenuItem();
            this.changeDescriptionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._highlightSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFlowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.domUniquenessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPagesWithSimilarFunctionalityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyPagesSimilarToTheSelectedRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.newRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replicateHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuResendRequests = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemSendTo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.countToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResponseBodyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setEncryptedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this._stopLink = new System.Windows.Forms.ToolStripStatusLabel();
            this._labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this._colorDialog = new System.Windows.Forms.ColorDialog();
            this._folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this._progressTimer = new System.Windows.Forms.Timer(this.components);
            this._label1 = new System.Windows.Forms.Label();
            this._reverseFilter = new System.Windows.Forms.CheckBox();
            this._textFilter = new System.Windows.Forms.TextBox();
            this._buttonClear = new System.Windows.Forms.Button();
            this._buttonDelete = new System.Windows.Forms.Button();
            this._addRequest = new System.Windows.Forms.Button();
            this.singleThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiThreadedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this._dataGrid.BackgroundColor = System.Drawing.Color.Black;
            this._dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._id,
            this._host,
            this._requestLine,
            this._status,
            this._thread,
            this._description,
            this._reqTime,
            this._respTime,
            this._duration,
            this._respSize,
            this._domId,
            this._scheme,
            this._requestContext,
            this._refererId,
            this._updatedPath,
            this._custom1,
            this._custom2,
            this._custom3});
            this._dataGrid.ContextMenuStrip = this._contextMenuStrip;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this._dataGrid.GridColor = System.Drawing.Color.Black;
            this._dataGrid.Name = "_dataGrid";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this._dataGrid.RowHeadersVisible = false;
            this._dataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dataGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this._dataGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Lime;
            this._dataGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Lime;
            this._dataGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this._dataGrid.RowTemplate.Height = 20;
            this._dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGrid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.CellBeginEdit);
            this._dataGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellUpdated);
            this._dataGrid.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.ColumnDisplayIndexChanged);
            this._dataGrid.SelectionChanged += new System.EventHandler(this.DataGridSelectionChanged);
            this._dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataGridMouseClick);
            // 
            // _id
            // 
            this._id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._id.FillWeight = 8F;
            resources.ApplyResources(this._id, "_id");
            this._id.Name = "_id";
            this._id.ReadOnly = true;
            // 
            // _host
            // 
            resources.ApplyResources(this._host, "_host");
            this._host.Name = "_host";
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
            // _duration
            // 
            this._duration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._duration.FillWeight = 10F;
            resources.ApplyResources(this._duration, "_duration");
            this._duration.Name = "_duration";
            // 
            // _respSize
            // 
            this._respSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._respSize.FillWeight = 10F;
            resources.ApplyResources(this._respSize, "_respSize");
            this._respSize.Name = "_respSize";
            this._respSize.ReadOnly = true;
            // 
            // _domId
            // 
            this._domId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._domId.FillWeight = 10F;
            resources.ApplyResources(this._domId, "_domId");
            this._domId.Name = "_domId";
            // 
            // _scheme
            // 
            this._scheme.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._scheme.FillWeight = 10F;
            resources.ApplyResources(this._scheme, "_scheme");
            this._scheme.Name = "_scheme";
            // 
            // _requestContext
            // 
            this._requestContext.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._requestContext.FillWeight = 30F;
            resources.ApplyResources(this._requestContext, "_requestContext");
            this._requestContext.Name = "_requestContext";
            // 
            // _refererId
            // 
            this._refererId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._refererId.FillWeight = 8F;
            resources.ApplyResources(this._refererId, "_refererId");
            this._refererId.Name = "_refererId";
            // 
            // _updatedPath
            // 
            this._updatedPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._updatedPath.FillWeight = 20F;
            resources.ApplyResources(this._updatedPath, "_updatedPath");
            this._updatedPath.Name = "_updatedPath";
            this._updatedPath.ReadOnly = true;
            // 
            // _custom1
            // 
            this._custom1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._custom1.FillWeight = 10F;
            resources.ApplyResources(this._custom1, "_custom1");
            this._custom1.Name = "_custom1";
            // 
            // _custom2
            // 
            this._custom2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._custom2.FillWeight = 10F;
            resources.ApplyResources(this._custom2, "_custom2");
            this._custom2.Name = "_custom2";
            // 
            // _custom3
            // 
            this._custom3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._custom3.FillWeight = 10F;
            resources.ApplyResources(this._custom3, "_custom3");
            this._custom3.Name = "_custom3";
            // 
            // _contextMenuStrip
            // 
            this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnsToolStripMenuItem,
            this._highlightDescription,
            this.changeDescriptionToolStripMenuItem1,
            this.toolStripMenuItem1,
            this._highlightSelection,
            this.selectFlowToolStripMenuItem,
            this.toolStripSeparator1,
            this._browse,
            this.toolStripSeparator2,
            this.copyUrlToolStripMenuItem,
            this._copySelection,
            this._selectAll,
            this.toolStripSeparator3,
            this._deleteSelection,
            this.toolStripSeparator4,
            this.domUniquenessToolStripMenuItem,
            this.toolStripSeparator6,
            this.compareToolStripMenuItem,
            this.toolStripSeparator5,
            this.showAllToolStripMenuItem,
            this.toolStripSeparator7,
            this.newRequestToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.replicateHeadersToolStripMenuItem,
            this._menuResendRequests,
            this._menuItemSendTo,
            this.toolStripSeparator8,
            this.countToolStripMenuItem,
            this.saveResponseBodyToolStripMenuItem,
            this.setEncryptedToolStripMenuItem});
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
            // changeDescriptionToolStripMenuItem1
            // 
            this.changeDescriptionToolStripMenuItem1.Name = "changeDescriptionToolStripMenuItem1";
            resources.ApplyResources(this.changeDescriptionToolStripMenuItem1, "changeDescriptionToolStripMenuItem1");
            this.changeDescriptionToolStripMenuItem1.Click += new System.EventHandler(this.ChangeDescriptionClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ChangeScheme);
            // 
            // _highlightSelection
            // 
            this._highlightSelection.Name = "_highlightSelection";
            resources.ApplyResources(this._highlightSelection, "_highlightSelection");
            this._highlightSelection.Click += new System.EventHandler(this.HighlightSelectionClick);
            // 
            // selectFlowToolStripMenuItem
            // 
            this.selectFlowToolStripMenuItem.Name = "selectFlowToolStripMenuItem";
            resources.ApplyResources(this.selectFlowToolStripMenuItem, "selectFlowToolStripMenuItem");
            this.selectFlowToolStripMenuItem.Click += new System.EventHandler(this.selectFlowToolStripMenuItem_Click);
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
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // domUniquenessToolStripMenuItem
            // 
            this.domUniquenessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showPagesWithSimilarFunctionalityToolStripMenuItem,
            this.showToolStripMenuItem,
            this.showOnlyPagesSimilarToTheSelectedRequestToolStripMenuItem});
            this.domUniquenessToolStripMenuItem.Name = "domUniquenessToolStripMenuItem";
            resources.ApplyResources(this.domUniquenessToolStripMenuItem, "domUniquenessToolStripMenuItem");
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // compareToolStripMenuItem
            // 
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            resources.ApplyResources(this.compareToolStripMenuItem, "compareToolStripMenuItem");
            this.compareToolStripMenuItem.Click += new System.EventHandler(this.CompareFirstTwoRequestsResponses);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // showAllToolStripMenuItem
            // 
            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
            resources.ApplyResources(this.showAllToolStripMenuItem, "showAllToolStripMenuItem");
            this.showAllToolStripMenuItem.Click += new System.EventHandler(this.ShowAllClick);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // newRequestToolStripMenuItem
            // 
            this.newRequestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawRequestToolStripMenuItem,
            this.fromURLToolStripMenuItem});
            this.newRequestToolStripMenuItem.Name = "newRequestToolStripMenuItem";
            resources.ApplyResources(this.newRequestToolStripMenuItem, "newRequestToolStripMenuItem");
            // 
            // rawRequestToolStripMenuItem
            // 
            this.rawRequestToolStripMenuItem.Name = "rawRequestToolStripMenuItem";
            resources.ApplyResources(this.rawRequestToolStripMenuItem, "rawRequestToolStripMenuItem");
            this.rawRequestToolStripMenuItem.Click += new System.EventHandler(this.NewRawRequestClick);
            // 
            // fromURLToolStripMenuItem
            // 
            this.fromURLToolStripMenuItem.Name = "fromURLToolStripMenuItem";
            resources.ApplyResources(this.fromURLToolStripMenuItem, "fromURLToolStripMenuItem");
            this.fromURLToolStripMenuItem.Click += new System.EventHandler(this.FromURLToolStripMenuItemClick);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            resources.ApplyResources(this.duplicateToolStripMenuItem, "duplicateToolStripMenuItem");
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateClick);
            // 
            // replicateHeadersToolStripMenuItem
            // 
            this.replicateHeadersToolStripMenuItem.Name = "replicateHeadersToolStripMenuItem";
            resources.ApplyResources(this.replicateHeadersToolStripMenuItem, "replicateHeadersToolStripMenuItem");
            this.replicateHeadersToolStripMenuItem.Click += new System.EventHandler(this.replicateHeadersToolStripMenuItem_Click);
            // 
            // _menuResendRequests
            // 
            this._menuResendRequests.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.singleThreadToolStripMenuItem,
            this.multiThreadedToolStripMenuItem});
            this._menuResendRequests.Name = "_menuResendRequests";
            resources.ApplyResources(this._menuResendRequests, "_menuResendRequests");

            // 
            // _menuItemSendTo
            // 
            this._menuItemSendTo.Name = "_menuItemSendTo";
            resources.ApplyResources(this._menuItemSendTo, "_menuItemSendTo");
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // countToolStripMenuItem
            // 
            this.countToolStripMenuItem.Name = "countToolStripMenuItem";
            resources.ApplyResources(this.countToolStripMenuItem, "countToolStripMenuItem");
            this.countToolStripMenuItem.Click += new System.EventHandler(this.CountSelection);
            // 
            // saveResponseBodyToolStripMenuItem
            // 
            this.saveResponseBodyToolStripMenuItem.Name = "saveResponseBodyToolStripMenuItem";
            resources.ApplyResources(this.saveResponseBodyToolStripMenuItem, "saveResponseBodyToolStripMenuItem");
            this.saveResponseBodyToolStripMenuItem.Click += new System.EventHandler(this.saveResponseBodyToolStripMenuItem_Click);
            // 
            // setEncryptedToolStripMenuItem
            // 
            this.setEncryptedToolStripMenuItem.Name = "setEncryptedToolStripMenuItem";
            resources.ApplyResources(this.setEncryptedToolStripMenuItem, "setEncryptedToolStripMenuItem");
            this.setEncryptedToolStripMenuItem.Click += new System.EventHandler(this.setEncryptedToolStripMenuItem_Click);
            // 
            // _statusStrip
            // 
            resources.ApplyResources(this._statusStrip, "_statusStrip");
            this._statusStrip.BackColor = System.Drawing.Color.White;
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel,
            this._progressBar,
            this._stopLink,
            this._labelStatus});
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
            // _labelStatus
            // 
            this._labelStatus.Name = "_labelStatus";
            resources.ApplyResources(this._labelStatus, "_labelStatus");
            // 
            // _progressTimer
            // 
            this._progressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
            // 
            // _label1
            // 
            resources.ApplyResources(this._label1, "_label1");
            this._label1.BackColor = System.Drawing.Color.Black;
            this._label1.ForeColor = System.Drawing.Color.White;
            this._label1.Name = "_label1";
            // 
            // _reverseFilter
            // 
            resources.ApplyResources(this._reverseFilter, "_reverseFilter");
            this._reverseFilter.BackColor = System.Drawing.Color.Black;
            this._reverseFilter.ForeColor = System.Drawing.Color.White;
            this._reverseFilter.Name = "_reverseFilter";
            this._reverseFilter.UseVisualStyleBackColor = false;
            this._reverseFilter.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // _textFilter
            // 
            resources.ApplyResources(this._textFilter, "_textFilter");
            this._textFilter.Name = "_textFilter";
            this._textFilter.TextChanged += new System.EventHandler(this.FilterChanged);
            this._textFilter.Enter += new System.EventHandler(this.FilterEnter);
            this._textFilter.Leave += new System.EventHandler(this.FilterLeave);
            // 
            // _buttonClear
            // 
            resources.ApplyResources(this._buttonClear, "_buttonClear");
            this._buttonClear.BackColor = System.Drawing.Color.Transparent;
            this._buttonClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._buttonClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this._buttonClear.ForeColor = System.Drawing.Color.White;
            this._buttonClear.Name = "_buttonClear";
            this._buttonClear.UseVisualStyleBackColor = false;
            this._buttonClear.Click += new System.EventHandler(this.ClearClick);
            // 
            // _buttonDelete
            // 
            resources.ApplyResources(this._buttonDelete, "_buttonDelete");
            this._buttonDelete.BackColor = System.Drawing.Color.Transparent;
            this._buttonDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._buttonDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this._buttonDelete.ForeColor = System.Drawing.Color.White;
            this._buttonDelete.Name = "_buttonDelete";
            this._buttonDelete.UseVisualStyleBackColor = false;
            this._buttonDelete.Click += new System.EventHandler(this.DeleteSelectionClick);
            // 
            // _addRequest
            // 
            resources.ApplyResources(this._addRequest, "_addRequest");
            this._addRequest.BackColor = System.Drawing.Color.Transparent;
            this._addRequest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._addRequest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this._addRequest.ForeColor = System.Drawing.Color.White;
            this._addRequest.Name = "_addRequest";
            this._addRequest.UseVisualStyleBackColor = false;
            this._addRequest.Click += new System.EventHandler(this.NewRawRequestClick);
            // 
            // singleThreadToolStripMenuItem
            // 
            this.singleThreadToolStripMenuItem.Name = "singleThreadToolStripMenuItem";
            resources.ApplyResources(this.singleThreadToolStripMenuItem, "singleThreadToolStripMenuItem");
            this.singleThreadToolStripMenuItem.Click += new System.EventHandler(this.ResendRequestsSingleThread);
            // 
            // multiThreadedToolStripMenuItem
            // 
            this.multiThreadedToolStripMenuItem.Name = "multiThreadedToolStripMenuItem";
            resources.ApplyResources(this.multiThreadedToolStripMenuItem, "multiThreadedToolStripMenuItem");
            this.multiThreadedToolStripMenuItem.Click += new System.EventHandler(this.ResendRequestsMultiThread);
            // 
            // TVRequestsList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._addRequest);
            this.Controls.Add(this._buttonDelete);
            this.Controls.Add(this._buttonClear);
            this.Controls.Add(this._textFilter);
            this.Controls.Add(this._label1);
            this.Controls.Add(this._reverseFilter);
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this._dataGrid);
            this.Name = "TVRequestsList";
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).EndInit();
            this._contextMenuStrip.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
		private System.Windows.Forms.ToolStripMenuItem copyUrlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hTTPToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hTTPSToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem domUniquenessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showPagesWithSimilarFunctionalityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showOnlyPagesSimilarToTheSelectedRequestToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel _labelStatus;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem compareToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem newRequestToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _menuResendRequests;
		private System.Windows.Forms.ToolStripMenuItem rawRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromURLToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _menuItemSendTo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem countToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeDescriptionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem replicateHeadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectFlowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveResponseBodyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.DataGridViewTextBoxColumn _id;
        private System.Windows.Forms.DataGridViewTextBoxColumn _host;
        private System.Windows.Forms.DataGridViewTextBoxColumn _requestLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn _status;
        private System.Windows.Forms.DataGridViewTextBoxColumn _thread;
        private System.Windows.Forms.DataGridViewTextBoxColumn _description;
        private System.Windows.Forms.DataGridViewTextBoxColumn _reqTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _respTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _duration;
        private System.Windows.Forms.DataGridViewTextBoxColumn _respSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn _domId;
        private System.Windows.Forms.DataGridViewTextBoxColumn _scheme;
        private System.Windows.Forms.DataGridViewTextBoxColumn _requestContext;
        private System.Windows.Forms.DataGridViewTextBoxColumn _refererId;
        private System.Windows.Forms.DataGridViewTextBoxColumn _updatedPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn _custom1;
        private System.Windows.Forms.DataGridViewTextBoxColumn _custom2;
        private System.Windows.Forms.DataGridViewTextBoxColumn _custom3;
        private System.Windows.Forms.Label _label1;
        private System.Windows.Forms.CheckBox _reverseFilter;
        private System.Windows.Forms.TextBox _textFilter;
        private System.Windows.Forms.Button _buttonClear;
        private System.Windows.Forms.Button _buttonDelete;
        private System.Windows.Forms.Button _addRequest;
        private System.Windows.Forms.ToolStripMenuItem setEncryptedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singleThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiThreadedToolStripMenuItem;
    }
}
