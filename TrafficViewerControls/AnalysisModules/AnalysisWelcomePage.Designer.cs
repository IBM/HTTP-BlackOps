namespace TrafficViewerControls.AnalysisModules
{
	partial class AnalysisWelcomePage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisWelcomePage));
			this._title = new System.Windows.Forms.Label();
			this._description = new System.Windows.Forms.Label();
			this._buttonStart = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _title
			// 
			resources.ApplyResources(this._title, "_title");
			this._title.Name = "_title";
			// 
			// _description
			// 
			resources.ApplyResources(this._description, "_description");
			this._description.Name = "_description";
			// 
			// _buttonStart
			// 
			resources.ApplyResources(this._buttonStart, "_buttonStart");
			this._buttonStart.Name = "_buttonStart";
			this._buttonStart.UseVisualStyleBackColor = true;
			// 
			// AnalysisWelcomePage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._buttonStart);
			this.Controls.Add(this._description);
			this.Controls.Add(this._title);
			this.Name = "AnalysisWelcomePage";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _title;
		private System.Windows.Forms.Label _description;
		private System.Windows.Forms.Button _buttonStart;
	}
}
