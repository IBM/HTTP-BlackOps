namespace TrafficViewerControls.TextBoxes
{
    partial class AttachmentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachmentForm));
            this.label1 = new System.Windows.Forms.Label();
            this._fileSelector = new TrafficViewerControls.FileSelector();
            this.button1 = new System.Windows.Forms.Button();
            this._checkEncode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _fileSelector
            // 
            this._fileSelector.BackColor = System.Drawing.Color.Transparent;
            this._fileSelector.CheckFileExists = true;
            this._fileSelector.Filter = "";
            this._fileSelector.Label = "Location:";
            resources.ApplyResources(this._fileSelector, "_fileSelector");
            this._fileSelector.Name = "_fileSelector";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _checkEncode
            // 
            resources.ApplyResources(this._checkEncode, "_checkEncode");
            this._checkEncode.Checked = true;
            this._checkEncode.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkEncode.Name = "_checkEncode";
            this._checkEncode.UseVisualStyleBackColor = true;
            // 
            // AttachmentForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._checkEncode);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._fileSelector);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttachmentForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private FileSelector _fileSelector;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox _checkEncode;
    }
}