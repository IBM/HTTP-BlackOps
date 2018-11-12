using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Search;

namespace TrafficViewerControls
{
	/// <summary>
	/// Find box
	/// </summary>
	public partial class ImmediateFinder : Form
	{

		private void CancelClick(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Occurs when the find button was clicked
		/// </summary>
		public event FindClickEvent FindClicked;

		private FinderResult _findResult = FinderResult.Unknown;
		/// <summary>
		/// Gets/sets if the last search found something
		/// </summary>
		public FinderResult FindResult
		{
			get
			{
				return _findResult;
			}
			set
			{
				_findResult = value;
			}
		}


		public override string Text
		{
			get
			{
				if (_textSearch != null)
				{
					return _textSearch.Text;
				}
				return String.Empty;
			}
			set
			{
				if (_textSearch != null)
				{
					_textSearch.Text = value;
				}
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ImmediateFinder()
		{
			InitializeComponent();
		}

		private void FindClick(object sender, EventArgs e)
		{
			if(FindClicked!=null)
			{
				FindClicked.Invoke(new FindClickEventArgs(_textSearch.Text,_checkUseRegex.Checked));
			}

			if (_findResult != FinderResult.NotFound)
			{
				this.Hide();
			}
		}

		private void TextSearchKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				FindClick(sender, null);
			}
		}

		private void ImmediateFinder_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
		}

		private void ImmediateFinder_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 27)
			{
				this.Hide();
			}
		}

	}

	public enum FinderResult
	{ 
		Unknown,
		Found,
		NotFound
	}

	/// <summary>
	/// Used by immediate finder
	/// </summary>
	/// <param name="e"></param>
	public delegate void FindClickEvent(FindClickEventArgs e);
	/// <summary>
	/// Information regarding the search criteria
	/// </summary>
	public class FindClickEventArgs : EventArgs
	{
		private SearchCriteria _searchCriteria;
		/// <summary>
		/// Search criteria
		/// </summary>
		public SearchCriteria SearchCriteria
		{
			get { return _searchCriteria; }
			set { _searchCriteria = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="searchText"></param>
		/// <param name="isRegex"></param>
		public FindClickEventArgs(string searchText, bool isRegex)
		{
			_searchCriteria = new SearchCriteria(isRegex, searchText);
		}
	}

}