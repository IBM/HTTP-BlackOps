namespace TrafficViewerControls.Browsing
{
    partial class RequestTrapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestTrapForm));
            this._label = new System.Windows.Forms.Label();
            this._textRequestLine = new System.Windows.Forms.TextBox();
            this._buttonRelease = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _label
            // 
            this._label.Location = new System.Drawing.Point(12, 8);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(352, 46);
            this._label.TabIndex = 0;
            this._label.Click += new System.EventHandler(this._label_Click);
            // 
            // _textRequestLine
            // 
            this._textRequestLine.Location = new System.Drawing.Point(15, 63);
            this._textRequestLine.Multiline = true;
            this._textRequestLine.Name = "_textRequestLine";
            this._textRequestLine.ReadOnly = true;
            this._textRequestLine.Size = new System.Drawing.Size(349, 49);
            this._textRequestLine.TabIndex = 1;
            // 
            // _buttonRelease
            // 
            this._buttonRelease.Location = new System.Drawing.Point(138, 122);
            this._buttonRelease.Name = "_buttonRelease";
            this._buttonRelease.Size = new System.Drawing.Size(94, 22);
            this._buttonRelease.TabIndex = 2;
            this._buttonRelease.Text = "Release";
            this._buttonRelease.UseVisualStyleBackColor = true;
            this._buttonRelease.Click += new System.EventHandler(this.ReleaseClick);
            // 
            // RequestTrapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 153);
            this.Controls.Add(this._buttonRelease);
            this.Controls.Add(this._textRequestLine);
            this.Controls.Add(this._label);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(384, 141);
            this.Name = "RequestTrapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Request Trapped!";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RequestTrapForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.TextBox _textRequestLine;
        private System.Windows.Forms.Button _buttonRelease;
    }
}