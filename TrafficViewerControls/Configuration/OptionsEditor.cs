using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using System.IO;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Importers;
using TrafficViewerControls.Properties;
using TrafficViewerInstance;

namespace TrafficViewerControls
{
	public partial class OptionsEditor : Form
	{

		private void OptionsEditorLoad(object sender, EventArgs e)
		{
			if (TrafficViewerOptions.Instance.MemoryBufferSize == 1 &&
					TrafficViewerOptions.Instance.EstimatedLineSize == 1024)
			{
				_checkOptMemory.Checked = true;
				_checkOptSpeed.Checked = false;
			}
			else
			{
				_checkOptMemory.Checked = false;
				_checkOptSpeed.Checked = true;
			}

			_boxIp.Text = TrafficViewerOptions.Instance.TrafficServerIp;

			_numProxyPort.Value = (decimal)TrafficViewerOptions.Instance.TrafficServerPort;
            _numServerSecurePort.Value = (decimal)TrafficViewerOptions.Instance.TrafficServerPortSecure;

			
			_gridDynElems.SetValues(TrafficViewerOptions.Instance.GetDynamicElements());

			//initialize the variable definitions list (in case is not set)
			TrafficViewerOptions.Instance.VariableDefinitions = TrafficViewerOptions.Instance.VariableDefinitions;

			//set the definitions list in a option grid format
			_gridVarDefs.SetValues((List<string>)
				TrafficViewerOptions.Instance.GetOption("VariableDefinitions"));

				//startup info
			_fileAutoLoad.Text = TrafficViewerOptions.Instance.StartupImport;

			//load profiles
			List<string> profilePaths = TrafficViewerOptions.Instance.GetProfilePaths();
			string startupProfile = TrafficViewerOptions.Instance.StartupImportProfile;
			int selectedIndex = 0;
			_boxStartupProfile.Items.Clear();
			foreach (string path in profilePaths)
			{
				try
				{
					string fName = Path.GetFileName(path);
					_boxStartupProfile.Items.Add(fName);
					if (fName == startupProfile)
					{
						selectedIndex = _boxStartupProfile.Items.Count - 1;
					}
				}
				catch { }
			}
			_boxStartupProfile.SelectedIndex = selectedIndex;


			_boxStartupParser.Items.Clear();

			foreach (ITrafficParser parser in TrafficViewer.Instance.TrafficParsers)
			{
				_boxStartupParser.Items.Add(parser.Name);

				if (String.Compare(parser.Name, TrafficViewerOptions.Instance.StartupParser) == 0)
				{
					_boxStartupParser.SelectedIndex = _boxStartupParser.Items.Count - 1;
				}
			}

			_swatchBackground.Color = TVColorConverter.GetColorFromString(
				TrafficViewerOptions.Instance.ColorTextboxBackground);
			_swatchTextColor.Color = TVColorConverter.GetColorFromString(
				TrafficViewerOptions.Instance.ColorTextboxText);
			_swatchDiffColor.Color = TVColorConverter.GetColorFromString(
				TrafficViewerOptions.Instance.ColorDiffText);
			_swatchHighlightColor.Color = TVColorConverter.GetColorFromString(
				TrafficViewerOptions.Instance.ColorHighlight);

			_extensionGrid.SetValues((IEnumerable<string>)TrafficViewerOptions.Instance.GetOption("Extensions"));

			_checkPrompt.Checked = TrafficViewerOptions.Instance.ActionOnClose == (int)ConfirmCloseResult.Unknown;

			
			_gridSessionIds.SetValues(TrafficViewerOptions.Instance.GetSessionIdRegexes());

			_gridResponsePatterns.SetValues(TrafficViewerOptions.Instance.GetResponsePatterns());

			_boxHttpClient.Items.Clear();

			foreach (IHttpClientFactory clientFactory in TrafficViewer.Instance.HttpClientFactoryList)
			{
                _boxHttpClient.Items.Add(clientFactory.ClientType);

                if (String.Compare(clientFactory.ClientType, TrafficViewerOptions.Instance.HttpClientName) == 0)
				{
					_boxHttpClient.SelectedIndex = _boxHttpClient.Items.Count - 1;
				}
			}

			_checkIgnoreInvalidCert.Checked = TrafficViewerOptions.Instance.IgnoreInvalidSslCert;
			_checkUseProxy.Checked = TrafficViewerOptions.Instance.UseProxy;
			
			
			_boxProxyHost.Text = TrafficViewerOptions.Instance.HttpProxyServer;
			_proxyCertificate.Text = TrafficViewerOptions.Instance.ProxyCert;
			_boxProxyCertPass.Text = TrafficViewerOptions.Instance.ProxyCertPass;
			_boxProxyPort.Value = (decimal)TrafficViewerOptions.Instance.HttpProxyPort;
			_boxTimeout.Value = (decimal)TrafficViewerOptions.Instance.HttpRequestTimeout;
			_numericRequestDelay.Value = (decimal)TrafficViewerOptions.Instance.RequestDelay;
			_textRequestDelayFilter.Text = TrafficViewerOptions.Instance.RequestDelayFilter;

			
            _textForwardingHost.Text = TrafficViewerOptions.Instance.ForwardingHost;
            _numForwardingPort.Value = TrafficViewerOptions.Instance.ForwardingPort;

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

			TrafficViewerOptions.Instance.TrafficServerIp = _boxIp.Text;
			TrafficViewerOptions.Instance.TrafficServerPort = (int)_numProxyPort.Value;
            TrafficViewerOptions.Instance.TrafficServerPortSecure = (int)_numServerSecurePort.Value;

			//re-initialize the variable definitions so we can use the SetMultiValueOption method
			TrafficViewerOptions.Instance.VariableDefinitions = null;
			TrafficViewerOptions.Instance.SetMultiValueOption("VariableDefinitions", _gridVarDefs.GetValues());

			TrafficViewerOptions.Instance.SetDynamicElements(_gridDynElems.GetValues());
			//import at startup
			TrafficViewerOptions.Instance.StartupImport = _fileAutoLoad.Text;
			TrafficViewerOptions.Instance.StartupImportProfile = _boxStartupProfile.Text;

			TrafficViewerOptions.Instance.ColorTextboxBackground = TVColorConverter.GetARGBString(
					_swatchBackground.Color);
			TrafficViewerOptions.Instance.ColorTextboxText = TVColorConverter.GetARGBString(
					_swatchTextColor.Color);
			TrafficViewerOptions.Instance.ColorDiffText = TVColorConverter.GetARGBString(
					_swatchDiffColor.Color);
			TrafficViewerOptions.Instance.ColorHighlight = TVColorConverter.GetARGBString(
					_swatchHighlightColor.Color);

			TrafficViewerOptions.Instance.SetExtensions(_extensionGrid.GetValues());

			TrafficViewerOptions.Instance.SetResponsePatterns(_gridResponsePatterns.GetValues());
			TrafficViewerOptions.Instance.SetSessionIdRegexes(_gridSessionIds.GetValues());


			if (_checkPrompt.Checked)
			{
				TrafficViewerOptions.Instance.ActionOnClose = (int)ConfirmCloseResult.Unknown;
			}

			
			TrafficViewerOptions.Instance.StartupParser = _boxStartupParser.SelectedItem as string;

			TrafficViewerOptions.Instance.HttpClientName = _boxHttpClient.SelectedItem as string;

            //http clients
			TrafficViewer.Instance.HttpClientFactory = TrafficViewer.Instance.HttpClientFactoryList[_boxHttpClient.SelectedIndex];


			TrafficViewerOptions.Instance.IgnoreInvalidSslCert = _checkIgnoreInvalidCert.Checked;

			TrafficViewerOptions.Instance.UseProxy = _checkUseProxy.Checked;
			

			TrafficViewerOptions.Instance.HttpProxyServer = _boxProxyHost.Text;
			TrafficViewerOptions.Instance.ProxyCert = _proxyCertificate.Text;
			TrafficViewerOptions.Instance.ProxyCertPass = _boxProxyCertPass.Text;

			TrafficViewerOptions.Instance.HttpProxyPort = (int)_boxProxyPort.Value;
			TrafficViewerOptions.Instance.HttpRequestTimeout = (int)_boxTimeout.Value;
			TrafficViewerOptions.Instance.RequestDelay = (int)_numericRequestDelay.Value;
			TrafficViewerOptions.Instance.RequestDelayFilter = _textRequestDelayFilter.Text;
			
            if (String.IsNullOrEmpty(TrafficViewerOptions.Instance.ForwardingHost) && !String.IsNullOrEmpty(_textForwardingHost.Text))
            {
                if (MessageBox.Show(Resources.WarnForwarding, 
                    Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    _textForwardingHost.Text = String.Empty;
                }
            }

            TrafficViewerOptions.Instance.ForwardingHost = _textForwardingHost.Text;
            TrafficViewerOptions.Instance.ForwardingPort = (int)_numForwardingPort.Value;

			
			TrafficViewerOptions.Instance.Save();

			//TrafficViewer.Instance.LoadExtensions(); //this causes extensions added dynamically to be deleted

			//WarnRestartNeeded();
			TrafficViewer.InitSdkSettings();

			this.Hide();

		}

		private static void WarnRestartNeeded()
		{
			MessageBox.Show(
			TrafficViewerControls.Properties.Resources.InfoNeedsRestart,
			TrafficViewerControls.Properties.Resources.BoxTitle,
			MessageBoxButtons.OK,
			MessageBoxIcon.Information);
		}

		public OptionsEditor()
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

		private void OptMemoryCheckedChanged(object sender, EventArgs e)
		{
			TogglePerfomance();
		}

		private void OptSpeedCheckedChanged(object sender, EventArgs e)
		{
			TogglePerfomance();
		}


		private void OptionsEditorFormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		private void AnalysisModulesGridAddClick(object sender, EventArgs e)
		{
			if (_dialogOpenFile.ShowDialog(this) == DialogResult.OK)
			{
				_extensionGrid.AddRow(Enum.GetName(typeof(TrafficViewerExtensionFunction),
					TrafficViewerExtensionFunction.Disabled), _dialogOpenFile.FileName);
			}
		}

		private void _checkUseProxy_CheckedChanged(object sender, EventArgs e)
		{
			_boxProxyHost.ReadOnly = !_checkUseProxy.Checked;
			_boxProxyPort.ReadOnly = !_checkUseProxy.Checked;
		}

      

        private void ResetBlackopsCA(object sender, EventArgs e)
        {

            TrafficViewer.GenerateBlackopsCA();
            MessageBox.Show("New cert generated. Save and re-import in your browser trusted root CA store");
        }

        private void SaveBlackopsCA(object sender, EventArgs e)
        {
            if (_saveCertDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(_saveCertDialog.FileName, TrafficViewerOptions.Instance.CACertCer);
            }
        }

	


	}
}