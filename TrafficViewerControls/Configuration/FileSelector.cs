using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TrafficViewerControls
{
	public partial class FileSelector : UserControl
	{
		/// <summary>
		/// Triggered when the user has successfully selected a file
		/// </summary>
		public event EventHandler FileSelected;

		public override string Text
		{
			get
			{
				return _textBox.Text;
			}
			set
			{
				_textBox.Text = value;
			}
		}

		/// <summary>
		/// The label
		/// </summary>
		public string Label
		{
			get
			{
				return _label.Text;
			}
			set
			{
				_label.Text = value;
			}
		}

		/// <summary>
		/// File filter
		/// </summary>
		public string Filter
		{
			get
			{
				return _dialog.Filter;
			}
			set
			{
				_dialog.Filter = value;
			}
		}

		/// <summary>
		/// Whether to check if the file exists
		/// </summary>
		public bool CheckFileExists
		{
			get
			{
				return _dialog.CheckFileExists;
			}
			set
			{
				_dialog.CheckFileExists = value;
			}

		}

		public FileSelector()
		{
			InitializeComponent();
		}

		private void ButtonClick(object sender, EventArgs e)
		{
			DialogResult dr = _dialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_textBox.Text = _dialog.FileName;
				if (FileSelected != null)
				{
					FileSelected.Invoke(sender, e);
				}
			}
		}

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Size sz = TextRenderer.MeasureText(this._label.Text, this._label.Font);
            this._label.Size = new Size(sz.Width + 5, this.Height);
            sz = TextRenderer.MeasureText(this._button.Text, this._button.Font);
            this._button.Width = sz.Width + 9;
            this._button.Top = this._textBox.Top;
            this._button.Height = this._textBox.Height;
            this._textBox.Left = this._label.Right + 5;
            int tbSize = this.Width - (this._label.Left + this._label.Width + this._button.Width + 5);
            if (tbSize <= 10)
                tbSize = 10;
            this._textBox.Size = new Size(tbSize, this.Height);
            this._button.Left = this._textBox.Right + 1;

        }
	}
}
