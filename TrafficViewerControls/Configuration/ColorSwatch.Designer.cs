namespace TrafficViewerControls.Configuration
{
	partial class ColorSwatch
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
			this._labelText = new System.Windows.Forms.Label();
			this._buttonSwatch = new System.Windows.Forms.Button();
			this._dialogColor = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// _labelText
			// 
			this._labelText.AutoSize = true;
			this._labelText.Location = new System.Drawing.Point(12, 9);
			this._labelText.Name = "_labelText";
			this._labelText.Size = new System.Drawing.Size(36, 13);
			this._labelText.TabIndex = 0;
			// 
			// _buttonSwatch
			// 
			this._buttonSwatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonSwatch.Location = new System.Drawing.Point(205, 4);
			this._buttonSwatch.Name = "_buttonSwatch";
			this._buttonSwatch.Size = new System.Drawing.Size(31, 23);
			this._buttonSwatch.TabIndex = 1;
			this._buttonSwatch.UseVisualStyleBackColor = false;
			this._buttonSwatch.Click += new System.EventHandler(this.SwatchClick);
			// 
			// ColorSwatch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._buttonSwatch);
			this.Controls.Add(this._labelText);
			this.Name = "ColorSwatch";
			this.Size = new System.Drawing.Size(249, 31);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _labelText;
		private System.Windows.Forms.Button _buttonSwatch;
		private System.Windows.Forms.ColorDialog _dialogColor;
	}
}
