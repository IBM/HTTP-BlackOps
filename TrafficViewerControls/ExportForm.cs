using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TrafficViewerSDK.Exporters;
using System.IO;
using TrafficViewerControls.Properties;
using TrafficViewerInstance;
using CommonControls;

namespace TrafficViewerControls
{
	public partial class ExportForm : Form
	{
		private ITrafficDataAccessor _source;
		private ProgressDialog _progressDialog;
		private AsyncCallback _callback;
		private Stream _stream;

		public ExportForm(ITrafficDataAccessor source)
		{
			InitializeComponent();

			_progressDialog = new ProgressDialog();

			_callback = new AsyncCallback(BackgroundOperationCallback);

			_source = source;
			_fileName.CheckFileExists = false;


			foreach (ITrafficExporter exporter in TrafficViewer.Instance.TrafficExporters)
			{
				_listExporters.Items.Add(exporter.Caption);
			}

			_listExporters.SelectedIndex = 0;

		}

		private void SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_listExporters.SelectedIndex >= 0 && _listExporters.SelectedIndex < TrafficViewer.Instance.TrafficExporters.Count)
			{
				ITrafficExporter currentExporter = TrafficViewer.Instance.TrafficExporters[_listExporters.SelectedIndex];
				_fileName.Filter = String.Format("{0} file|*.{1}", currentExporter.Caption, currentExporter.Extension);
			}
		}

		private void ExportClick(object sender, EventArgs e)
		{
			
			int newPort = 0;
			string newHost = _textNewHost.Text;

			if (_checkReplaceHost.Checked)
			{
				if (!String.IsNullOrEmpty(_textNewPort.Text) && !int.TryParse(_textNewPort.Text, out newPort))
				{
					MessageBox.Show(Resources.ErrorPort, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (Uri.CheckHostName(newHost) == UriHostNameType.Unknown)
				{
					MessageBox.Show(Resources.ErrorHost, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			try
			{
				_buttonExport.Enabled = false;
				ITrafficExporter currentExporter = TrafficViewer.Instance.TrafficExporters[_listExporters.SelectedIndex];
				_stream = File.Open(_fileName.Text, FileMode.Create, FileAccess.Write, FileShare.None);
				TrafficViewer.Instance.BeginExport(currentExporter, _stream, newHost, newPort, _callback);
				_progressDialog.Start();
				
			}
			catch(Exception ex)
			{	
				MessageBox.Show(String.Format(Resources.ExportError, ex.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			
		}

		private void BackgroundOperationCallback(IAsyncResult result)
		{
			if (_progressDialog.IsHandleCreated)
			{
				_progressDialog.Invoke((MethodInvoker)delegate
				{
					_progressDialog.Stop();
					_buttonExport.Enabled = true;
					_stream.Close();
					this.Close();
				});
			}
			
		}

		private void ReplaceHostCheckedChanged(object sender, EventArgs e)
		{
			_textNewHost.ReadOnly = !_checkReplaceHost.Checked;
			_textNewPort.ReadOnly = !_checkReplaceHost.Checked;
		}
	}
}
