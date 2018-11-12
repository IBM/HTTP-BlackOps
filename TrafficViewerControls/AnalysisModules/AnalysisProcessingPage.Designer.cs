namespace TrafficViewerControls.AnalysisModules
{
	partial class AnalysisProcessingPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisProcessingPage));
			this._buttonDone = new System.Windows.Forms.Button();
			this._textLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._progressTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// _buttonDone
			// 
			resources.ApplyResources(this._buttonDone, "_buttonDone");
			this._buttonDone.Name = "_buttonDone";
			this._buttonDone.UseVisualStyleBackColor = true;
			this._buttonDone.Click += new System.EventHandler(this.DoneClick);
			// 
			// _textLog
			// 
			resources.ApplyResources(this._textLog, "_textLog");
			this._textLog.Name = "_textLog";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _progressBar
			// 
			resources.ApplyResources(this._progressBar, "_progressBar");
			this._progressBar.Name = "_progressBar";
			// 
			// _progressTimer
			// 
			this._progressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
			// 
			// AnalysisProcessingPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._progressBar);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._textLog);
			this.Controls.Add(this._buttonDone);
			this.Name = "AnalysisProcessingPage";
			this.Load += new System.EventHandler(this.AnalysisProcessingPage_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _buttonDone;
		private System.Windows.Forms.TextBox _textLog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.Timer _progressTimer;
	}
}
