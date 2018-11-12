using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using TrafficViewerSDK;
using TrafficViewerControls;
using TrafficServer;
using TrafficViewerSDK.AnalysisModules;
using TrafficViewerControls.AnalysisModules;
using TrafficViewerSDK.Options;
using TrafficViewerControls.Browsing;
using TrafficViewerControls.Properties;
using TrafficViewerSDK.Importers;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Search;
using TrafficViewerInstance;
using System.Diagnostics;
using System.Net.Security;
using TrafficViewerSDK.Exploiters;
using TrafficViewerControls.DefaultExploiters;
using System.Net;
using CommonControls;
using System.Text.RegularExpressions;

namespace TrafficViewerControls
{
	public partial class TrafficViewerForm : Form
	{
		private ITrafficParser _parser;
		private ParsingOptions _parsingOptions;
		private RequestViewerLoader _requestViewerLoader;
		private IEnumerable<string> _currentRawLogPaths;
		private string _currentTVFilePath = String.Empty;

		/// <summary>
		/// Flag indicating if the current traffic file has changed
		/// </summary>
		private bool _tvfChanged = false;
		private ImportInfo _lastImportInfo;
		SearchForm _searchForm;
		private AsyncCallback _asyncCallback;

		/// <summary>
		/// Used to show that a background operation is in progress
		/// </summary>
		private ProgressDialog _progressDialog;

		private ImportInfo _startupImportInfo = null;

		private string _nameAndVersion;
        private SearchCriteria _currentSearchCriteria;

		public TrafficViewerForm()
			: this(null, null)
		{

		}

		public TrafficViewerForm(ImportInfo startupImportInfo, string startupTVF)
		{
			InitializeComponent();

			_nameAndVersion = String.Format("{0} {1} ({2} {3})", TrafficViewerConstants.APP_NAME, TrafficViewerConstants.VERSION, Resources.Build, TrafficViewerConstants.BUILD);

			//initialize the progress dialog
			_progressDialog = new ProgressDialog();
			//create a callback to be passed to asyncroneus operations to stop the progress dialog
			_asyncCallback = new AsyncCallback(BackgroundOperationCallback);

			//create a new traffic viewer file
			NewTVF();

			//attach a handler to when the user modifies a request to know that the current
			//traffic data has changed
			_requestViewer.TrafficView.SaveRequested += new RequestTrafficViewSaveEvent(TrafficViewSaveRequested);

			//attach stop load command to request list
			_requestsList.StopLinkClick += new EventHandler(RequestsListOnStopLinkClicked);

			
			//points the unpacked folder dialog to the temp folder
			_dialogFolderBrowser.SelectedPath = Environment.GetEnvironmentVariable("temp");

			//initializes the analysis modules menu
			LoadAnalysisModules();

			//initializes the exploiters menu
			LoadExploiters();

			_menuStrip.ExploiterItemClicked += ExploiterItemClicked;

			//hooks a method to the analysis module click event
			_menuStrip.AnalysisModuleClicked += new AnalysisModuleClickEvent(MenuStripAnalysisModuleClicked);

			//hook up to the traffic visualizer click
			_menuStrip.VisualizeTraffic += new EventHandler(VisualizeTraffic);

			TrafficViewer.Instance.ExceptionMessageHandler = new ErrorBox();

			//search
			_searchForm = new SearchForm();
			_searchForm.SearchIndexChanged += new SearchIndexChangedEvent(SearchIndexChanged);
			_searchForm.SearchExecuted += new SearchExecutedEvent(SearchExecuted);
			_searchForm.ReplaceRequestedEvent += new ReplaceEvent(ReplaceRequested);

			//initialize the request trap with the request trap form
			//HttpTrap.Instance.RequestTrapped += new RequestTrapEvent(RequestTrapEvent);
			//HttpTrap.Instance.ResponseTrapped += new RequestTrapEvent(ResponseTrapEvent);
            HttpTrap.Instance.TrapOn += HttpTrapOn;
            HttpTrap.Instance.TrapOff += HttpTrapOff;
			//initialize the arguments, they will be processed on LOAD
			_startupImportInfo = startupImportInfo;
			_currentTVFilePath = startupTVF;

			HttpAuthenticationManager.Instance.CredentialsProvider = new CredentialsForm(this);
		}

        void HttpTrapOff(object sender, EventArgs e)
        {
            _requestsList.DisableAutoScroll = false;
        }

        void HttpTrapOn(object sender, EventArgs e)
        {
            _requestsList.DisableAutoScroll = true;
        }

		void ExploiterItemClicked(ExploiterClickArgs e)
		{
			e.Module.Execute(
				TrafficViewer.Instance.TrafficViewerFile,
				_requestsList.GetSelectedRequests(),
				TrafficViewer.Instance.HttpClientFactory);
		}

		void RequestTrapEvent(RequestTrapEventEventArgs e)
		{
			try
			{
                
				//check one more time
				if (HttpTrap.Instance.TrapRequests)
				{
                    bool autoScroll = _requestsList.DisableAutoScroll;
                    //disable autoscroll in the grid
                    _requestsList.DisableAutoScroll = true;

					RequestTrapForm form = new RequestTrapForm(e.TvReqInfo.RequestLine, false);
                    
					//show the form modal, this will freeze the current thread and prevent the request from being sent
					form.ShowDialog();
					//unlock the proxy thread
                    _requestsList.DisableAutoScroll = autoScroll;
				}
			}
			finally
			{
                
				e.ReqLock.Set();
			}

		}

		void ResponseTrapEvent(RequestTrapEventEventArgs e)
		{
			try
			{
				if (HttpTrap.Instance.TrapResponses)
				{
                    bool autoScroll = _requestsList.DisableAutoScroll;
                    ///disable autoscroll in the grid
                    _requestsList.DisableAutoScroll = true;
                    
					RequestTrapForm form = new RequestTrapForm(e.TvReqInfo.RequestLine, true);

					//show the form modal, this will freeze the current thread and prevent the request from being sent
					form.ShowDialog();
                    _requestsList.DisableAutoScroll = autoScroll;
                    //unlock the proxy thread
					e.HttpRespInfo.ProcessResponse(TrafficViewer.Instance.TrafficViewerFile.LoadResponseData(e.TvReqInfo.Id));
                    
				}
			}
			finally
			{
				e.ReqLock.Set();
			}

		}


		private void TrafficViewSaveRequested(RequestTrafficViewSaveArgs e)
		{
			_tvfChanged = true;
		}

		private void LoadAnalysisModules()
		{
			_menuStrip.LoadAnalysisModules(TrafficViewer.Instance.AnalysisModules);
		}

		private void LoadExploiters()
		{
			IList<IExploiter> exploiters = TrafficViewer.Instance.Exploiters;
            exploiters.Insert(0, new CertGenerator());
            exploiters.Insert(0, new CustomTestsExploiter());
            exploiters.Insert(0, new FuzzingExploiter());
            exploiters.Insert(0, new ManInTheMiddleExploiter());
            exploiters.Insert(0, new ScriptCrawlerExploiter());
            exploiters.Insert(0, new FormExploiter());
            
            _menuStrip.LoadExploiters(exploiters);
            
		}

		private void VisualizeTraffic(object sender, EventArgs e)
		{
			TrafficPlaybackBrowser form = new TrafficPlaybackBrowser(TrafficViewer.Instance.TrafficViewerFile);
			form.Show();
		}

		private void TrafficViewerMainLoad(object sender, EventArgs e)
		{

			//set the size of the main controls

			_menuStrip.Width = this.Width;


			_requestViewer.Size = new Size(_splitContainer.SplitterDistance -
				_splitContainer.SplitterWidth -
				_splitContainer.Margin.Right,
				_splitContainer.Height);
			_requestsList.Size = new Size(_splitContainer.Width -
				_splitContainer.SplitterDistance -
				_splitContainer.SplitterWidth -
				_splitContainer.Margin.Left
				, _splitContainer.Height - _splitContainer.Margin.Bottom);




			//import files that were specified with auto-load on startup or through command line arguments
			if (!String.IsNullOrEmpty(_currentTVFilePath))
			{
				OpenTvf();
			}
			else if (_startupImportInfo != null)
			{
				ImportLogStart(_startupImportInfo);
			}

			//execute onload events for plugins
			TrafficViewer.Instance.HttpClientFactory.OnLoad();

			//bring window to front
			this.BringToFront();
		}

		private void BackgroundOperationCallback(IAsyncResult result)
		{
			if (_progressDialog.IsHandleCreated)
			{
				_progressDialog.Invoke((MethodInvoker)delegate
				{
					_progressDialog.Stop();
					this.BringToFront();
				});
			}
			_requestsList.StopLoadingEvents = false;
			_requestsList.RequestEventsQueueSize = 0;
		}

		#region Request List & Viewer

		

		/// <summary>
		/// A new request was selected
		/// </summary>
		/// <param name="e"></param>
		private void RequestSelected(TVDataAccessorDataArgs e)
		{
            if (!String.IsNullOrWhiteSpace(e.Header.Validation)) 
            {
                _requestViewer.SetHighlighting(Regex.Replace(e.Header.Validation,"\\$(body|header)=",""), true);
            }
            else if (_currentSearchCriteria != null)
            {
                _requestViewer.SetHighlighting(_currentSearchCriteria.Texts[0], _currentSearchCriteria.IsRegex);
            }
            else
            {
                _requestViewer.SetHighlighting(String.Empty, false);
            }
			_requestViewerLoader.CallLoadRequest(e.RequestId);
		}

		#endregion

		#region New, Load, Tail & Clear

		private void NewTVF()
		{
			_currentTVFilePath = String.Empty;
			this.Text = String.Format("{0} - {1}", Resources.Untitled, _nameAndVersion);

			TrafficViewer.Instance.NewTvf();
			//attach to state changed to enable/disable menues accordingly
			TrafficViewer.Instance.TrafficViewerFile.StateChanged += new TVDataAccessorStateHandler(FileStateChanged);
			//attach the traffic viewer file to to request list as a data source
			_requestsList.DataSource = TrafficViewer.Instance.TrafficViewerFile;
            _proxyConsole.StopAllProxies();
            _proxyConsole.ReloadProxies();
			//attach a request viewer to the file
			_requestViewerLoader = new RequestViewerLoader(_requestViewer, TrafficViewer.Instance.TrafficViewerFile);

			//clear the search cache
			SearchResultCache.Instance.Clear();

		}


		private void NewClick(object sender, EventArgs e)
		{
			//closes the previous file
			if (!CloseTVF()) return;
			NewTVF();
			_menuStrip.State = TVMenuStripStates.Loaded;
		}


		/// <summary>
		/// Clears the traffic viewer file as well as the target log
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Clear(object sender, EventArgs e)
		{
			//warn the user
			DialogResult dr = MessageBox.Show(
				Resources.ClearWarning,
				Resources.Warning,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if (dr == DialogResult.Yes)
			{
				_requestsList.StopLoadingEvents = true;
				TrafficViewer.Instance.BeginClear(true, _asyncCallback);
				_progressDialog.Start();
			}
		}

		/// <summary>
		/// Happens when the request list stop link is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RequestsListOnStopLinkClicked(object sender, EventArgs e)
		{
			TrafficViewer.Instance.Tail = false;
			StopParser();
		}

		/// <summary>
		/// Stops an ongoing import
		/// </summary>
		private void StopParser()
		{
			if (TrafficViewer.Instance.Tail)
			{
				TrafficViewer.Instance.Tail = false;
			}
			TrafficViewer.Instance.CancelImport();
		}

		/// <summary>
		/// Changes the enabled menues based on the state of the file
		/// </summary>
		/// <param name="e"></param>
		private void FileStateChanged(TVDataAccessorStateArgs e)
		{
			if (TrafficViewer.Instance.TrafficViewerFile.State == AccessorState.Saving)
			{
				_tvfChanged = false;
			}
			else if (TrafficViewer.Instance.TrafficViewerFile.State != AccessorState.Idle)
			{
				_tvfChanged = true;
			}

			this.Invoke((MethodInvoker)delegate
			{
				if (TrafficViewer.Instance.TrafficViewerFile.State == AccessorState.Idle)
				{
					_menuStrip.State = TrafficViewerControls.TVMenuStripStates.Loaded;
				}
				else
				{
					_menuStrip.State = TrafficViewerControls.TVMenuStripStates.Loading;
				}
			});
		}

		/// <summary>
		/// Occurs when the main application form is closed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TrafficViewerMainFormClosing(object sender, FormClosingEventArgs e)
		{
			TrafficViewer.Instance.TrafficViewerFile.StateChanged -= new TVDataAccessorStateHandler(FileStateChanged);
			_requestsList.StopLoadingEvents = true;
			StopParser();
			if (!CloseTVF())
			{
				e.Cancel = true;
				return;
			}
            _proxyConsole.StopAllProxies();
		}

		private bool CloseTVF()
		{
			if (_tvfChanged && TrafficViewer.Instance.TrafficViewerFile.RequestCount > 0)
			{
				ConfirmCloseResult result = (ConfirmCloseResult)TrafficViewerOptions.Instance.ActionOnClose;
				if (result == ConfirmCloseResult.Unknown)
				{
					ConfirmClose confirmForm = new ConfirmClose();
					confirmForm.ShowDialog();
					result = confirmForm.Result;
				}
				if (result == ConfirmCloseResult.Cancel)
				{
					return false;//cancel operation
				}
				else if (result == ConfirmCloseResult.Save)
				{
					Save();
				}
				else if (result == ConfirmCloseResult.Leave)
				{
					TrafficViewer.Instance.CloseTvf(true);
				}
				else if (result == ConfirmCloseResult.Discard)
				{
					TrafficViewer.Instance.CloseTvf(false);
				}
			}
			else
			{
				TrafficViewer.Instance.CloseTvf(false);
			}

			return true;
		}

		/// <summary>
		/// Stops any load operations
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StopLoadOperation(object sender, EventArgs e)
		{
			TrafficViewer.Instance.CancelImport();
			TrafficViewer.Instance.Tail = false;
		}

		private void Tail(object sender, EventArgs e)
		{
			TrafficViewer.Instance.Tail = true;
		}

		/// <summary>
		/// Handles the import from log command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImportLog(object sender, EventArgs e)
		{

			ImportDialogResult res = ImportFileForm.Execute(_lastImportInfo);
			if (res.DialogResult == DialogResult.OK)
			{
				ImportLogStart(res.ImportInfo);
				//_requestsList.LoadRequests(TrafficViewer.Instance.TrafficDataAccessor); //deprecated
			}
		}

		private void ImportLogStart(ImportInfo importInfo)
		{

			_lastImportInfo = importInfo;

			if (!importInfo.Append)
			{
				if (!CloseTVF()) return;
				NewTVF();
			}

			_tvfChanged = true;
			_currentTVFilePath = String.Empty;
			_currentRawLogPaths = importInfo.TargetFiles;
			Text = String.Empty;
			foreach (string path in _currentRawLogPaths)
			{
				Text += Path.GetFileName(path) + "; ";
			}

			Text = Text.TrimEnd(' ', ';');
			if (importInfo.TargetFiles.Count == 0 && importInfo.Sender != null)
			{
				Text += String.Format("{0} - {1}", _lastImportInfo.Sender, _nameAndVersion);
			}
			else
			{
				Text += " - " + _nameAndVersion;
			}
			_parsingOptions = importInfo.Profile;
			//load the parser
			_parser = importInfo.Parser;

			try
			{
				TrafficViewer.Instance.BeginImport(importInfo, null);
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, ex.Message);
				MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Occurs whe the user clicks on the Open menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenClick(object sender, EventArgs e)
		{

			DialogResult dr = _dialogOpenFile.ShowDialog();
			if (dr == DialogResult.OK)
			{
				if (!CloseTVF()) return;

				_currentTVFilePath = _dialogOpenFile.FileName;
				//update the title bar
				OpenTvf();
			}
		}

		private void OpenTvf()
		{
			//clear the search cache
			SearchResultCache.Instance.Clear();

			Text = Path.GetFileName(_currentTVFilePath);
			Text += " - " + _nameAndVersion;
			_requestsList.RequestEventsQueueSize = 200;
			TrafficViewer.Instance.BeginOpenTvf(_currentTVFilePath, _asyncCallback);
			_progressDialog.Start();

		}

		private void OpenUnpackedClick(object sender, EventArgs e)
		{

			DialogResult dr = _dialogFolderBrowser.ShowDialog();
			if (dr == DialogResult.OK)
			{
				if (!CloseTVF()) return;

				TrafficViewer.Instance.BeginOpenUnpackedTvf(_dialogFolderBrowser.SelectedPath, _asyncCallback);

				_progressDialog.Start();
			}
		}

		#endregion

		#region Save

		/// <summary>
		/// Saves the file
		/// </summary>
		private void Save()
		{
			if (String.IsNullOrEmpty(_currentTVFilePath))
			{
				SaveAs();
			}
			else
			{
				TrafficViewer.Instance.BeginSaveTvf(_currentTVFilePath, _asyncCallback);
				_progressDialog.Start();
			}
		}

		/// <summary>
		/// Requires the user to specify the path for the current file
		/// </summary>
		private void SaveAs()
		{
			DialogResult dr = _dialogSaveFile.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_currentTVFilePath = _dialogSaveFile.FileName;
				Text = Path.GetFileName(_currentTVFilePath) + " - " + _nameAndVersion;
				TrafficViewer.Instance.BeginSaveTvf(_currentTVFilePath, _asyncCallback);
				_progressDialog.Start();
			}
		}

		private void SaveClick(object sender, EventArgs e)
		{
			Save();
		}

		private void SaveAsClick(object sender, EventArgs e)
		{
			SaveAs();
		}

		#endregion

		#region Proxy


		
		#endregion

		#region Search

		private void SearchClick(object sender, EventArgs e)
		{
			_searchForm.Show();
			_searchForm.BringToFront();
		}

		private void SearchExecuted(SearchExecutedEventArgs e)
		{
            _currentSearchCriteria = new SearchCriteria(e.IsRegex,e.SearchText);
            _requestViewer.SetHighlighting(e.SearchText, e.IsRegex);
		}

		private void SearchIndexChanged(SearchIndexChangedEventArgs e)
		{
            _currentSearchCriteria = new SearchCriteria(e.IsRegex, e.SearchText);
			_requestViewer.SetHighlighting(e.SearchText, e.IsRegex, e.LineMatch);
			_requestsList.Select(e.RequestId);
		}

		private void ReplaceRequested(ReplaceEventArgs e)
		{
			BackgroundWorker replaceWorker = new BackgroundWorker();
			replaceWorker.DoWork += new DoWorkEventHandler(ReplaceWorkerDoWork);
			replaceWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ReplaceWorkerCompleted);
			replaceWorker.RunWorkerAsync(e);
			_progressDialog.Start();

		}

		private void ReplaceWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_progressDialog.Stop();
			_requestViewerLoader.HandleReplace(e.Result as ReplaceEventArgs);
			_requestsList.HandleReplace(e.Result as ReplaceEventArgs);
			this.BringToFront();
		}

		private void ReplaceWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			ReplaceEventArgs rArgs = e.Argument as ReplaceEventArgs;
			if (rArgs != null)
			{
				TrafficViewer.Instance.TrafficViewerFile.Replace(rArgs.Matches, rArgs.Replacement);
			}

			e.Result = rArgs;
		}


		#endregion

		#region Other Menu Operations

		private void OptionsClick(object sender, EventArgs e)
		{
			OptionsEditor editor = new OptionsEditor();
			editor.ShowDialog();
		}

		void MenuStripAnalysisModuleClicked(TrafficViewerSDK.AnalysisModules.AnalysisModuleClickArgs e)
		{
			AnalisysModulesForm form = new AnalisysModulesForm(e.Module, TrafficViewer.Instance.TrafficViewerFile);
			form.Show();
		}

		
		private void ExportClick(object sender, EventArgs e)
		{
			ExportForm exportForm = new ExportForm(TrafficViewer.Instance.TrafficViewerFile);
			exportForm.ShowDialog();
		}



		#endregion

		private void LaunchEmbeddedBrowser()
		{
			ManualExploreBrowser meBrowser = new ManualExploreBrowser(TrafficViewer.Instance.TrafficViewerFile, null);
			meBrowser.Show();
		}


		private void EmbeddedBrowserClicked(object sender, EventArgs e)
		{
			/*Thread t = new Thread(new ThreadStart(LaunchEmbeddedBrowser));
			t.SetApartmentState(ApartmentState.STA);
			t.Start();*/
			LaunchEmbeddedBrowser();
		}

		private void ExternalBrowserClicked(object sender, EventArgs e)
		{
			ExternalBrowserListener listener = new ExternalBrowserListener(TrafficViewer.Instance.TrafficViewerFile);
			listener.Show();
		}

		private void CurrentProfileClicked(object sender, EventArgs e)
		{
			ProfileEditor.Edit(TrafficViewer.Instance.TrafficViewerFile.Profile);
		}

        



	}
}