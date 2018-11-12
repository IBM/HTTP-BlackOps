using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;

namespace TrafficViewerSDK.Importers
{
	/// <summary>
	/// Allows specifying the parser state
	/// </summary>
    public enum ParserThreadState
    {
		/// <summary>
		/// The parser is stopped
		/// </summary>
        Stopped,
		/// <summary>
		/// The parser is running
		/// </summary>
        Running
    }

	/// <summary>
	/// Information about threads in the traffic files
	/// </summary>
    public class ThreadInfo
    {
        private string _threadId=String.Empty;
		/// <summary>
		/// Id of the thread
		/// </summary>
        public string ThreadId
        {
            get
            {
                return _threadId;
            }
            set
            {
                _threadId = value;
            }
        }

        private LocationInThread _location = LocationInThread.NonHttpTraffic;
		/// <summary>
		/// Current location in that file following the thread
		/// </summary>
        internal LocationInThread Location
        {
            get { return _location; }
            set { _location = value; }
        }

		private bool _suspended = false;
		/// <summary>
		/// Indicates if the current thread was suspended by a end thread line
		/// </summary>
		public bool Suspended
		{
			get { return _suspended; }
			set { _suspended = value; }
		}


        private Stack<KeyValuePair<int,RequestResponseBytes>> _currentRequests=
            new Stack<KeyValuePair<int, RequestResponseBytes>>();
		/// <summary>
		/// Current requests
		/// </summary>
        public Stack<KeyValuePair<int, RequestResponseBytes>> CurrentRequests
        {
            get { return _currentRequests; }
            set { _currentRequests = value; }
        }

        private string _description=String.Empty;
		/// <summary>
		/// Description of the thread extracted from the traffic file
		/// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

		/// <summary>
		/// Shows the thread id
		/// </summary>
		/// <returns></returns>
        public override string ToString()
        {
            return _threadId;
        }

    }
}
