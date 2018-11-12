using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Defines a line type
	/// </summary>
   public enum LineType
    {
	   /// <summary>
	   /// Beginthread
	   /// </summary>
        BeginThread,
	   /// <summary>
		/// HttpTraffic
	   /// </summary>
        HttpTraffic,
	   /// <summary>
		/// FirstRequestLine
	   /// </summary>
        FirstRequestLine,
	   /// <summary>
		/// FirstResponseLine
	   /// </summary>
        FirstResponseLine,
	   /// <summary>
		/// EndOfFile
	   /// </summary>
        EndOfFile,
	   /// <summary>
	   /// EndThread
	   /// </summary>
        EndThread,
	   /// <summary>
	   /// NonHttpTraffic
	   /// </summary>
        NonHttpTraffic,
	   /// <summary>
	   /// ResponseReceived
	   /// </summary>
        ResponseReceived,
		/// <summary>
		/// Sending request message
		/// </summary>
		SendingRequest,
		/// <summary>
		///Receiving response message
		/// </summary>
		ReceivingResponse
    }

	/// <summary>
	/// Resolves to a line type based on regular expression definitions
	/// </summary>
    public class LineTypeSelector
    {

        #region Options

		private const byte CHAR_LOWER_LIMIT = 32;
		private const byte CHAR_UPPER_LIMIT = 124;

        private Regex _beginThreadRegex;
		private Regex _endThreadRegex;
		private Regex _requestLineRegex;
		private Regex _responseLineRegex;
		private Regex _responseReceivedMessageRegex;
		private Regex _nonHttpTrafficRegex;
		private Regex _sendingRequestRegex;
		private Regex _receivingResponseRegex;


        #endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parsingOptions"></param>
        public LineTypeSelector(ParsingOptions parsingOptions)
        {
            _beginThreadRegex = new Regex(parsingOptions.BeginThreadRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_endThreadRegex = new Regex(parsingOptions.EndThreadRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_requestLineRegex = new Regex(parsingOptions.RequestLineRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_responseLineRegex = new Regex(parsingOptions.ResponseLineRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_responseReceivedMessageRegex = new Regex(parsingOptions.ResponseReceivedMessageRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_nonHttpTrafficRegex = new Regex(parsingOptions.NonHttpTrafficRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_sendingRequestRegex = new Regex(parsingOptions.SendingRequestRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_receivingResponseRegex = new Regex(parsingOptions.ReceivingResponseRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

		/// <summary>
		/// Gets the type of the current line
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
        public LineType GetLineType(string line)
        {
			//order is important here, messaging should be before request/response regexes
			if (line == null) return LineType.EndOfFile;

			//determine without using regular expressions what is binary content and not log messages
			//log messages use characters between a certain range
			if (line.Length == 0) return LineType.HttpTraffic;
			
			byte c = (byte)line[0];
			if (c < CHAR_LOWER_LIMIT || c > CHAR_UPPER_LIMIT)
			{
				return LineType.HttpTraffic;
			}

            if (Utils.IsMatch(line, _beginThreadRegex)) return LineType.BeginThread;
            if (Utils.IsMatch(line, _endThreadRegex)) return LineType.EndThread;
            if (Utils.IsMatch(line, _responseReceivedMessageRegex)) return LineType.ResponseReceived;
			if (Utils.IsMatch(line, _sendingRequestRegex)) return LineType.SendingRequest;
			if (Utils.IsMatch(line, _receivingResponseRegex)) return LineType.ReceivingResponse;
			if (Utils.IsMatch(line, _nonHttpTrafficRegex)) return LineType.NonHttpTraffic;
            if (Utils.IsMatch(line, _requestLineRegex)) return LineType.FirstRequestLine;
            if (Utils.IsMatch(line, _responseLineRegex)) return LineType.FirstResponseLine;
			

            return LineType.HttpTraffic;
        }

    }
}
