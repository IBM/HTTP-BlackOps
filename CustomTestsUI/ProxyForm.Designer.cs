namespace CustomTestsUI
{
    partial class ProxyForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxyForm));
            this._button = new System.Windows.Forms.Button();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._labelCurrentTestRequest = new System.Windows.Forms.Label();
            this._labelCurrentRequest = new System.Windows.Forms.Label();
            this._labelTestsRemaining = new System.Windows.Forms.Label();
            this._labelHostAndPort = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _button
            // 
            this._button.Location = new System.Drawing.Point(136, 157);
            this._button.Name = "_button";
            this._button.Size = new System.Drawing.Size(65, 29);
            this._button.TabIndex = 0;
            this._button.Text = "Start";
            this._button.UseVisualStyleBackColor = true;
            this._button.Click += new System.EventHandler(this.StartClick);
            // 
            // _timer
            // 
            this._timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._labelCurrentTestRequest);
            this.groupBox1.Controls.Add(this._labelCurrentRequest);
            this.groupBox1.Controls.Add(this._labelTestsRemaining);
            this.groupBox1.Controls.Add(this._labelHostAndPort);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 130);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stats";
            // 
            // _labelCurrentTestRequest
            // 
            this._labelCurrentTestRequest.AutoSize = true;
            this._labelCurrentTestRequest.Location = new System.Drawing.Point(10, 70);
            this._labelCurrentTestRequest.Name = "_labelCurrentTestRequest";
            this._labelCurrentTestRequest.Size = new System.Drawing.Size(166, 13);
            this._labelCurrentTestRequest.TabIndex = 8;
            this._labelCurrentTestRequest.Text = "Current test request index: {index}";
            // 
            // _labelCurrentRequest
            // 
            this._labelCurrentRequest.AutoSize = true;
            this._labelCurrentRequest.Location = new System.Drawing.Point(10, 94);
            this._labelCurrentRequest.Name = "_labelCurrentRequest";
            this._labelCurrentRequest.Size = new System.Drawing.Size(146, 13);
            this._labelCurrentRequest.TabIndex = 7;
            this._labelCurrentRequest.Text = "Current request index: {index}";
            // 
            // _labelTestsRemaining
            // 
            this._labelTestsRemaining.AutoSize = true;
            this._labelTestsRemaining.Location = new System.Drawing.Point(9, 46);
            this._labelTestsRemaining.Name = "_labelTestsRemaining";
            this._labelTestsRemaining.Size = new System.Drawing.Size(180, 13);
            this._labelTestsRemaining.TabIndex = 6;
            this._labelTestsRemaining.Text = "Tests for current test request {count}";
            // 
            // _labelHostAndPort
            // 
            this._labelHostAndPort.AutoSize = true;
            this._labelHostAndPort.Location = new System.Drawing.Point(10, 22);
            this._labelHostAndPort.Name = "_labelHostAndPort";
            this._labelHostAndPort.Size = new System.Drawing.Size(194, 13);
            this._labelHostAndPort.TabIndex = 5;
            this._labelHostAndPort.Text = "Proxy listening on {host} and port {port}.";
            // 
            // ProxyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 193);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(351, 333);
            this.MinimizeBox = false;
            this.Name = "ProxyForm";
            this.Text = "Attack Proxy";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProxyForm_FormClosing);
            this.Load += new System.EventHandler(this.ProxyForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _button;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label _labelCurrentTestRequest;
        private System.Windows.Forms.Label _labelCurrentRequest;
        private System.Windows.Forms.Label _labelTestsRemaining;
        private System.Windows.Forms.Label _labelHostAndPort;
    }
}