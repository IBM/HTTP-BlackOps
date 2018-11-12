using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.Options;
using TrafficViewerInstance;

namespace TrafficViewerControls
{
	/// <summary>
	/// Dialog used to get inform
	/// </summary>
	public partial class ConfirmClose : Form
	{
		private ConfirmCloseResult _result = ConfirmCloseResult.Cancel;
		/// <summary>
		/// Returns the result of the dialog
		/// </summary>
		public ConfirmCloseResult Result
		{
			get { return _result; }
			set { _result = value; }
		}

		public ConfirmClose()
		{
			InitializeComponent();
		}

		private void PackClick(object sender, EventArgs e)
		{
			_result = ConfirmCloseResult.Save;
			ProcessDontPrompt();
			this.Close();
		}

		private void LeaveClick(object sender, EventArgs e)
		{
			_result = ConfirmCloseResult.Leave;
			ProcessDontPrompt();
			this.Close();
		}

		private void DiscardClick(object sender, EventArgs e)
		{
			_result = ConfirmCloseResult.Discard;
			ProcessDontPrompt();
			this.Close();
		}



		private void ProcessDontPrompt()
		{
			if (_checkPrompt.Checked)
			{
				TrafficViewerOptions.Instance.ActionOnClose = (int)_result;
			}
			else
			{
				TrafficViewerOptions.Instance.ActionOnClose = (int)ConfirmCloseResult.Unknown;
			}
		}

	}

	public enum ConfirmCloseResult
	{ 
		Unknown,
		Save,
		Leave,
		Discard,
		Cancel
	}
}