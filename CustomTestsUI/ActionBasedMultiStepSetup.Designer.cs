using CommonControls;
namespace CustomTestsUI
{
    partial class ActionBasedMultiStepSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionBasedMultiStepSetup));
            this._buttonOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this._multiStepsGrid = new CommonControls.OptionsGrid();
            this.SuspendLayout();
            // 
            // _buttonOK
            // 
            this._buttonOK.Location = new System.Drawing.Point(195, 349);
            this._buttonOK.Name = "_buttonOK";
            this._buttonOK.Size = new System.Drawing.Size(94, 24);
            this._buttonOK.TabIndex = 1;
            this._buttonOK.Text = "OK";
            this._buttonOK.UseVisualStyleBackColor = true;
            this._buttonOK.Click += new System.EventHandler(this._buttonOK_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(295, 349);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 24);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _multiStepsGrid
            // 
            this._multiStepsGrid.BackColor = System.Drawing.Color.Transparent;
            this._multiStepsGrid.Columns = "*.login (from AppScan only) or HTD file path";
            this._multiStepsGrid.LabelText = "Specify location of one or more files to use as Multi-Step Operations";
            this._multiStepsGrid.Location = new System.Drawing.Point(13, 4);
            this._multiStepsGrid.Margin = new System.Windows.Forms.Padding(4);
            this._multiStepsGrid.Name = "_multiStepsGrid";
            this._multiStepsGrid.Size = new System.Drawing.Size(559, 338);
            this._multiStepsGrid.TabIndex = 3;
            // 
            // ActionBasedMultiStepSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 385);
            this.Controls.Add(this._multiStepsGrid);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._buttonOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(601, 423);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(601, 423);
            this.Name = "ActionBasedMultiStepSetup";
            this.Text = "Multi-Step Operations";
            this.Load += new System.EventHandler(this.ActionBasedMultiStepSetup_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonOK;
        private System.Windows.Forms.Button button1;
        private OptionsGrid _multiStepsGrid;
    }
}