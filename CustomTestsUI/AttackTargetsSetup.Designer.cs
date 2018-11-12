﻿using CommonControls;
namespace CustomTestsUI
{
    partial class AttackTargetsSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttackTargetsSetup));
            this._buttonOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this._attackTargetListGrid = new CommonControls.OptionsGrid();
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
            // _attackTargetListGrid
            // 
            this._attackTargetListGrid.BackColor = System.Drawing.Color.Transparent;
            this._attackTargetListGrid.Columns = "Name\r\nStatus:Enabled,Disabled\r\nRequest Pattern";
            this._attackTargetListGrid.LabelText = "Specify one or more attack targets";
            this._attackTargetListGrid.Location = new System.Drawing.Point(5, 3);
            this._attackTargetListGrid.Margin = new System.Windows.Forms.Padding(4);
            this._attackTargetListGrid.Name = "_attackTargetListGrid";
            this._attackTargetListGrid.Size = new System.Drawing.Size(577, 339);
            this._attackTargetListGrid.TabIndex = 3;
            // 
            // AttackTargetsSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 385);
            this.Controls.Add(this._attackTargetListGrid);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._buttonOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(601, 423);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(601, 423);
            this.Name = "AttackTargetsSetup";
            this.Text = "Attack Targets";
            this.Load += new System.EventHandler(this.ActionBasedMultiStepSetup_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private OptionsGrid _multiStepGrid;
        private System.Windows.Forms.Button _buttonOK;
        private System.Windows.Forms.Button button1;
        private OptionsGrid _attackTargetListGrid;
    }
}