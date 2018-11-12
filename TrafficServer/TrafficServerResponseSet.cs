using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;

namespace TrafficServer
{

	public class TrafficServerResponseSet:ICloneable
	{
		private const int NULL_INDEX = -1;

		private int _currentIndex = NULL_INDEX;

		private List<int> _matches = new List<int>();
		/// <summary>
		/// Gets the list of matches
		/// </summary>
		public List<int> Matches
		{
			get { return _matches; }
		}


		public int Count
		{
			get
			{
				return _matches.Count;
			}
		}

		public void Reset()
		{
			_currentIndex = -1;
		}

		/// <summary>
		/// Gets the next element in the matches collection. If the last element was already returned 
		/// the first element is returned next
		/// </summary>
		/// <returns>Request id of the next match</returns>
		public int GetNext()
		{
			//there are no elements in the matches collection
			if (_matches.Count == 0)
			{
				return NULL_INDEX;
			}

			_currentIndex++;
			if (_currentIndex >= _matches.Count)
			{
				_currentIndex = 0;
			}

			return _matches[_currentIndex];
		}

		/// <summary>
		/// Adds a response id  to the set
		/// </summary>
		/// <param name="matchId"></param>
		public void Add(int matchId)
		{
			_matches.Add(matchId);
		}

		/// <summary>
		/// Adds a match collection to the current one
		/// </summary>
		/// <param name="matchIds"></param>
		public void AddRange(List<int> matchIds)
		{
			_matches.AddRange(matchIds);
		}


		#region ICloneable Members

		/// <summary>
		/// creates a deep copy of the response set
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			TrafficServerResponseSet clone = new TrafficServerResponseSet();
			clone.AddRange(_matches);
			clone._currentIndex = _currentIndex;
			return clone;
		}

		#endregion
	}
}
