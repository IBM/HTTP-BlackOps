namespace TrafficViewerControls.Browsing
{
    partial class ExternalBrowserListener
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalBrowserListener));
            this._buttonStart = new System.Windows.Forms.Button();
            this._labelMessage = new System.Windows.Forms.Label();
            this._checkTrapReq = new System.Windows.Forms.CheckBox();
            this._checkTrapResp = new System.Windows.Forms.CheckBox();
            this._labelTrap = new System.Windows.Forms.Label();
            this._textTrapMatch = new System.Windows.Forms.TextBox();
            this._checkTrackReqContext = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _buttonStart
            // 
            this._buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonStart.BackColor = System.Drawing.Color.Transparent;
            this._buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonStart.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonStart.ForeColor = System.Drawing.Color.White;
            this._buttonStart.Location = new System.Drawing.Point(115, 231);
            this._buttonStart.Name = "_buttonStart";
            this._buttonStart.Size = new System.Drawing.Size(97, 23);
            this._buttonStart.TabIndex = 0;
            this._buttonStart.Text = "Start";
            this._buttonStart.UseVisualStyleBackColor = false;
            this._buttonStart.Click += new System.EventHandler(this._buttonStart_Click);
            // 
            // _labelMessage
            // 
            this._labelMessage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelMessage.Location = new System.Drawing.Point(12, 18);
            this._labelMessage.Name = "_labelMessage";
            this._labelMessage.Size = new System.Drawing.Size(307, 54);
            this._labelMessage.TabIndex = 1;
            this._labelMessage.Text = "Listening on ip {ip} port {port}. Configure your browser settings to use this add" +
    "ress as a proxy.";
            this._labelMessage.Click += new System.EventHandler(this._labelMessage_Click);
            // 
            // _checkTrapReq
            // 
            this._checkTrapReq.AutoSize = true;
            this._checkTrapReq.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkTrapReq.Location = new System.Drawing.Point(15, 75);
            this._checkTrapReq.Name = "_checkTrapReq";
            this._checkTrapReq.Size = new System.Drawing.Size(117, 18);
            this._checkTrapReq.TabIndex = 2;
            this._checkTrapReq.Text = "Trap requests";
            this._checkTrapReq.UseVisualStyleBackColor = true;
            this._checkTrapReq.CheckedChanged += new System.EventHandler(this._checkTrap_CheckedChanged);
            // 
            // _checkTrapResp
            // 
            this._checkTrapResp.AutoSize = true;
            this._checkTrapResp.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkTrapResp.Location = new System.Drawing.Point(15, 99);
            this._checkTrapResp.Name = "_checkTrapResp";
            this._checkTrapResp.Size = new System.Drawing.Size(124, 18);
            this._checkTrapResp.TabIndex = 3;
            this._checkTrapResp.Text = "Trap responses";
            this._checkTrapResp.UseVisualStyleBackColor = true;
            this._checkTrapResp.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // _labelTrap
            // 
            this._labelTrap.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelTrap.Location = new System.Drawing.Point(12, 158);
            this._labelTrap.Name = "_labelTrap";
            this._labelTrap.Size = new System.Drawing.Size(307, 22);
            this._labelTrap.TabIndex = 4;
            this._labelTrap.Text = "Trap match regex (stop proxy to apply):";
            // 
            // _textTrapMatch
            // 
            this._textTrapMatch.Location = new System.Drawing.Point(15, 183);
            this._textTrapMatch.Name = "_textTrapMatch";
            this._textTrapMatch.Size = new System.Drawing.Size(290, 20);
            this._textTrapMatch.TabIndex = 5;
            this._textTrapMatch.Text = ".*";
            // 
            // _checkTrackReqContext
            // 
            this._checkTrackReqContext.AutoSize = true;
            this._checkTrackReqContext.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._checkTrackReqContext.Location = new System.Drawing.Point(15, 123);
            this._checkTrackReqContext.Name = "_checkTrackReqContext";
            this._checkTrackReqContext.Size = new System.Drawing.Size(173, 18);
            this._checkTrackReqContext.TabIndex = 6;
            this._checkTrackReqContext.Text = "Track request context";
            this._checkTrackReqContext.UseVisualStyleBackColor = true;
            this._checkTrackReqContext.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // ExternalBrowserListener
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(329, 267);
            this.Controls.Add(this._checkTrackReqContext);
            this.Controls.Add(this._textTrapMatch);
            this.Controls.Add(this._labelTrap);
            this.Controls.Add(this._checkTrapResp);
            this.Controls.Add(this._checkTrapReq);
            this.Controls.Add(this._labelMessage);
            this.Controls.Add(this._buttonStart);
            this.ForeColor = System.Drawing.Color.Lime;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(345, 305);
            this.MinimumSize = new System.Drawing.Size(345, 305);
            this.Name = "ExternalBrowserListener";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Listening...";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExternalBrowserListener_FormClosing);
            this.Load += new System.EventHandler(this.ExternalBrowserListener_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _buttonStart;
        private System.Windows.Forms.Label _labelMessage;
        private System.Windows.Forms.CheckBox _checkTrapReq;
		private System.Windows.Forms.CheckBox _checkTrapResp;
		private System.Windows.Forms.Label _labelTrap;
		private System.Windows.Forms.TextBox _textTrapMatch;
		private System.Windows.Forms.CheckBox _checkTrackReqContext;
    }
}