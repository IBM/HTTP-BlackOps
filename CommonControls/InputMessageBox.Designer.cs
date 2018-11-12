namespace CommonControls
{
	partial class InputMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputMessageBox));
            this._labelMessage = new System.Windows.Forms.Label();
            this._buttonOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this._textInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _labelMessage
            // 
            resources.ApplyResources(this._labelMessage, "_labelMessage");
            this._labelMessage.Name = "_labelMessage";
            this._labelMessage.Click += new System.EventHandler(this._labelMessage_Click);
            // 
            // _buttonOk
            // 
            resources.ApplyResources(this._buttonOk, "_buttonOk");
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.UseVisualStyleBackColor = true;
            this._buttonOk.Click += new System.EventHandler(this._buttonOk_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _textInput
            // 
            resources.ApplyResources(this._textInput, "_textInput");
            this._textInput.Name = "_textInput";
            this._textInput.TabStop = false;
            this._textInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textInput_KeyPress);
            // 
            // InputMessageBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._textInput);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._buttonOk);
            this.Controls.Add(this._labelMessage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputMessageBox";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _labelMessage;
		private System.Windows.Forms.Button _buttonOk;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox _textInput;
	}
}