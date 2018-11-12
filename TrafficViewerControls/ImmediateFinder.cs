using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TrafficViewerControls
{
	public partial class ImmediateFinder : Form
	{
		private RichTextBox _target;
		private RequestTrafficView _view;
		private List<KeyValuePair<int, int>> _matches = new List<KeyValuePair<int,int>>();
		private int _currMatchIndex = -1;

		private ImmediateFinder()
		{
			InitializeComponent();
		}

		private static ImmediateFinder _instance = null;

		private static object _lock = new object();

		private void NewFind()
		{
			_matches.Clear();

			if (_comboLocation.SelectedIndex == (int)FindLocation.Request)
			{
				_target = _view.RequestBox;
			}
			else
			{
				_target = _view.ResponseBox;
			}

			string targetText = _target.Text;
			string what = _textSearch.Text;


			if (what != String.Empty && what != null)
			{

				if (_checkUseRegex.Checked)
				{
					MatchCollection rMatches = Regex.Matches(targetText, what,
						RegexOptions.Multiline|RegexOptions.IgnoreCase);
					foreach (Match m in rMatches)
					{
						_matches.Add(new KeyValuePair<int, int>(m.Index, m.Length));
					}
				}
				else
				{
					int start = 0;

					while ((start = targetText.IndexOf(what, start, StringComparison.OrdinalIgnoreCase)) > -1)
					{
						_matches.Add(new KeyValuePair<int, int>(start, what.Length));
						start += what.Length;
					}
				}
			}

			FindAgain();
		}

		private void FindAgain()
		{
			_target.SelectAll();
			_target.SelectionBackColor = Color.LightYellow;

			if (_matches.Count > 0)
			{
				if (_currMatchIndex >= _matches.Count - 1)
				{
					_currMatchIndex = 0;
				}
				else
				{
					_currMatchIndex++;
				}
				Highlight(_matches[_currMatchIndex].Key, _matches[_currMatchIndex].Value);
			}
		}

		private void Highlight(int start,int length)
		{
			_target.Select(start, length);
			_target.SelectionBackColor = Color.Yellow;
			_target.Select(start, 0);
			_target.ScrollToCaret();
		}

		private void FindButtonClick(object sender, EventArgs e)
		{
			NewFind();
		}


		/// <summary>
		/// Shows the find form 
		/// </summary>
		/// <param name="view">The view control</param>
		/// <param name="where">Which location should be highlighted by default</param>
		/// <param name="type">If this is a new search or just search again</param>
		public static void Find(RequestTrafficView view, FindLocation where, FindType type)
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = new ImmediateFinder();
				}

				if (type == FindType.NewFind)
				{

					_instance._comboLocation.SelectedIndex = (int)where;
					_instance._view = view;

					_instance.Show();
				}
				else
				{
					if (_instance._matches.Count > 0)
					{
						_instance.FindAgain();
					}
					else
					{
						_instance.Show();
					}
				}
			}
		}

		private void ImmediateFinderFormClosing(object sender, FormClosingEventArgs e)
		{
			_instance.Hide();
			e.Cancel = true;
		}

		private void FindAgainClick(object sender, EventArgs e)
		{
			FindAgain();
		}


	}

	public enum FindLocation
	{ 
		Request,
		Response
	}


	public enum FindType
	{ 
		NewFind,
		FindAgain
	}
}