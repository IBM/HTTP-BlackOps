using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.AnalysisModules;
using TrafficViewerSDK;

namespace TrafficViewerControls.AnalysisModules
{
	public partial class AnalysisProcessingPage : UserControl
	{
		/// <summary>
		/// The current analysis module
		/// </summary>
		private IAnalysisModule _module;

		/// <summary>
		/// The results of the analysis
		/// </summary>
		private IAnalysisResults _results;

		/// <summary>
		/// The current data source
		/// </summary>
		private ITrafficDataAccessor _source;

		private void WriteMessage(string message)
		{
			_textLog.Text += message + Environment.NewLine;
			_textLog.Select(_textLog.Text.Length - 1, 0);
			_textLog.ScrollToCaret();
		}

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="module"></param>
		/// <param name="source"></param>
		public AnalysisProcessingPage(IAnalysisModule module, ITrafficDataAccessor source)
		{
			InitializeComponent();
			_module = module;
			_source = source;

			_module.LogEvent += new AnalysisModuleLogEvent(ModuleLogEvent);

		}

		private void ExecutionCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			_buttonDone.Enabled = true;
			_progressTimer.Stop();
			_progressBar.Visible = false;

			WriteMessage("============================================");
			if (_results != null)
			{
				if(_results.ResultText != null)
				{
					WriteMessage(_results.ResultText);
				}
				if (!String.IsNullOrEmpty(_results.ResultBrowserContent))
				{
					AnalysisModulesHtmlView htmlView = new AnalysisModulesHtmlView(_results.ResultBrowserContent, _results.ResultBrowserContentExtension);
					htmlView.Show();
				}
			}

		}

		private void ExecuteModule(object sender, DoWorkEventArgs e)
		{
			try
			{
				_results = _module.PerformAnalysis(_source);
			}
			catch (Exception ex)
			{
				Invoke((MethodInvoker)delegate
				{
					WriteMessage(String.Format("Error in Analysis Module: {0}", ex.Message));
				});
			}
		}

		private void ModuleLogEvent(AnalysisModuleLogMessage e)
		{
			try
			{
				Invoke((MethodInvoker)delegate
				{
					WriteMessage(e.Message);
				});
			}
			catch { }
		}

		private void ProgressTimerTick(object sender, EventArgs e)
		{
			_progressTimer.Stop();

			if (_progressBar.Value < 100)
			{
				_progressBar.Value += _progressBar.Step;
			}
			else
			{
				_progressBar.Value = 0;
			}

			_progressTimer.Start();
		}

		private void AnalysisProcessingPage_Load(object sender, EventArgs e)
		{
			_progressTimer.Start();

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(ExecuteModule);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ExecutionCompleted);

			worker.RunWorkerAsync();
			
		}

		private void DoneClick(object sender, EventArgs e)
		{
			(this.Parent.Parent.Parent as Form).Close();
		}
	}
}
