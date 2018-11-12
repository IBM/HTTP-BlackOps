namespace TrafficViewerControls.AnalysisModules
{
	partial class AnalysisModulesHtmlView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisModulesHtmlView));
            this._browser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // _browser
            // 
            resources.ApplyResources(this._browser, "_browser");
            this._browser.Name = "_browser";
            // 
            // AnalysisModulesHtmlView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._browser);
            this.Name = "AnalysisModulesHtmlView";
            this.Load += new System.EventHandler(this.AnalysisModulesHtmlViewLoad);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser _browser;
	}
}