using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonControls
{
	public partial class InputMessageBox : Form
	{
		public InputMessageBox()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Displays the dialog with a message and returns the text entered by the user
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public string ShowDialog(string message, string defaultVal = null)
		{
			_labelMessage.Text = message;
            if (defaultVal != null)
            { 
                _textInput.Text = defaultVal;
            }
			if (ShowDialog() == DialogResult.OK)
			{
				return _textInput.Text;
			}

			return null;
		}

		private void _buttonOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void _textInput_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}

        private void _labelMessage_Click(object sender, EventArgs e)
        {

        }


	}
}
