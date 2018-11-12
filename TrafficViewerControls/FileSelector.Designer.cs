namespace TrafficViewerControls
{
	partial class FileSelector
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
			this._label = new System.Windows.Forms.Label();
			this._textBox = new System.Windows.Forms.TextBox();
			this._button = new System.Windows.Forms.Button();
			this._dialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// _label
			// 
			this._label.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._label.AutoSize = true;
			this._label.Location = new System.Drawing.Point(4, 9);
			this._label.Name = "_label";
			this._label.Size = new System.Drawing.Size(35, 13);
			this._label.TabIndex = 0;
			this._label.Text = "label1";
			// 
			// _textBox
			// 
			this._textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._textBox.Location = new System.Drawing.Point(71, 5);
			this._textBox.Name = "_textBox";
			this._textBox.Size = new System.Drawing.Size(243, 20);
			this._textBox.TabIndex = 1;
			// 
			// _button
			// 
			this._button.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._button.Location = new System.Drawing.Point(333, 4);
			this._button.Name = "_button";
			this._button.Size = new System.Drawing.Size(31, 23);
			this._button.TabIndex = 2;
			this._button.Text = "...";
			this._button.UseVisualStyleBackColor = true;
			this._button.Click += new System.EventHandler(this.ButtonClick);
			// 
			// _dialog
			// 
			this._dialog.FileName = "openFileDialog1";
			// 
			// FileSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this._button);
			this.Controls.Add(this._textBox);
			this.Controls.Add(this._label);
			this.Name = "FileSelector";
			this.Size = new System.Drawing.Size(381, 33);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _label;
		private System.Windows.Forms.TextBox _textBox;
		private System.Windows.Forms.Button _button;
		private System.Windows.Forms.OpenFileDialog _dialog;
	}
}
