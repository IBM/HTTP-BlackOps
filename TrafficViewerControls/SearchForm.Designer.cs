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
			this._boxSearchText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._checkIsRegex = new System.Windows.Forms.CheckBox();
			this._buttonSearch = new System.Windows.Forms.Button();
			this._list = new System.Windows.Forms.ListBox();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.label2 = new System.Windows.Forms.Label();
			this._boxDescriptionFilter = new System.Windows.Forms.TextBox();
			this._dropType = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this._timer = new System.Windows.Forms.Timer(this.components);
			this._searchWorker = new System.ComponentModel.BackgroundWorker();
			this._statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _boxSearchText
			// 
			resources.ApplyResources(this._boxSearchText, "_boxSearchText");
			this._boxSearchText.Name = "_boxSearchText";
			this._boxSearchText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchTextKeyPress);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _checkIsRegex
			// 
			resources.ApplyResources(this._checkIsRegex, "_checkIsRegex");
			this._checkIsRegex.Name = "_checkIsRegex";
			this._checkIsRegex.UseVisualStyleBackColor = true;
			// 
			// _buttonSearch
			// 
			resources.ApplyResources(this._buttonSearch, "_buttonSearch");
			this._buttonSearch.Name = "_buttonSearch";
			this._buttonSearch.UseVisualStyleBackColor = true;
			this._buttonSearch.Click += new System.EventHandler(this.SearchClick);
			// 
			// _list
			// 
			resources.ApplyResources(this._list, "_list");
			this._list.FormattingEnabled = true;
			this._list.MinimumSize = new System.Drawing.Size(637, 238);
			this._list.Name = "_list";
			this._list.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._progressBar});
			resources.ApplyResources(this._statusStrip, "_statusStrip");
			this._statusStrip.Name = "_statusStrip";
			// 
			// _progressBar
			// 
			this._progressBar.Name = "_progressBar";
			resources.ApplyResources(this._progressBar, "_progressBar");
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// _boxDescriptionFilter
			// 
			resources.ApplyResources(this._boxDescriptionFilter, "_boxDescriptionFilter");
			this._boxDescriptionFilter.Name = "_boxDescriptionFilter";
			this._boxDescriptionFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DescriptionFilterKeyPress);
			// 
			// _dropType
			// 
			this._dropType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._dropType.FormattingEnabled = true;
			this._dropType.Items.AddRange(new object[] {
            resources.GetString("_dropType.Items"),
            resources.GetString("_dropType.Items1"),
            resources.GetString("_dropType.Items2"),
            resources.GetString("_dropType.Items3")});
			resources.ApplyResources(this._dropType, "_dropType");
			this._dropType.Name = "_dropType";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
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
			// SearchForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this._dropType);
			this.Controls.Add(this._boxDescriptionFilter);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._statusStrip);
			this.Controls.Add(this._list);
			this.Controls.Add(this._buttonSearch);
			this.Controls.Add(this._checkIsRegex);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._boxSearchText);
			this.Name = "SearchForm";
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _boxSearchText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox _checkIsRegex;
		private System.Windows.Forms.Button _buttonSearch;
		private System.Windows.Forms.ListBox _list;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _boxDescriptionFilter;
		private System.Windows.Forms.ToolStripProgressBar _progressBar;
		private System.Windows.Forms.ComboBox _dropType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Timer _timer;
		private System.ComponentModel.BackgroundWorker _searchWorker;
	}
}