namespace TrafficViewerControls
{
	partial class RequestLogSyncView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestLogSyncView));
			this._fileSelector = new TrafficViewerControls.FileSelector();
			this._browser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// _fileSelector
			// 
			this._fileSelector.BackColor = System.Drawing.Color.White;
			this._fileSelector.CheckFileExists = true;
			resources.ApplyResources(this._fileSelector, "_fileSelector");
			this._fileSelector.Filter = "";
			this._fileSelector.ForeColor = System.Drawing.Color.Black;
			this._fileSelector.Label = "Log file path:";
			this._fileSelector.Name = "_fileSelector";
			// 
			// _browser
			// 
			this._browser.AllowWebBrowserDrop = false;
			resources.ApplyResources(this._browser, "_browser");
			this._browser.IsWebBrowserContextMenuEnabled = false;
			this._browser.Name = "_browser";
			this._browser.ScriptErrorsSuppressed = true;
			this._browser.WebBrowserShortcutsEnabled = false;
			// 
			// RequestLogSyncView
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this._fileSelector);
			this.Controls.Add(this._browser);
			this.Name = "RequestLogSyncView";
			resources.ApplyResources(this, "$this");
			this.ResumeLayout(false);

		}

		#endregion

		private FileSelector _fileSelector;
		private System.Windows.Forms.WebBrowser _browser;






	}
}
