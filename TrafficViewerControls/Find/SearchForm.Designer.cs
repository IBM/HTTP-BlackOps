namespace TrafficViewerControls
{
	partial class SearchForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this._timer = new System.Windows.Forms.Timer(this.components);
            this._searchWorker = new System.ComponentModel.BackgroundWorker();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._list = new System.Windows.Forms.ListBox();
            this._buttonReplaceAll = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._dropType = new System.Windows.Forms.ComboBox();
            this._buttonReplaceOnce = new System.Windows.Forms.Button();
            this._boxReplace = new System.Windows.Forms.TextBox();
            this._boxDescriptionFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._checkIsRegex = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._boxSearchText = new System.Windows.Forms.TextBox();
            this._buttonSearch = new System.Windows.Forms.Button();
            this._buttonCopy = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _timer
            // 
            this._timer.Interval = 50;
            this._timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // _searchWorker
            // 
            this._searchWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SearchWorkerDoWork);
            this._searchWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SearchWorkerCompleted);
            // 
            // _statusStrip
            // 
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._progressBar,
            this.toolStripStatusLabel1});
            resources.ApplyResources(this._statusStrip, "_statusStrip");
            this._statusStrip.Name = "_statusStrip";
            // 
            // _progressBar
            // 
            this._progressBar.Name = "_progressBar";
            resources.ApplyResources(this._progressBar, "_progressBar");
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.IsLink = true;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // _list
            // 
            resources.ApplyResources(this._list, "_list");
            this._list.FormattingEnabled = true;
            this._list.Name = "_list";
            this._list.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // _buttonReplaceAll
            // 
            resources.ApplyResources(this._buttonReplaceAll, "_buttonReplaceAll");
            this._buttonReplaceAll.Name = "_buttonReplaceAll";
            this._buttonReplaceAll.UseVisualStyleBackColor = true;
            this._buttonReplaceAll.Click += new System.EventHandler(this.ReplaceAllClick);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // _dropType
            // 
            resources.ApplyResources(this._dropType, "_dropType");
            this._dropType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._dropType.FormattingEnabled = true;
            this._dropType.Items.AddRange(new object[] {
            resources.GetString("_dropType.Items"),
            resources.GetString("_dropType.Items1"),
            resources.GetString("_dropType.Items2"),
            resources.GetString("_dropType.Items3"),
            resources.GetString("_dropType.Items4")});
            this._dropType.Name = "_dropType";
            // 
            // _buttonReplaceOnce
            // 
            resources.ApplyResources(this._buttonReplaceOnce, "_buttonReplaceOnce");
            this._buttonReplaceOnce.Name = "_buttonReplaceOnce";
            this._buttonReplaceOnce.UseVisualStyleBackColor = true;
            this._buttonReplaceOnce.Click += new System.EventHandler(this.ReplaceOnceClick);
            // 
            // _boxReplace
            // 
            resources.ApplyResources(this._boxReplace, "_boxReplace");
            this._boxReplace.Name = "_boxReplace";
            // 
            // _boxDescriptionFilter
            // 
            resources.ApplyResources(this._boxDescriptionFilter, "_boxDescriptionFilter");
            this._boxDescriptionFilter.Name = "_boxDescriptionFilter";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // _checkIsRegex
            // 
            resources.ApplyResources(this._checkIsRegex, "_checkIsRegex");
            this._checkIsRegex.Name = "_checkIsRegex";
            this._checkIsRegex.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // _boxSearchText
            // 
            resources.ApplyResources(this._boxSearchText, "_boxSearchText");
            this._boxSearchText.Name = "_boxSearchText";
            this.toolTip1.SetToolTip(this._boxSearchText, resources.GetString("_boxSearchText.ToolTip"));
            // 
            // _buttonSearch
            // 
            resources.ApplyResources(this._buttonSearch, "_buttonSearch");
            this._buttonSearch.Name = "_buttonSearch";
            this._buttonSearch.UseVisualStyleBackColor = true;
            this._buttonSearch.Click += new System.EventHandler(this.SearchClick);
            // 
            // _buttonCopy
            // 
            this._buttonCopy.Image = global::TrafficViewerControls.Properties.Resources.ShowAllFiles_349;
            resources.ApplyResources(this._buttonCopy, "_buttonCopy");
            this._buttonCopy.Name = "_buttonCopy";
            this.toolTip1.SetToolTip(this._buttonCopy, resources.GetString("_buttonCopy.ToolTip"));
            this._buttonCopy.UseVisualStyleBackColor = true;
            this._buttonCopy.Click += new System.EventHandler(this.ButtonCopyClick);
            // 
            // SearchForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._buttonCopy);
            this.Controls.Add(this._list);
            this.Controls.Add(this._buttonReplaceAll);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._dropType);
            this.Controls.Add(this._buttonReplaceOnce);
            this.Controls.Add(this._boxReplace);
            this.Controls.Add(this._boxDescriptionFilter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._checkIsRegex);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._boxSearchText);
            this.Controls.Add(this._buttonSearch);
            this.Controls.Add(this._statusStrip);
            this.Name = "SearchForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchForm_FormClosing);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer _timer;
		private System.ComponentModel.BackgroundWorker _searchWorker;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStripProgressBar _progressBar;
		private System.Windows.Forms.ListBox _list;
		private System.Windows.Forms.Button _buttonReplaceAll;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox _dropType;
		private System.Windows.Forms.Button _buttonReplaceOnce;
		private System.Windows.Forms.TextBox _boxReplace;
		private System.Windows.Forms.TextBox _boxDescriptionFilter;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox _checkIsRegex;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _boxSearchText;
		private System.Windows.Forms.Button _buttonSearch;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button _buttonCopy;
        private System.Windows.Forms.ToolTip toolTip1;
	}
}