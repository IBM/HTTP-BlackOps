namespace TrafficViewerControls.Browsing
{
    partial class ProxySetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxySetup));
            this._buttonStart = new System.Windows.Forms.Button();
            this._hostBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._portBox = new System.Windows.Forms.TextBox();
            this._securePort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._extraOptionsGrid = new CommonControls.OptionsGrid();
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
            this._buttonStart.Location = new System.Drawing.Point(115, 340);
            this._buttonStart.Name = "_buttonStart";
            this._buttonStart.Size = new System.Drawing.Size(116, 23);
            this._buttonStart.TabIndex = 0;
            this._buttonStart.Text = "OK";
            this._buttonStart.UseVisualStyleBackColor = false;
            this._buttonStart.Click += new System.EventHandler(this._buttonStart_Click);
            // 
            // _hostBox
            // 
            this._hostBox.BackColor = System.Drawing.Color.Black;
            this._hostBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._hostBox.Font = new System.Drawing.Font("Courier New", 8.25F);
            this._hostBox.ForeColor = System.Drawing.Color.White;
            this._hostBox.FormattingEnabled = true;
            this._hostBox.Location = new System.Drawing.Point(108, 21);
            this._hostBox.Name = "_hostBox";
            this._hostBox.Size = new System.Drawing.Size(189, 22);
            this._hostBox.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 14);
            this.label1.TabIndex = 10;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "Port:";
            // 
            // _portBox
            // 
            this._portBox.BackColor = System.Drawing.Color.Black;
            this._portBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._portBox.ForeColor = System.Drawing.Color.White;
            this._portBox.Location = new System.Drawing.Point(108, 55);
            this._portBox.Name = "_portBox";
            this._portBox.Size = new System.Drawing.Size(63, 20);
            this._portBox.TabIndex = 12;
            // 
            // _securePort
            // 
            this._securePort.BackColor = System.Drawing.Color.Black;
            this._securePort.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._securePort.ForeColor = System.Drawing.Color.White;
            this._securePort.Location = new System.Drawing.Point(108, 87);
            this._securePort.Name = "_securePort";
            this._securePort.Size = new System.Drawing.Size(63, 20);
            this._securePort.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(12, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 14);
            this.label3.TabIndex = 13;
            this.label3.Text = "Secure Port:";
            // 
            // _extraOptionsGrid
            // 
            this._extraOptionsGrid.BackColor = System.Drawing.Color.Transparent;
            this._extraOptionsGrid.Columns = "Option\r\nValue";
            this._extraOptionsGrid.ForeColor = System.Drawing.Color.White;
            this._extraOptionsGrid.LabelText = "Extra Options:";
            this._extraOptionsGrid.Location = new System.Drawing.Point(13, 124);
            this._extraOptionsGrid.Margin = new System.Windows.Forms.Padding(4);
            this._extraOptionsGrid.Name = "_extraOptionsGrid";
            this._extraOptionsGrid.Size = new System.Drawing.Size(322, 209);
            this._extraOptionsGrid.TabIndex = 16;
            // 
            // ProxySetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(348, 376);
            this.Controls.Add(this._extraOptionsGrid);
            this.Controls.Add(this._securePort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._portBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._hostBox);
            this.Controls.Add(this._buttonStart);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(364, 414);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(364, 414);
            this.Name = "ProxySetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Proxy Setup";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProxySetup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _buttonStart;
        private System.Windows.Forms.ComboBox _hostBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _portBox;
        private System.Windows.Forms.TextBox _securePort;
        private System.Windows.Forms.Label label3;
        private CommonControls.OptionsGrid _extraOptionsGrid;
    }
}