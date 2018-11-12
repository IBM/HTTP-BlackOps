/******************************************************************
* IBM Confidential
* OCO Source Materials
* IBM Rational Traffic Viewer
* (c) Copyright IBM Corp. 2010 All Rights Reserved.
* 
* The source code for this program is not published or otherwise
* divested of its trade secrets, irrespective of what has been
* deposited with the U.S. Copyright Office.
******************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls
{
	public partial class OptionsEditor : Form
	{
		private static OptionsEditor _instance = null;
		private static object _lock = new object();

		/// <summary>
		/// Displays the current options editor instance
		/// </summary>
		public static void Display()
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = new OptionsEditor();
				}

				if (TrafficViewerOptions.Instance.MemoryBufferSize == 1 &&
					TrafficViewerOptions.Instance.EstimatedLineSize == 1024)
				{
					_instance._checkOptMemory.Checked = true;
				}
				_instance._numProxyPort.Value = (decimal)TrafficViewerOptions.Instance.ProxyPort;
				if (TrafficViewerOptions.Instance.ProxyMode == TrafficServerMode.Strict)
				{
					_instance._checkProxyStrict.Checked = true;
				}

				//initialize the variable definitions list (in case is not set)
				TrafficViewerOptions.Instance.VariableDefinitions = TrafficViewerOptions.Instance.VariableDefinitions;

				//set the definitions list in a option grid format
				_instance._gridVarDefs.SetValues((List<string>)
					TrafficViewerOptions.Instance.GetOption("VariableDefinitions"));
				
				//show the form
				_instance.ShowDialog();
			}

		}


		private OptionsEditor()
		{
			InitializeComponent();
		}

		private void CancelClick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void TogglePerfomance()
		{
			_checkOptMemory.Checked = !_checkOptSpeed.Checked;
			_checkOptSpeed.Checked = !_checkOptMemory.Checked;
		}

		private void ToggleProxyMode()
		{
			_checkProxyBrowser.Checked = !_checkProxyStrict.Checked;
			_checkProxyStrict.Checked = !_checkProxyBrowser.Checked;
		}

		private void OptMemoryCheckedChanged(object sender, EventArgs e)
		{
			TogglePerfomance();
		}

		private void OptSpeedCheckedChanged(object sender, EventArgs e)
		{
			TogglePerfomance();
		}

		private void ProxyStrictCheckedChanged(object sender, EventArgs e)
		{
			ToggleProxyMode();
		}

		private void ProxyBrowserCheckedChanged(object sender, EventArgs e)
		{
			ToggleProxyMode();
		}

		private void OkClick(object sender, EventArgs e)
		{
			if (_checkOptMemory.Checked)
			{
				TrafficViewerOptions.Instance.MemoryBufferSize = 1;
				TrafficViewerOptions.Instance.EstimatedLineSize = 1024;
			}
			else
			{
				TrafficViewerOptions.Instance.EstimatedLineSize = 10240;
				TrafficViewerOptions.Instance.MemoryBufferSize = 100;
			}

			TrafficViewerOptions.Instance.ProxyPort = (int)_numProxyPort.Value;

			if (_checkProxyStrict.Checked)
			{
				TrafficViewerOptions.Instance.ProxyMode = TrafficServerMode.Strict;
			}
			else
			{
				TrafficViewerOptions.Instance.ProxyMode = TrafficServerMode.BrowserFriendly;
			}

			//re-initialize the variable definitions so we can use the SetMultiValueOption method
			TrafficViewerOptions.Instance.VariableDefinitions = null;
			TrafficViewerOptions.Instance.SetMultiValueOption("VariableDefinitions", _gridVarDefs.GetValues());

			this.Hide();
		}
	}
}