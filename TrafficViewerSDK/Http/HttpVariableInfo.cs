using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Information about the http variable
	/// </summary>
    public class HttpVariableInfo
    {
        private string _name;
        /// <summary>
        /// Gets the variable name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private string _value;
        /// <summary>
        /// Gets the variable value
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _type;
        /// <summary>
        /// Gets the variable type, accroding to the variable definition
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }


        private RequestLocation _location;
        /// <summary>
        /// Gets the location in the reqeust according to the variable definition
        /// </summary>
        public RequestLocation Location
        {
            get { return _location; }
            set { _location = value; }
        }


        private bool _isTracked;
        /// <summary>
        /// Gets whether the variable is being tracked or not
        /// </summary>
        public bool IsTracked
        {
            get { return _isTracked; }
            set { _isTracked = value; }
        }

		/// <summary>
		/// Overriden GetHashCode method
		/// </summary>
		/// <returns></returns>
        public override int GetHashCode()
        {
            return _name.GetHashCode() ^
                    _value.GetHashCode() ^
                    _type.GetHashCode() ^
                    _location.GetHashCode() ^
                    _isTracked.GetHashCode();
        }
        

    }
}
