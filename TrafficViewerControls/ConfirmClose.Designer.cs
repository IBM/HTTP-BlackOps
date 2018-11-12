namespace TrafficViewerControls
{
	partial class ConfirmClose
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmClose));
            this.label1 = new System.Windows.Forms.Label();
            this._buttonPack = new System.Windows.Forms.Button();
            this._buttonLeave = new System.Windows.Forms.Button();
            this._buttonDiscard = new System.Windows.Forms.Button();
            this._checkPrompt = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _buttonPack
            // 
            resources.ApplyResources(this._buttonPack, "_buttonPack");
            this._buttonPack.Name = "_buttonPack";
            this._buttonPack.UseVisualStyleBackColor = true;
            this._buttonPack.Click += new System.EventHandler(this.PackClick);
            // 
            // _buttonLeave
            // 
            resources.ApplyResources(this._buttonLeave, "_buttonLeave");
            this._buttonLeave.Name = "_buttonLeave";
            this._buttonLeave.UseVisualStyleBackColor = true;
            this._buttonLeave.Click += new System.EventHandler(this.LeaveClick);
            // 
            // _buttonDiscard
            // 
            resources.ApplyResources(this._buttonDiscard, "_buttonDiscard");
            this._buttonDiscard.Name = "_buttonDiscard";
            this._buttonDiscard.UseVisualStyleBackColor = true;
            this._buttonDiscard.Click += new System.EventHandler(this.DiscardClick);
            // 
            // _checkPrompt
            // 
            resources.ApplyResources(this._checkPrompt, "_checkPrompt");
            this._checkPrompt.Name = "_checkPrompt";
            this._checkPrompt.UseVisualStyleBackColor = true;
            // 
            // ConfirmClose
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._checkPrompt);
            this.Controls.Add(this._buttonDiscard);
            this.Controls.Add(this._buttonLeave);
            this.Controls.Add(this._buttonPack);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmClose";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _buttonPack;
		private System.Windows.Forms.Button _buttonLeave;
		private System.Windows.Forms.Button _buttonDiscard;
		private System.Windows.Forms.CheckBox _checkPrompt;
	}
}