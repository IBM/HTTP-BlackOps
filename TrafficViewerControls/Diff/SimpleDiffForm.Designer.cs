namespace TrafficViewerControls.Diff
{
	partial class SimpleDiffForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleDiffForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this._requestID1 = new System.Windows.Forms.TextBox();
            this._boxFirst = new TrafficViewerControls.TextBoxes.TrafficTextBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this._requestID2 = new System.Windows.Forms.TextBox();
            this._buttonPrevDiff = new System.Windows.Forms.Button();
            this._buttonNextDiff = new System.Windows.Forms.Button();
            this._buttonPrev = new System.Windows.Forms.Button();
            this._buttonNext = new System.Windows.Forms.Button();
            this._boxSecond = new TrafficViewerControls.TextBoxes.TrafficTextBox();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._labelSimilarity = new System.Windows.Forms.ToolStripStatusLabel();
            this._progress = new System.Windows.Forms.ToolStripProgressBar();
            this._diffWorker = new System.ComponentModel.BackgroundWorker();
            this._timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this._requestID1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this._boxFirst);
            // 
            // _requestID1
            // 
            resources.ApplyResources(this._requestID1, "_requestID1");
            this._requestID1.BackColor = System.Drawing.SystemColors.Control;
            this._requestID1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._requestID1.ForeColor = System.Drawing.SystemColors.MenuText;
            this._requestID1.Name = "_requestID1";
            this._requestID1.ReadOnly = true;
            this._requestID1.TabStop = false;
            // 
            // _boxFirst
            // 
            resources.ApplyResources(this._boxFirst, "_boxFirst");
            this._boxFirst.AutoSearchCriteria = null;
            this._boxFirst.LineMatch = null;
            this._boxFirst.Name = "_boxFirst";
            this._boxFirst.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New" +
    ";}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            // 
            // splitContainer3
            // 
            resources.ApplyResources(this.splitContainer3, "splitContainer3");
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this._requestID2);
            this.splitContainer3.Panel1.Controls.Add(this._buttonPrevDiff);
            this.splitContainer3.Panel1.Controls.Add(this._buttonNextDiff);
            this.splitContainer3.Panel1.Controls.Add(this._buttonPrev);
            this.splitContainer3.Panel1.Controls.Add(this._buttonNext);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this._boxSecond);
            // 
            // _requestID2
            // 
            resources.ApplyResources(this._requestID2, "_requestID2");
            this._requestID2.BackColor = System.Drawing.SystemColors.Control;
            this._requestID2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._requestID2.ForeColor = System.Drawing.SystemColors.MenuText;
            this._requestID2.Name = "_requestID2";
            this._requestID2.ReadOnly = true;
            this._requestID2.TabStop = false;
            // 
            // _buttonPrevDiff
            // 
            resources.ApplyResources(this._buttonPrevDiff, "_buttonPrevDiff");
            this._buttonPrevDiff.Name = "_buttonPrevDiff";
            this._buttonPrevDiff.UseVisualStyleBackColor = true;
            this._buttonPrevDiff.Click += new System.EventHandler(this.ButtonPrevDiffClick);
            // 
            // _buttonNextDiff
            // 
            resources.ApplyResources(this._buttonNextDiff, "_buttonNextDiff");
            this._buttonNextDiff.Name = "_buttonNextDiff";
            this._buttonNextDiff.UseVisualStyleBackColor = true;
            this._buttonNextDiff.Click += new System.EventHandler(this.ButtonNextDiffClick);
            // 
            // _buttonPrev
            // 
            resources.ApplyResources(this._buttonPrev, "_buttonPrev");
            this._buttonPrev.Name = "_buttonPrev";
            this._buttonPrev.UseVisualStyleBackColor = true;
            this._buttonPrev.Click += new System.EventHandler(this.PrevClick);
            // 
            // _buttonNext
            // 
            resources.ApplyResources(this._buttonNext, "_buttonNext");
            this._buttonNext.Name = "_buttonNext";
            this._buttonNext.UseVisualStyleBackColor = true;
            this._buttonNext.Click += new System.EventHandler(this.NextClick);
            // 
            // _boxSecond
            // 
            resources.ApplyResources(this._boxSecond, "_boxSecond");
            this._boxSecond.AutoSearchCriteria = null;
            this._boxSecond.LineMatch = null;
            this._boxSecond.Name = "_boxSecond";
            this._boxSecond.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New" +
    ";}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            // 
            // _statusStrip
            // 
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._labelSimilarity,
            this._progress});
            resources.ApplyResources(this._statusStrip, "_statusStrip");
            this._statusStrip.Name = "_statusStrip";
            // 
            // _labelSimilarity
            // 
            this._labelSimilarity.Name = "_labelSimilarity";
            resources.ApplyResources(this._labelSimilarity, "_labelSimilarity");
            // 
            // _progress
            // 
            this._progress.Name = "_progress";
            resources.ApplyResources(this._progress, "_progress");
            // 
            // _diffWorker
            // 
            this._diffWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DiffDoWork);
            this._diffWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DiffWorkerCompleted);
            // 
            // _timer
            // 
            this._timer.Tick += new System.EventHandler(this.ProgressTick);
            // 
            // SimpleDiffForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SimpleDiffForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleDiffForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private TrafficViewerControls.TextBoxes.TrafficTextBox _boxFirst;
		private System.Windows.Forms.Button _buttonPrevDiff;
		private System.Windows.Forms.Button _buttonNextDiff;
		private System.Windows.Forms.Button _buttonPrev;
		private System.Windows.Forms.Button _buttonNext;
		private TrafficViewerControls.TextBoxes.TrafficTextBox _boxSecond;
		private System.Windows.Forms.TextBox _requestID1;
		private System.Windows.Forms.TextBox _requestID2;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel _labelSimilarity;
		private System.Windows.Forms.ToolStripProgressBar _progress;
		private System.ComponentModel.BackgroundWorker _diffWorker;
		private System.Windows.Forms.Timer _timer;


	}
}