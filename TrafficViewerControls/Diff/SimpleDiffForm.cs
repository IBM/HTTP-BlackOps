using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TVDiff;
using TVDiff.Implementations;
using TrafficViewerControls.TextBoxes;
using System.Threading;
using TrafficViewerSDK.Options;
using TrafficViewerInstance;

namespace TrafficViewerControls.Diff
{
	public partial class SimpleDiffForm : Form
	{

		private ITrafficDataAccessor _source;
		private int _currFirstIndex;
		private int _currSecondIndex;

		private int _currFirstDiffIndex = -1;
		private int _currSecondDiffIndex = -1;

		private IDiffObjectsCollection _firstDiffs;
		private IDiffObjectsCollection _secondDiffs;

		private bool _wasNextEnabled = true;
		private bool _wasPrevEnabled = true;

		/// <summary>
		/// Flag signalling that a diff is in progress
		/// </summary>
		private bool _diffInProgress = false;

		private RtfBuilder _builder = new RtfBuilder();

		/// <summary>
		/// Switches diff buttons on or off
		/// </summary>
		/// <param name="enabled"></param>
		private void DiffButtonsSwitch(bool enabled)
		{
			if (!enabled)
			{
				_wasNextEnabled = _buttonNext.Enabled;
				_wasPrevEnabled = _buttonPrev.Enabled;
				_buttonNext.Enabled = enabled;
				_buttonPrev.Enabled = enabled;
			}
			else
			{
				_buttonNext.Enabled = _wasNextEnabled;
				_buttonPrev.Enabled = _wasPrevEnabled;
			}
		}


		private void DoDiff(int indexOfFirst, int indexOfSecond)
		{
			TVRequestInfo info1 = _source.GetRequestInfo(indexOfFirst);
			TVRequestInfo info2 = _source.GetRequestInfo(indexOfSecond);

			_requestID1.Text = info1.Id + ", " + info1.Description + ", " + info1.DomUniquenessId;
			_requestID2.Text = info2.Id + ", " + info2.Description + ", " + info2.DomUniquenessId; ;

			_progress.Visible = true;
			_timer.Start();

			//turn off the diff buttons so the user doesn't start another async operation by mistake
			DiffButtonsSwitch(false);

			_diffWorker.RunWorkerAsync(new int[2] { indexOfFirst, indexOfSecond });

		}		
			
		private void DiffDoWork(object sender, DoWorkEventArgs e)
		{
			_diffInProgress = true;
			int[] args = e.Argument as int[];
			
			if (args != null && args.Length > 1)
			{
				int indexOfFirst = args[0];
				int indexOfSecond = args[1];

				RequestsDiffer differ = new RequestsDiffer();
				e.Result = differ.Diff(indexOfFirst, indexOfSecond, _source);
			}
		}

		private void DiffWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_progress.Visible = false;
			_timer.Stop();

			RequestsDifferResult result = e.Result as RequestsDifferResult;
			if (result != null)
			{
				_labelSimilarity.Text = String.Format(Properties.Resources.SimilarityText,
					result.BodyAproximateSimilarity * 100);

				_firstDiffs = result.DiffsForFirst;
				_secondDiffs = result.DiffsForSecond;

				_boxFirst.Rtf = GetRtf(result.FirstText, result.DiffsForFirst);
				_boxSecond.Rtf = GetRtf(result.SecondText, result.DiffsForSecond);
			}

			//re-enable the diff buttons
			DiffButtonsSwitch(true);

			_diffInProgress = false;
		}

		private void ProgressTick(object sender, EventArgs e)
		{
			_timer.Stop();

			if (_progress.Value < 100)
			{
				_progress.Value += 10;
			}
			else
			{
				_progress.Value = 0;
			}

			_timer.Start();
		}

		private string GetRtf(string text, IDiffObjectsCollection differences)
		{
			int i, n = differences.Count;

			RtfHighlight[] diffHighlights = new RtfHighlight[n];

			for (i = 0; i < n; i++)
			{
				diffHighlights[i] = new RtfHighlight(differences[i]);
				diffHighlights[i].Color = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorDiffText);
			}

			return _builder.Convert(text, diffHighlights);
		}


		private string GetText(int index)
		{
			StringBuilder sb = new StringBuilder();

			byte[] bytes = _source.LoadRequestData(index);
			sb.Append(Constants.DefaultEncoding.GetString(bytes));
			sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
			bytes = _source.LoadResponseData(index);
			sb.Append(Constants.DefaultEncoding.GetString(bytes));

			return sb.ToString();
		}

		public SimpleDiffForm(ITrafficDataAccessor source, int indexOfFirst, int indexOfSecond)
		{
			InitializeComponent();
			_source = source;
			_currFirstIndex = Math.Min(indexOfFirst,indexOfSecond);
			_currSecondIndex = Math.Max(indexOfFirst,indexOfSecond);
			DoDiff(_currFirstIndex, _currSecondIndex);
		}

		private void NextClick(object sender, EventArgs e)
		{
			TVRequestInfo header1 = _source.GetNext(ref _currFirstIndex);
			TVRequestInfo header2 = _source.GetNext(ref _currSecondIndex);

			_currFirstDiffIndex = -1;
			_currSecondDiffIndex = -1;

			if (header1 != null && header2 != null)
			{
				if (!_buttonPrev.Enabled)
				{
					_buttonPrev.Enabled = true;
				}

				DoDiff(_currFirstIndex, _currSecondIndex);
			}
			else
			{
				_buttonNext.Enabled = false;
			}
		}

		private void PrevClick(object sender, EventArgs e)
		{
			TVRequestInfo header1 = _source.GetPrevious(ref _currFirstIndex);
			TVRequestInfo header2 = _source.GetPrevious(ref _currSecondIndex);

			_currFirstDiffIndex = -1;
			_currSecondDiffIndex = -1;

			if (header1 != null && header2 != null)
			{
				if (!_buttonNext.Enabled)
				{
					_buttonNext.Enabled = true;
				}

				DoDiff(_currFirstIndex, _currSecondIndex);
			}
			else
			{
				_buttonPrev.Enabled = false;
			}
		}

		private void ButtonNextDiffClick(object sender, EventArgs e)
		{
			NavigateDiff(1);
		}



		private void ButtonPrevDiffClick(object sender, EventArgs e)
		{
			NavigateDiff(-1);
		}

		/// <summary>
		/// Navigates to the next or previous diff
		/// </summary>
		/// <param name="cursor">1 to go next -1 to go previous</param>
		private void NavigateDiff(int cursor)
		{
			_currFirstDiffIndex += cursor;
			if (_currFirstDiffIndex < _firstDiffs.Count && _currFirstDiffIndex > -1)
			{
				//highlight the text
				_boxFirst.Select((int)_firstDiffs[_currFirstDiffIndex].Position,
								_firstDiffs[_currFirstDiffIndex].Length);
				
			}

			_currSecondDiffIndex += cursor;
			if (_currSecondDiffIndex < _secondDiffs.Count && _currSecondDiffIndex > -1)
			{
				_boxSecond.Select((int)_secondDiffs[_currSecondDiffIndex].Position,
								_secondDiffs[_currSecondDiffIndex].Length);
			}
		}

		private void SimpleDiffForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = _diffInProgress;
		}

		
	}
}