using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using TrafficViewerSDK;
using TrafficViewerSDK.Search;

namespace TrafficViewerControls
{
	public partial class RequestLogSyncView : UserControl
	{
		private Random _randGenerator = new Random();

		private const int COLLAPSED_SIZE = 200;
		/// <summary>
		/// The range around the timestamp in seconds
		/// </summary>
		private const int RANGE = 1;

		private TimeStampsFinder _finder;

		private DateTime _currentTime;
		/// <summary>
		/// Specifies the time that should be searched in the attached log
		/// </summary>
		public DateTime EventTime
		{
			get
			{
				return _currentTime;
			}
			set
			{
				_currentTime = value;

				LoadTimeStamps();
			}
		}

		private void FileSelected(object sender, EventArgs e)
		{
			_browser.Document.All["message"].InnerText = String.Empty;

			_finder = new TimeStampsFinder(_fileSelector.Text);

			LoadTimeStamps();

		}

		private void LoadTimeStamps()
		{
			if (_currentTime != default(DateTime) && _finder != null)
			{
				try
				{
					_browser.Document.All["sectionsDiv"].InnerText = String.Empty;

					List<ByteArrayBuilder> entries = _finder.Find(_currentTime, RANGE);

					foreach (ByteArrayBuilder bytes in entries)
					{
						AddSection(Constants.DefaultEncoding.GetString(bytes.ToArray()));
					}
				}
				catch { }
			}
		}

		private void AddSection(string text)
		{
			text = Utils.HtmlEncode(text);
			text = text.Replace("\n", "\n<br/>");		

			int trimSize = Math.Min(text.Length, COLLAPSED_SIZE);

			string trimmedText = text.Substring(0, trimSize);

			if (trimSize == COLLAPSED_SIZE)
			{
				trimmedText += "...\n</br>";
			}

			trimmedText += "<br/>";
			text += "<br/>";

			uint hash = (uint)text.GetHashCode();
			string sectionName = hash.ToString();
			sectionName += _randGenerator.Next().ToString();


			_browser.Document.InvokeScript("AddSection", new object[3] { sectionName, trimmedText, text });

		}

		public RequestLogSyncView()
		{
			InitializeComponent();

			string html = Properties.Resources.CollapsibleElementsPage;

			html = html.Replace("{0}", Properties.Resources.LogSyncDefaultMessage);

			byte[] bytes = Constants.DefaultEncoding.GetBytes(html);


			_browser.DocumentStream = new MemoryStream(bytes);

			_fileSelector.FileSelected += new EventHandler(FileSelected);

		}

	}
}
