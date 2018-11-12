using System.Windows.Forms;
namespace TrafficViewerControls.AnalysisModules
{
	partial class AnalisysModulesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalisysModulesForm));
            this._split = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this._split)).BeginInit();
            this._split.SuspendLayout();
            this.SuspendLayout();
            // 
            // _split
            // 
            resources.ApplyResources(this._split, "_split");
            this._split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._split.Name = "_split";
            // 
            // _split.Panel1
            // 
            this._split.Panel1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this._split.Panel1, "_split.Panel1");
            // 
            // AnalisysModulesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._split);
            this.Name = "AnalisysModulesForm";
            ((System.ComponentModel.ISupportInitialize)(this._split)).EndInit();
            this._split.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _split;
	}
}