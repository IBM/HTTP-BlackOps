using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TrafficViewerSDK.Search;
using TrafficViewerSDK.Options;
using TrafficViewerInstance;

namespace TrafficViewerControls
{
    public partial class SearchForm : Form
    {
        private SearchCriteriaSet _criteriaSet;
        private LineMatches _matches;
        private LineSearcher _searcher;
        private const int LOAD_CHUNK_SIZE = 500; //how many search results to load at a time
        private const int MAX_LINE_SIZE = 255; //maximum line size to display in the search screen
        private int _last;
        private bool _requestTimerStop;

        /// <summary>
        /// Search control constructor
        /// </summary>
        /// <param name="dataSource">Data source of the search</param>
        public SearchForm()
        {
            InitializeComponent();

            _dropType.SelectedIndex = TrafficViewerOptions.Instance.LastSearchType;
        }

        /// <summary>
        /// Occurs when the search button was clicked 
        /// </summary>
        public event SearchExecutedEvent SearchExecuted;

        /// <summary>
        /// Occurs when an item was selected on the search form
        /// </summary>
        public event SearchIndexChangedEvent SearchIndexChanged;

        /// <summary>
        /// Occurs when the user clicks the Replace or Replace All buttons
        /// </summary>
        public event ReplaceEvent ReplaceRequestedEvent;


        private void SearchClick(object sender, EventArgs e)
        {
            StartSearch();
        }

        private void StartSearch()
        {
            if (_boxSearchText.Text != String.Empty)
            {
                _matches = new LineMatches();

                _buttonSearch.Enabled = false;
                _progressBar.Visible = true;
                this.Text = _boxSearchText.Text;

                //configure the search
                SearchContext context = (SearchContext)_dropType.SelectedIndex;
                TrafficViewerOptions.Instance.LastSearchType = _dropType.SelectedIndex;

                _searcher = new LineSearcher();
                //clear the search caches
                SearchResultCache.Instance.Clear();
                SearchSubsetsCache.Instance.Clear();




                _criteriaSet = new SearchCriteriaSet();

                _criteriaSet.DescriptionFilter = _boxDescriptionFilter.Text;

                string[] searchLines = _boxSearchText.Text.Split(new char[2] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);

                _criteriaSet.Add(new TrafficViewerSDK.Search.SearchCriteria(context,
                     _checkIsRegex.Checked, searchLines));


                _last = 0;
                _list.Items.Clear();

                //run the background worker
                _searchWorker.RunWorkerAsync();

                //start the load timer
                _requestTimerStop = false;
                _timer.Start();

                //invoke the event
                if (SearchExecuted != null)
                {
                    SearchExecuted.Invoke(new SearchExecutedEventArgs(_boxSearchText.Text, _checkIsRegex.Checked));
                }

            }
        }

        private void SearchWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            _searcher.Search(TrafficViewer.Instance.TrafficViewerFile, _criteriaSet, _matches);
        }

        private void SearchWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _requestTimerStop = true;

        }

        private void TimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            //update the progress bar
            if (_progressBar.Value >= 100)
            {
                _progressBar.Value = 0;
            }
            _progressBar.Value += _progressBar.Step;

            int i, n = _matches.Count;

            for (i = _last; i < n && i - _last < LOAD_CHUNK_SIZE; i++)
            {
                string line = _matches[i].Line.Length <= MAX_LINE_SIZE ? _matches[i].Line : _matches[i].Line.Substring(0, MAX_LINE_SIZE);
                _list.Items.Add(String.Format("{0:D6}: {1}",
                        _matches[i].RequestId, line));
            }
            _last = i;

            if (_last < n || !_requestTimerStop)
            {
                _timer.Start();
            }
            else
            {
                _progressBar.Visible = false;
                _buttonSearch.Enabled = true;
                _buttonReplaceOnce.Enabled = _buttonReplaceAll.Enabled = _matches.Count > 0;
            }
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SearchIndexChanged != null && _list.SelectedIndex > -1 && SearchIndexChanged != null)
            {
                LineMatch match = _matches[_list.SelectedIndex];
                SearchIndexChanged.Invoke(new SearchIndexChangedEventArgs(_boxSearchText.Text, match.RequestId, match.Line, _checkIsRegex.Checked));
            }
        }

        private void SearchTextKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.KeyChar = '\0';
                StartSearch();
            }
        }

        private void DescriptionFilterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.KeyChar = '\0';
                StartSearch();
            }
        }


        private void ReplaceOnceClick(object sender, EventArgs e)
        {
            if (ReplaceRequestedEvent != null && _list.SelectedIndex >= 0 && _list.SelectedIndex < _matches.Count)
            {
                LineMatch match = _matches[_list.SelectedIndex];
                List<LineMatch> matchesArg = new List<LineMatch>();
                matchesArg.Add(match);
                ReplaceRequestedEvent.Invoke(new ReplaceEventArgs(matchesArg, _boxReplace.Text));
                _matches.RemoveAt(_list.SelectedIndex);
                _list.Items.RemoveAt(_list.SelectedIndex);
            }
        }

        private void ReplaceAllClick(object sender, EventArgs e)
        {
            if (ReplaceRequestedEvent != null)
            {
                ReplaceRequestedEvent.Invoke(new ReplaceEventArgs(_matches, _boxReplace.Text));
                //clear all matches
                _list.Items.Clear();
                _buttonReplaceAll.Enabled = _buttonReplaceOnce.Enabled = false;
            }
        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            _searcher.StopSearch();
        }

        private void ButtonCopyClick(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LineMatch match in _matches)
            {
                sb.AppendLine(match.Line);
            }
            Clipboard.SetDataObject(sb.ToString());
        }


    }
}