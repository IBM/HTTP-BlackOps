using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficViewerSDK.Options
{

	public enum TrackingType
	{ 
		ResponsePattern,
		AutoDetect,
		Function
	}

    /// <summary>
    /// Stores a tracking pattern definition
    /// </summary>
    public class TrackingPattern
    {

		private TrackingType _trackingType = TrackingType.ResponsePattern;
		/// <summary>
		/// How will the tracking be done
		/// </summary>
		public TrackingType TrackingType
		{
			get { return _trackingType; }
			set { _trackingType = value; }
		}


        private string _name;
        /// <summary>
        /// The pattern name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
       
        private string _requestPattern;
        /// <summary>
        /// The pattern that will be replaced in requests
        /// </summary>
        public string RequestPattern
        {
            get { return _requestPattern; }
            set { _requestPattern = value; }
        }

        private string _trackingValue;
        /// <summary>
        /// The pattern that will be replaced in responses
        /// </summary>
        public string TrackingValue
        {
            get { return _trackingValue; }
            set { _trackingValue = value; }
        }

		/// <summary>
		/// Converts to string
		/// </summary>
		/// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}",_name,_requestPattern,_trackingType,_trackingValue);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
		/// <param name="requestPattern"></param>
		/// <param name="trackingType"></param>
        /// <param name="trackingValue"></param>
		public TrackingPattern(string name, string requestPattern, TrackingType trackingType, string trackingValue)
        {
            this._name = name;
			this._requestPattern = requestPattern;
            this._trackingValue = trackingValue;
			_trackingType = trackingType;
        }
    }
}
