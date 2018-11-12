using System.Drawing;
using System.Windows.Forms;
namespace TrafficViewerControls
{
    public partial class FileSelector : UserControl
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
            this._label = new System.Windows.Forms.Label();
            this._textBox = new System.Windows.Forms.TextBox();

            this._button = new System.Windows.Forms.Button();
            this._button.ForeColor = System.Drawing.Color.Black;
            this._dialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();

            // 
            // _label
            // 
            this._label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(0, 5);
            this._label.Name = "_label";
            this._label.TabIndex = 0;
            this._label.Text = "label1";
            this._label.Size = TextRenderer.MeasureText(this._label.Text, this._label.Font);

            // 
            // _textBox
            // 
            this._textBox.Location = new Point(this._label.Right, this._label.Top);
            this._textBox.Name = "_textBox";
            this._textBox.TabIndex = 1;

            // 
            // _button
            // 
            this._button.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._button.Location = new System.Drawing.Point(this._textBox.Right, this._textBox.Top);
            this._button.Name = "_button";
            this._button.TabIndex = 2;
            this._button.Text = "...";
            this._button.UseVisualStyleBackColor = true;
            this._button.Click += new System.EventHandler(this.ButtonClick);
            this._button.Size = TextRenderer.MeasureText(this._button.Text, this._button.Font);

            // 
            // _dialog
            // 
            this._dialog.FileName = "";
            // 
            // FileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._button);
            this.Controls.Add(this._textBox);
            this.Controls.Add(this._label);
            this.Name = "FileSelector";
            this.Size = new System.Drawing.Size(381, 33);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.TextBox _textBox;
        private System.Windows.Forms.Button _button;
        private System.Windows.Forms.OpenFileDialog _dialog;
    }
}
