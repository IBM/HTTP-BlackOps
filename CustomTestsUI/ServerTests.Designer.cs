namespace CustomTestsUI
{
    partial class ServerTests
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerTests));
            this._dataGrid = new System.Windows.Forms.DataGridView();
            this._issueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._variantName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._hits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._mutation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._validation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._tags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._buttonLoad = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._filterBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _dataGrid
            // 
            this._dataGrid.AllowUserToAddRows = false;
            this._dataGrid.AllowUserToDeleteRows = false;
            this._dataGrid.AllowUserToOrderColumns = true;
            this._dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._issueType,
            this._variantName,
            this._hits,
            this._mutation,
            this._validation,
            this._tags});
            this._dataGrid.Location = new System.Drawing.Point(12, 47);
            this._dataGrid.Name = "_dataGrid";
            this._dataGrid.ReadOnly = true;
            this._dataGrid.Size = new System.Drawing.Size(707, 368);
            this._dataGrid.TabIndex = 0;
            // 
            // _issueType
            // 
            this._issueType.HeaderText = "Issue Type";
            this._issueType.Name = "_issueType";
            this._issueType.ReadOnly = true;
            this._issueType.Width = 141;
            // 
            // _variantName
            // 
            this._variantName.HeaderText = "Variant Name";
            this._variantName.Name = "_variantName";
            this._variantName.ReadOnly = true;
            this._variantName.Width = 141;
            // 
            // _hits
            // 
            this._hits.HeaderText = "Hits";
            this._hits.Name = "_hits";
            this._hits.ReadOnly = true;
            this._hits.Width = 50;
            // 
            // _mutation
            // 
            this._mutation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._mutation.HeaderText = "Mutation";
            this._mutation.Name = "_mutation";
            this._mutation.ReadOnly = true;
            // 
            // _validation
            // 
            this._validation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._validation.HeaderText = "Validation";
            this._validation.Name = "_validation";
            this._validation.ReadOnly = true;
            // 
            // _tags
            // 
            this._tags.HeaderText = "Tags";
            this._tags.Name = "_tags";
            this._tags.ReadOnly = true;
            // 
            // _buttonLoad
            // 
            this._buttonLoad.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._buttonLoad.Location = new System.Drawing.Point(271, 423);
            this._buttonLoad.Name = "_buttonLoad";
            this._buttonLoad.Size = new System.Drawing.Size(75, 23);
            this._buttonLoad.TabIndex = 1;
            this._buttonLoad.Text = "Load";
            this._buttonLoad.UseVisualStyleBackColor = true;
            this._buttonLoad.Click += new System.EventHandler(this.LoadClick);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(352, 423);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CancelClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filter:";
            // 
            // _filterBox
            // 
            this._filterBox.Location = new System.Drawing.Point(50, 18);
            this._filterBox.Name = "_filterBox";
            this._filterBox.Size = new System.Drawing.Size(393, 20);
            this._filterBox.TabIndex = 4;
            this._filterBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterBoxKeyDown);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(449, 16);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Apply";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ApplyFilterClick);
            // 
            // ServerTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 455);
            this.Controls.Add(this.button2);
            this.Controls.Add(this._filterBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._buttonLoad);
            this.Controls.Add(this._dataGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerTests";
            this.Text = "Available Server Tests";
            this.Load += new System.EventHandler(this.ServerTests_Load);
            ((System.ComponentModel.ISupportInitialize)(this._dataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _dataGrid;
        private System.Windows.Forms.Button _buttonLoad;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _filterBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn _issueType;
        private System.Windows.Forms.DataGridViewTextBoxColumn _variantName;
        private System.Windows.Forms.DataGridViewTextBoxColumn _hits;
        private System.Windows.Forms.DataGridViewTextBoxColumn _mutation;
        private System.Windows.Forms.DataGridViewTextBoxColumn _validation;
        private System.Windows.Forms.DataGridViewTextBoxColumn _tags;
    }
}