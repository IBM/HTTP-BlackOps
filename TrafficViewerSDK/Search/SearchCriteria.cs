using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// List of search criteria
	/// </summary>
	public class SearchCriteriaSet : List<ISearchCriteria>
	{
		private string _descriptionFilter = String.Empty;
		/// <summary>
		/// Sets a description filter for the criteria
		/// </summary>
		public string DescriptionFilter
		{
			get { return _descriptionFilter; }
			set { _descriptionFilter = value; }
		}

		private int _startCriteriaIndex = 0;
		/// <summary>
		/// Gets/sets the start criteria
		/// </summary>
		public int StartCriteriaIndex
		{
			get { return _startCriteriaIndex; }
			set { _startCriteriaIndex = value; }
		}
		/// <summary>
		/// Overriden gethashcode
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int result = _descriptionFilter.GetHashCode();
			foreach (ISearchCriteria c in this)
			{
				result = result ^ c.GetHashCode();
			}
			return result;
		}
	}

	/// <summary>
	/// Encapsulates properties for a search criteria
	/// </summary>
	public class SearchCriteria : ISearchCriteria
	{
		List<string> _texts; // list of text to search
		SearchContext _context;
		bool _isRegex;

		/// <summary>
		/// Overriden get hash
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int result = _context.GetHashCode() ^ _isRegex.GetHashCode();
			foreach (string t in _texts)
			{
				result = result ^ t.GetHashCode();
			}
			return result;
		}

		/// <summary>
		/// Overriden Equals
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			SearchCriteria other = obj as SearchCriteria;
			if (other != null)
			{
				return this.GetHashCode() == other.GetHashCode();
			}

			return false;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="isRegex"></param>
		/// <param name="texts"></param>
		public SearchCriteria(SearchContext context, bool isRegex, params string[] texts)
		{
			_context = context;
			_isRegex = isRegex;
			_texts = new List<string>();
			_texts.AddRange(texts);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isRegex"></param>
		/// <param name="texts"></param>
		public SearchCriteria(bool isRegex, params string[] texts)
			: this(SearchContext.None, isRegex, texts)
		{
		}

		#region ISearchCriteria Members
		/// <summary>
		/// List of strings to look for
		/// </summary>
		public IList<string> Texts
		{
			get { return _texts; }
		}
		/// <summary>
		/// Context of the search
		/// </summary>
		public SearchContext Context
		{
			get { return _context; }
		}
		/// <summary>
		/// If the text contains regular expressions
		/// </summary>
		public bool IsRegex
		{
			get { return _isRegex; }
		}

		/// <summary>
		/// True if the criteria matches the specified string
		/// </summary>
		/// <param name="toBeSearched"></param>
		/// <returns></returns>
		public bool Matches(string toBeSearched)
		{
			List<MatchCoordinates> matchCoordinates;
			return Matches(toBeSearched, 0, out matchCoordinates);
		}


		/// <summary>
		/// This is an AND match between all the elements of the criteria
		/// </summary>
		/// <param name="toBeSearched"></param>
		/// <param name="relativePosition"></param>
		/// <param name="matchCoordinates"></param>
		/// <returns></returns>
		public bool Matches(string toBeSearched, int relativePosition, out List<MatchCoordinates> matchCoordinates)
		{
			matchCoordinates = new List<MatchCoordinates>(); //create a empty list of match coordinates
            bool found = false;
			foreach (string t in _texts)
			{
				if (String.IsNullOrEmpty(t))
				{
					continue; //skip empty entries
				}

				if (_isRegex)
				{

					MatchCollection matches = null;
					try
					{
						matches = Regex.Matches(toBeSearched, t);
					}
					catch
					{
						//ignore malformed regex
					}
					if (matches == null || matches.Count == 0)
					{
                        found |= false;
					}
					else
					{
                        found |= true;
						foreach (Match m in matches)
						{
							matchCoordinates.Add(new MatchCoordinates(relativePosition + m.Index, m.Length));
						}
					}
				}
				else
				{
					int startIndex = 0;

					int matchIndex;

					do
					{
						matchIndex = toBeSearched.IndexOf(t, startIndex, StringComparison.CurrentCultureIgnoreCase);
						if (matchIndex != -1)
						{
							matchCoordinates.Add(new MatchCoordinates(relativePosition + matchIndex, t.Length));
							startIndex = matchIndex + t.Length;
							found |= true;
						}
					}
					while (matchIndex > -1);

					
				}

			}
			return found;
		}

		#endregion
	}

	/// <summary>
	/// Represents a search
	/// </summary>
	public interface ISearchCriteria
	{
		/// <summary>
		/// Collection of strings to be used in the match. For now the match is an AND between the strings
		/// </summary>
		IList<string> Texts
		{
			get;
		}

		/// <summary>
		/// Where in the request to search
		/// </summary>
		SearchContext Context
		{
			get;
		}

		/// <summary>
		/// If the criteria is a regex
		/// </summary>
		bool IsRegex
		{
			get;
		}

		/// <summary>
		/// Checks if the object matches this criteria
		/// </summary>
		/// <param name="toBeSearched"></param>
		/// <returns></returns>
		bool Matches(string toBeSearched);

		/// <summary>
		/// Checks if the object matches this criteria
		/// </summary>
		/// <param name="toBeSearched"></param>
		/// <param name="relativePosition">The relative position of the string toBeSearched in a specific context (Request/Response etc..)</param>
		/// <param name="matchCoordinates">List of the location and length of the match for each text in the criteria</param>
		/// <returns></returns>
		bool Matches(string toBeSearched, int relativePosition, out List<MatchCoordinates> matchCoordinates);


	}

	/// <summary>
	/// Stores information about the location and length of a match
	/// </summary>
	public class MatchCoordinates
	{
		private int _matchPosition;
		/// <summary>
		/// Gets the position of the match
		/// </summary>
		public int MatchPosition
		{
			get { return _matchPosition; }
			set { _matchPosition = value; }
		}

		private int _matchLength;
		/// <summary>
		/// Gets the length of the match
		/// </summary>
		public int MatchLength
		{
			get { return _matchLength; }
			set { _matchLength = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		public MatchCoordinates(int start, int length)
		{
			_matchPosition = start;
			_matchLength = length;
		}
	}



}
