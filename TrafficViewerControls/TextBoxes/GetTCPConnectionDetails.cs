using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrafficViewerControls.TextBoxes
{
	public partial class GetTCPConnectionDetails : Form
	{

		private bool _isSaved = false;
		/// <summary>
		/// Whether the form was saved or not
		/// </summary>
		public bool IsSaved
		{
			get { return _isSaved; }
			set { _isSaved = value; }
		}

		
		public string Host
		{
			get
			{
				return _textHost.Text;
			}
		}

		public string Port
		{
			get
			{
				return _textPort.Text;
			}
		}


		public bool IsSecure
		{
			get
			{
				return _checkIsSecure.Checked;
			}
		}


		public GetTCPConnectionDetails()
		{
			InitializeComponent();
			_textHost.Text = TrafficViewerInstance.TrafficViewerOptions.Instance.ForwardingHost;
			_textPort.Text = TrafficViewerInstance.TrafficViewerOptions.Instance.ForwardingPort.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void _buttonStart_Click(object sender, EventArgs e)
		{
			_isSaved = true;
			this.Hide();
		}

		private void GetTCPConnectionDetails_Activated(object sender, EventArgs e)
		{
			_isSaved = false;
		}

		private void _textPort_TextChanged(object sender, EventArgs e)
		{
			if (_textPort.Text.Contains("443"))
			{
				_checkIsSecure.Checked = true;
			}
			else if(_textPort.Text.Contains("80"))
			{
				_checkIsSecure.Checked = false;
			}
		}
	}
}
