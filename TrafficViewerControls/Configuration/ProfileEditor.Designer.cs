using CommonControls;
namespace TrafficViewerControls
{
	partial class ProfileEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileEditor));
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabExclusions = new System.Windows.Forms.TabPage();
            this._exclusions = new CommonControls.OptionsGrid();
            this._tabReplacements = new System.Windows.Forms.TabPage();
            this._responseReplacements = new CommonControls.OptionsGrid();
            this.label9 = new System.Windows.Forms.Label();
            this._requestReplacements = new CommonControls.OptionsGrid();
            this._tabPageTrackingPatterns = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this._trackingPatterns = new CommonControls.OptionsGrid();
            this._tabRegex = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this._boxTimeFormat = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this._boxNonHttp = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this._boxProxyConnection = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._boxResponseReceived = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._boxDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._boxEndThread = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._boxBeginThread = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._boxThreadId = new System.Windows.Forms.TextBox();
            this._tabCustom = new System.Windows.Forms.TabPage();
            this._customFields = new CommonControls.OptionsGrid();
            this._tabGUI = new System.Windows.Forms.TabPage();
            this._highlightingDefs = new CommonControls.OptionsGrid();
            this._buttonSave = new System.Windows.Forms.Button();
            this._buttonSaveAs = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._dialogSave = new System.Windows.Forms.SaveFileDialog();
            this._tabControl.SuspendLayout();
            this._tabExclusions.SuspendLayout();
            this._tabReplacements.SuspendLayout();
            this._tabPageTrackingPatterns.SuspendLayout();
            this._tabRegex.SuspendLayout();
            this._tabCustom.SuspendLayout();
            this._tabGUI.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tabControl
            // 
            resources.ApplyResources(this._tabControl, "_tabControl");
            this._tabControl.Controls.Add(this._tabExclusions);
            this._tabControl.Controls.Add(this._tabReplacements);
            this._tabControl.Controls.Add(this._tabPageTrackingPatterns);
            this._tabControl.Controls.Add(this._tabRegex);
            this._tabControl.Controls.Add(this._tabCustom);
            this._tabControl.Controls.Add(this._tabGUI);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            // 
            // _tabExclusions
            // 
            this._tabExclusions.Controls.Add(this._exclusions);
            resources.ApplyResources(this._tabExclusions, "_tabExclusions");
            this._tabExclusions.Name = "_tabExclusions";
            this._tabExclusions.UseVisualStyleBackColor = true;
            // 
            // _exclusions
            // 
            resources.ApplyResources(this._exclusions, "_exclusions");
            this._exclusions.BackColor = System.Drawing.Color.Transparent;
            this._exclusions.Columns = "Exclusion";
            this._exclusions.LabelText = "Enter Exclusions Here:";
            this._exclusions.Name = "_exclusions";
            // 
            // _tabReplacements
            // 
            this._tabReplacements.Controls.Add(this._responseReplacements);
            this._tabReplacements.Controls.Add(this.label9);
            this._tabReplacements.Controls.Add(this._requestReplacements);
            resources.ApplyResources(this._tabReplacements, "_tabReplacements");
            this._tabReplacements.Name = "_tabReplacements";
            this._tabReplacements.UseVisualStyleBackColor = true;
            // 
            // _responseReplacements
            // 
            resources.ApplyResources(this._responseReplacements, "_responseReplacements");
            this._responseReplacements.BackColor = System.Drawing.Color.Transparent;
            this._responseReplacements.Columns = "Regex\r\nReplacement\r\n";
            this._responseReplacements.LabelText = "Response Replacements";
            this._responseReplacements.Name = "_responseReplacements";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // _requestReplacements
            // 
            resources.ApplyResources(this._requestReplacements, "_requestReplacements");
            this._requestReplacements.BackColor = System.Drawing.Color.Transparent;
            this._requestReplacements.Columns = "Regex\r\nReplacement\r\n";
            this._requestReplacements.LabelText = "Request Replacements";
            this._requestReplacements.Name = "_requestReplacements";
            // 
            // _tabPageTrackingPatterns
            // 
            this._tabPageTrackingPatterns.Controls.Add(this.label10);
            this._tabPageTrackingPatterns.Controls.Add(this._trackingPatterns);
            resources.ApplyResources(this._tabPageTrackingPatterns, "_tabPageTrackingPatterns");
            this._tabPageTrackingPatterns.Name = "_tabPageTrackingPatterns";
            this._tabPageTrackingPatterns.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // _trackingPatterns
            // 
            resources.ApplyResources(this._trackingPatterns, "_trackingPatterns");
            this._trackingPatterns.BackColor = System.Drawing.Color.Transparent;
            this._trackingPatterns.Columns = "Name\r\nRequest Pattern\r\nTracking Type:ResponsePattern,AutoDetect,Function\r\nTrackin" +
    "g Value\r\n";
            this._trackingPatterns.LabelText = "";
            this._trackingPatterns.Name = "_trackingPatterns";
            // 
            // _tabRegex
            // 
            this._tabRegex.Controls.Add(this.label8);
            this._tabRegex.Controls.Add(this._boxTimeFormat);
            this._tabRegex.Controls.Add(this.label7);
            this._tabRegex.Controls.Add(this._boxNonHttp);
            this._tabRegex.Controls.Add(this.label6);
            this._tabRegex.Controls.Add(this._boxProxyConnection);
            this._tabRegex.Controls.Add(this.label5);
            this._tabRegex.Controls.Add(this._boxResponseReceived);
            this._tabRegex.Controls.Add(this.label4);
            this._tabRegex.Controls.Add(this._boxDescription);
            this._tabRegex.Controls.Add(this.label3);
            this._tabRegex.Controls.Add(this._boxEndThread);
            this._tabRegex.Controls.Add(this.label2);
            this._tabRegex.Controls.Add(this._boxBeginThread);
            this._tabRegex.Controls.Add(this.label1);
            this._tabRegex.Controls.Add(this._boxThreadId);
            resources.ApplyResources(this._tabRegex, "_tabRegex");
            this._tabRegex.Name = "_tabRegex";
            this._tabRegex.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // _boxTimeFormat
            // 
            resources.ApplyResources(this._boxTimeFormat, "_boxTimeFormat");
            this._boxTimeFormat.Name = "_boxTimeFormat";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // _boxNonHttp
            // 
            resources.ApplyResources(this._boxNonHttp, "_boxNonHttp");
            this._boxNonHttp.Name = "_boxNonHttp";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // _boxProxyConnection
            // 
            resources.ApplyResources(this._boxProxyConnection, "_boxProxyConnection");
            this._boxProxyConnection.Name = "_boxProxyConnection";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // _boxResponseReceived
            // 
            resources.ApplyResources(this._boxResponseReceived, "_boxResponseReceived");
            this._boxResponseReceived.Name = "_boxResponseReceived";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // _boxDescription
            // 
            resources.ApplyResources(this._boxDescription, "_boxDescription");
            this._boxDescription.Name = "_boxDescription";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // _boxEndThread
            // 
            resources.ApplyResources(this._boxEndThread, "_boxEndThread");
            this._boxEndThread.Name = "_boxEndThread";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // _boxBeginThread
            // 
            resources.ApplyResources(this._boxBeginThread, "_boxBeginThread");
            this._boxBeginThread.Name = "_boxBeginThread";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _boxThreadId
            // 
            resources.ApplyResources(this._boxThreadId, "_boxThreadId");
            this._boxThreadId.Name = "_boxThreadId";
            // 
            // _tabCustom
            // 
            this._tabCustom.Controls.Add(this._customFields);
            resources.ApplyResources(this._tabCustom, "_tabCustom");
            this._tabCustom.Name = "_tabCustom";
            this._tabCustom.UseVisualStyleBackColor = true;
            // 
            // _customFields
            // 
            resources.ApplyResources(this._customFields, "_customFields");
            this._customFields.BackColor = System.Drawing.Color.Transparent;
            this._customFields.Columns = "Name\r\nRegex\r\n";
            this._customFields.LabelText = "Enter custom field definitions:";
            this._customFields.Name = "_customFields";
            // 
            // _tabGUI
            // 
            this._tabGUI.Controls.Add(this._highlightingDefs);
            resources.ApplyResources(this._tabGUI, "_tabGUI");
            this._tabGUI.Name = "_tabGUI";
            this._tabGUI.UseVisualStyleBackColor = true;
            // 
            // _highlightingDefs
            // 
            resources.ApplyResources(this._highlightingDefs, "_highlightingDefs");
            this._highlightingDefs.BackColor = System.Drawing.Color.Transparent;
            this._highlightingDefs.Columns = "Description\r\nColor";
            this._highlightingDefs.LabelText = "Highlighting Definitions:";
            this._highlightingDefs.Name = "_highlightingDefs";
            // 
            // _buttonSave
            // 
            resources.ApplyResources(this._buttonSave, "_buttonSave");
            this._buttonSave.Name = "_buttonSave";
            this._buttonSave.UseVisualStyleBackColor = true;
            this._buttonSave.Click += new System.EventHandler(this.SaveClick);
            // 
            // _buttonSaveAs
            // 
            resources.ApplyResources(this._buttonSaveAs, "_buttonSaveAs");
            this._buttonSaveAs.Name = "_buttonSaveAs";
            this._buttonSaveAs.UseVisualStyleBackColor = true;
            this._buttonSaveAs.Click += new System.EventHandler(this.SaveAsClick);
            // 
            // _buttonCancel
            // 
            resources.ApplyResources(this._buttonCancel, "_buttonCancel");
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _dialogSave
            // 
            this._dialogSave.DefaultExt = "xml";
            resources.ApplyResources(this._dialogSave, "_dialogSave");
            this._dialogSave.RestoreDirectory = true;
            // 
            // ProfileEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonSaveAs);
            this.Controls.Add(this._buttonSave);
            this.Controls.Add(this._tabControl);
            this.Name = "ProfileEditor";
            this.ShowInTaskbar = false;
            this._tabControl.ResumeLayout(false);
            this._tabExclusions.ResumeLayout(false);
            this._tabExclusions.PerformLayout();
            this._tabReplacements.ResumeLayout(false);
            this._tabPageTrackingPatterns.ResumeLayout(false);
            this._tabRegex.ResumeLayout(false);
            this._tabRegex.PerformLayout();
            this._tabCustom.ResumeLayout(false);
            this._tabCustom.PerformLayout();
            this._tabGUI.ResumeLayout(false);
            this._tabGUI.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _tabRegex;
		private System.Windows.Forms.TabPage _tabGUI;
		private System.Windows.Forms.Button _buttonSave;
		private System.Windows.Forms.Button _buttonSaveAs;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _boxResponseReceived;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _boxDescription;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _boxEndThread;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _boxBeginThread;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _boxThreadId;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox _boxNonHttp;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _boxProxyConnection;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox _boxTimeFormat;
		private OptionsGrid _highlightingDefs;
		private System.Windows.Forms.TabPage _tabCustom;
		private OptionsGrid _customFields;
		private System.Windows.Forms.SaveFileDialog _dialogSave;
		private System.Windows.Forms.TabPage _tabExclusions;
		private OptionsGrid _exclusions;
		private System.Windows.Forms.TabPage _tabReplacements;
		private System.Windows.Forms.Label label9;
		private OptionsGrid _requestReplacements;
        private System.Windows.Forms.TabPage _tabPageTrackingPatterns;
        private OptionsGrid _trackingPatterns;
		private System.Windows.Forms.Label label10;
        private OptionsGrid _responseReplacements;
	}
}