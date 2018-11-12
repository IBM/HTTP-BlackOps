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
			}
		}
	}
}
