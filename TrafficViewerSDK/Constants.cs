using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace TrafficViewerSDK
{
	/// <summary>
	/// Hoolds commonly used SDK constants
	/// </summary>
    public static class Constants
    {

        /// <summary>
        /// The string for the htd file type
        /// </summary>
        public const string HTD_STRING = "htd";
        /// <summary>
        /// The string for the exd file type
        /// </summary>
        public const string EXD_STRING = "exd";

        /// <summary>
        /// Byte array for newline
        /// </summary>
		public static byte[] NewLineBytes = new byte[2] { 13, 10 };

        /// <summary>
        /// Dynamically generated value
        /// </summary>
        public static readonly string SEQUENCE_VAR_PATTERN = "__dynamic_value__ticks__";

        /// <summary>
        /// Dynamically generated GUID value
        /// </summary>
        public static readonly string GUID_VAR_PATTERN = "__dynamic_value__guid__";
        
		/// <summary>
        /// \t
        /// </summary>
		public const string VALUES_SEPARATOR = "\t";
		/// <summary>
		/// Time format used to save timestamp data
		/// </summary>
        public const string COMMON_TIME_FORMAT = "MM-dd-yyyy HH:mm:ss.fff";
		/// <summary>
		/// String used to mark dynamic elements during request normalization
		/// </summary>
		public const string DYNAMIC_ELEM_STRING = "__d_el__";

		/// <summary>
		/// Gets the default encoding used to convert bytes to string
		/// </summary>
		public static Encoding DefaultEncoding
		{
			get
			{
				return new ModifiedExtendedASCIIEncoding();
			}
		}

        public const string FUZZ_STRING = "<fuzz>";
    }
}
