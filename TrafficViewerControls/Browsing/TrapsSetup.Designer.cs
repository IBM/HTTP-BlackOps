namespace TrafficViewerControls.Browsing
{
    partial class TrapsSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrapsSetup));
            this._buttonApply = new System.Windows.Forms.Button();
            this._trapsGrid = new CommonControls.OptionsGrid();
            this._buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonApply
            // 
            this._buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._buttonApply.BackColor = System.Drawing.Color.Transparent;
            this._buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonApply.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonApply.ForeColor = System.Drawing.Color.White;
            this._buttonApply.Location = new System.Drawing.Point(221, 341);
            this._buttonApply.Name = "_buttonApply";
            this._buttonApply.Size = new System.Drawing.Size(91, 23);
            this._buttonApply.TabIndex = 0;
            this._buttonApply.Text = "Apply";
            this._buttonApply.UseVisualStyleBackColor = false;
            this._buttonApply.Click += new System.EventHandler(this.ApplyClick);
            // 
            // _trapsGrid
            // 
            this._trapsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._trapsGrid.BackColor = System.Drawing.Color.Transparent;
            this._trapsGrid.Columns = "Enabled:Disabled,Enabled\r\nType:Request,Response\r\nRegex";
            this._trapsGrid.ForeColor = System.Drawing.Color.White;
            this._trapsGrid.LabelText = "Traps:";
            this._trapsGrid.Location = new System.Drawing.Point(13, 13);
            this._trapsGrid.Margin = new System.Windows.Forms.Padding(4);
            this._trapsGrid.Name = "_trapsGrid";
            this._trapsGrid.Size = new System.Drawing.Size(600, 320);
            this._trapsGrid.TabIndex = 16;
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._buttonCancel.BackColor = System.Drawing.Color.Transparent;
            this._buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonCancel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonCancel.ForeColor = System.Drawing.Color.White;
            this._buttonCancel.Location = new System.Drawing.Point(318, 341);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(91, 23);
            this._buttonCancel.TabIndex = 17;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = false;
            this._buttonCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // TrapsSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(626, 376);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._trapsGrid);
            this.Controls.Add(this._buttonApply);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(642, 414);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(642, 414);
            this.Name = "TrapsSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Traps";
            this.Load += new System.EventHandler(this.TrapsSetup_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonApply;
        private CommonControls.OptionsGrid _trapsGrid;
        private System.Windows.Forms.Button _buttonCancel;
    }
}