namespace TrafficViewerControls
{
	partial class ImmediateFinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImmediateFinder));
            this._textSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._checkUseRegex = new System.Windows.Forms.CheckBox();
            this._buttonFind = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _textSearch
            // 
            resources.ApplyResources(this._textSearch, "_textSearch");
            this._textSearch.Name = "_textSearch";
            this._textSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextSearchKeyPress);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _checkUseRegex
            // 
            resources.ApplyResources(this._checkUseRegex, "_checkUseRegex");
            this._checkUseRegex.Name = "_checkUseRegex";
            this._checkUseRegex.UseVisualStyleBackColor = true;
            // 
            // _buttonFind
            // 
            resources.ApplyResources(this._buttonFind, "_buttonFind");
            this._buttonFind.Name = "_buttonFind";
            this._buttonFind.UseVisualStyleBackColor = true;
            this._buttonFind.Click += new System.EventHandler(this.FindClick);
            // 
            // _buttonCancel
            // 
            resources.ApplyResources(this._buttonCancel, "_buttonCancel");
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // ImmediateFinder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonFind);
            this.Controls.Add(this._checkUseRegex);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._textSearch);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImmediateFinder";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImmediateFinder_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ImmediateFinder_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _textSearch;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox _checkUseRegex;
		private System.Windows.Forms.Button _buttonFind;
		private System.Windows.Forms.Button _buttonCancel;
	}
}