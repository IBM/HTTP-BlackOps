namespace TrafficViewerControls
{
    partial class EntitiesView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._buttonStart = new System.Windows.Forms.Button();
            this._gridParameters = new CommonControls.OptionsGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._buttonStart);
            this.splitContainer1.Panel1.Controls.Add(this._gridParameters);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(462, 373);
            this.splitContainer1.SplitterDistance = 344;
            this.splitContainer1.TabIndex = 0;
            // 
            // _buttonStart
            // 
            this._buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._buttonStart.BackColor = System.Drawing.Color.Transparent;
            this._buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonStart.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._buttonStart.ForeColor = System.Drawing.Color.White;
            this._buttonStart.Location = new System.Drawing.Point(169, 338);
            this._buttonStart.Name = "_buttonStart";
            this._buttonStart.Size = new System.Drawing.Size(117, 23);
            this._buttonStart.TabIndex = 15;
            this._buttonStart.Text = "Update Request";
            this._buttonStart.UseVisualStyleBackColor = false;
            this._buttonStart.Click += new System.EventHandler(this.UpdateRequest);
            // 
            // _gridParameters
            // 
            this._gridParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gridParameters.BackColor = System.Drawing.Color.Transparent;
            this._gridParameters.Columns = "Location\r\nName\r\nValue";
            this._gridParameters.ForeColor = System.Drawing.Color.White;
            this._gridParameters.LabelText = "Request Entities:";
            this._gridParameters.Location = new System.Drawing.Point(5, 7);
            this._gridParameters.Margin = new System.Windows.Forms.Padding(4);
            this._gridParameters.Name = "_gridParameters";
            this._gridParameters.Size = new System.Drawing.Size(454, 324);
            this._gridParameters.TabIndex = 8;
            // 
            // EntitiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EntitiesView";
            this.Size = new System.Drawing.Size(462, 373);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private CommonControls.OptionsGrid _gridParameters;
        private System.Windows.Forms.Button _buttonStart;


    }
}
