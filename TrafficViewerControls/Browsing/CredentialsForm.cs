using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.Http;

namespace TrafficViewerControls.Browsing
{
	public partial class CredentialsForm : Form, ICredentialsProvider
	{
		private Form _parent;

		public CredentialsForm(Form parent)
		{
			_parent = parent;
			InitializeComponent();
		}

		private void OKClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();

		}

		private void UserKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				OKClick(null, null);
			}
		}

		private void PassKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				OKClick(null, null);
			}
		}

        public bool Execute(out string domain, out string userName, out string password)
        {
            bool success = false;
			string d = null, u = null, p = null;
			_parent.Invoke((MethodInvoker)delegate
			{
				if (this.ShowDialog() == DialogResult.OK)
				{
					if (!String.IsNullOrEmpty(_textUser.Text))
					{
						d = _textDomain.Text;
						u = _textUser.Text;
						p = _textPass.Text;
						success = true;
					}

				}
			});
			domain = d;
			userName = u;
			password = p;
            return success;
        }

	}
}
